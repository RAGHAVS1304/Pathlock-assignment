public class ScheduledItem
{
    public int TaskId { get; set; }
    public string Title { get; set; } = "";
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
