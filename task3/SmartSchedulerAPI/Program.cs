using SmartSchedulerAPI.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/api/v1/schedule", (SchedulerRequest req) =>
{
    var dayCapacity = (req.WorkDayEnd - req.WorkDayStart).TotalHours;
    var cursor = req.StartDate.Date;
    var results = new List<ScheduledItem>();

    var tasks = req.Tasks
        .OrderBy(t => t.DueDate ?? DateTime.MaxValue)
        .ThenByDescending(t => t.Priority)
        .ToList();

    foreach (var t in tasks)
    {
        double remaining = t.DurationHours;

        while (remaining > 0)
        {
            if (req.SkipWeekends && (cursor.DayOfWeek == DayOfWeek.Saturday || cursor.DayOfWeek == DayOfWeek.Sunday))
            {
                cursor = cursor.AddDays(1);
                continue;
            }

            double used = results
                .Where(r => r.Date == cursor)
                .Sum(r => (r.EndTime - r.StartTime).TotalHours);

            double free = dayCapacity - used;
            if (free <= 0)
            {
                cursor = cursor.AddDays(1);
                continue;
            }

            double allocate = Math.Min(remaining, free);
            var start = req.WorkDayStart + TimeSpan.FromHours(used);
            var end = start + TimeSpan.FromHours(allocate);

            results.Add(new ScheduledItem
            {
                TaskId = t.Id,
                Title = t.Title,
                Date = cursor,
                StartTime = start,
                EndTime = end
            });

            remaining -= allocate;
            if (remaining > 0) cursor = cursor.AddDays(1);
        }
    }

    return Results.Ok(results);
});

app.Run();
