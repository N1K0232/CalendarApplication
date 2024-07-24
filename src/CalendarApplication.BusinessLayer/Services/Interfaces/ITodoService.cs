using CalendarApplication.Shared.Models;
using CalendarApplication.Shared.Models.Requests;
using OperationResults;

namespace CalendarApplication.BusinessLayer.Services.Interfaces;

public interface ITodoService
{
    Task<Result> DeleteAsync(Guid id);

    Task<Result<Todo>> GetAsync(Guid id);

    Task<Result<PaginatedList<Todo>>> GetListAsync(string name, string orderBy, int pageIndex, int itemsPerPage);

    Task<Result<Todo>> InsertAsync(SaveTodoRequest request);

    Task<Result<Todo>> UpdateAsync(Guid id, SaveTodoRequest request);
}