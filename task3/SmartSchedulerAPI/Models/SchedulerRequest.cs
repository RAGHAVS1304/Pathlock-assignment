public record TaskInput(
    int Id,
    string Title,
    double DurationHours,
    DateTime? DueDate,
    int Priority
);

public record SchedulerRequest(
    List<TaskInput> Tasks,
    DateTime StartDate,
    TimeSpan WorkDayStart,
    TimeSpan WorkDayEnd,
    bool SkipWeekends = true
);
