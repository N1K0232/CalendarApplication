using CalendarApplication.Authentication.Entities;
using CalendarApplication.DataAccessLayer.Entities.Common;

namespace CalendarApplication.DataAccessLayer.Entities;

public class Todo : BaseEntity
{
    public Guid UserId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public DateOnly StartDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public DateOnly? FinishDate { get; set; }

    public TimeOnly? FinishTime { get; set; }

    public virtual ApplicationUser User { get; set; }
}