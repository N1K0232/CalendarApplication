namespace CalendarApplication.Shared.Models.Requests;

public record class SaveTodoRequest(string Name, string? Description, DateTime StartDate, DateTime? FinishDate);