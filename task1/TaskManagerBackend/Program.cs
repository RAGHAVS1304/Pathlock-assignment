using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

record TaskItem(Guid Id, string Description, bool IsCompleted, DateTime CreatedAt);
record TaskCreateDto(string Description);

var tasks = new List<TaskItem>
{
    new(Guid.NewGuid(), "Sample Task 1", false, DateTime.UtcNow),
    new(Guid.NewGuid(), "Sample Task 2", true, DateTime.UtcNow)
};

app.MapGet("/api/tasks", () => Results.Ok(tasks));

app.MapGet("/api/tasks/{id:guid}", (Guid id) =>
{
    var task = tasks.FirstOrDefault(t => t.Id == id);
    return task is null ? Results.NotFound() : Results.Ok(task);
});

app.MapPost("/api/tasks", (TaskCreateDto dto) =>
{
    var newTask = new TaskItem(Guid.NewGuid(), dto.Description, false, DateTime.UtcNow);
    tasks.Add(newTask);
    return Results.Created($"/api/tasks/{newTask.Id}", newTask);
});

app.MapPut("/api/tasks/{id:guid}/toggle", (Guid id) =>
{
    var index = tasks.FindIndex(t => t.Id == id);
    if (index < 0) return Results.NotFound();

    var updated = tasks[index] with { IsCompleted = !tasks[index].IsCompleted };
    tasks[index] = updated;
    return Results.Ok(updated);
});

app.MapDelete("/api/tasks/{id:guid}", (Guid id) =>
{
    var removed = tasks.RemoveAll(t => t.Id == id) > 0;
    return removed ? Results.NoContent() : Results.NotFound();
});

app.Run();
