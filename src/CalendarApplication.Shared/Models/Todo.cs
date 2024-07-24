using CalendarApplication.Shared.Models.Common;

namespace CalendarApplication.Shared.Models;

public class Todo : BaseObject
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public DateOnly? FinishDate { get; set; }

    public TimeOnly? FinishTime { get; set; }
}