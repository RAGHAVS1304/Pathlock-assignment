using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagerBackend.Data;
using ProjectManagerBackend.Models;
using System.Security.Claims;

namespace ProjectManagerBackend.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _db;
    public TasksController(AppDbContext db) => _db = db;

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("{projectId}")]
    public async Task<IActionResult> AddTask(int projectId, TaskCreateDto dto)
    {
        var uid = GetUserId();
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == projectId && p.OwnerId == uid);
        if (project == null) return NotFound("Project not found");

        var task = new TaskItem { Title = dto.Title, DueDate = dto.DueDate, ProjectId = project.Id };
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
        return Ok(task);
    }

    [HttpPut("{taskId}/toggle")]
    public async Task<IActionResult> ToggleTask(int taskId)
    {
        var uid = GetUserId();
        var task = await _db.Tasks.Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.Project!.OwnerId == uid);
        if (task == null) return NotFound();

        task.IsCompleted = !task.IsCompleted;
        await _db.SaveChangesAsync();
        return Ok(task);
    }

    [HttpDelete("{taskId}")]
    public async Task<IActionResult> DeleteTask(int taskId)
    {
        var uid = GetUserId();
        var task = await _db.Tasks.Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.Project!.OwnerId == uid);
        if (task == null) return NotFound();

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public record TaskCreateDto(string Title, DateTime? DueDate);
