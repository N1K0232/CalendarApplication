using System.Linq.Dynamic.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CalendarApplication.Authentication.Extensions;
using CalendarApplication.BusinessLayer.Services.Interfaces;
using CalendarApplication.DataAccessLayer;
using CalendarApplication.Shared.Models;
using CalendarApplication.Shared.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OperationResults;
using TinyHelpers.Extensions;
using Entities = CalendarApplication.DataAccessLayer.Entities;

namespace CalendarApplication.BusinessLayer.Services;

public class TodoService : ITodoService
{
    private readonly IApplicationDbContext dbContext;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IMapper mapper;

    public TodoService(IApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.httpContextAccessor = httpContextAccessor;
        this.mapper = mapper;
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var todo = await dbContext.GetAsync<Entities.Todo>(id);
        if (todo is not null)
        {
            await dbContext.DeleteAsync(todo);
            var affectedRows = await dbContext.SaveAsync();

            if (affectedRows > 0)
            {
                return Result.Ok();
            }

            return Result.Fail(FailureReasons.ClientError, "No todo deleted");
        }

        return Result.Fail(FailureReasons.ItemNotFound, $"No todo found with id {id}");
    }

    public async Task<Result<Todo>> GetAsync(Guid id)
    {
        var dbTodo = await dbContext.GetAsync<Entities.Todo>(id);
        if (dbTodo is not null)
        {
            var todo = mapper.Map<Todo>(dbTodo);
            return todo;
        }

        return Result.Fail(FailureReasons.ItemNotFound, $"No todo found with id {id}");
    }

    public async Task<Result<PaginatedList<Todo>>> GetListAsync(string name, string orderBy, int pageIndex, int itemsPerPage)
    {
        var query = dbContext.GetData<Entities.Todo>();

        if (name.HasValue())
        {
            query = query.Where(t => t.Name.Contains(name));
        }

        if (orderBy.HasValue())
        {
            query = query.OrderBy(orderBy);
        }

        var totalCount = await query.CountAsync();
        var todos = await query.ProjectTo<Todo>(mapper.ConfigurationProvider).ToListAsync();

        return new PaginatedList<Todo>(todos, totalCount, pageIndex, itemsPerPage);
    }

    public async Task<Result<Todo>> InsertAsync(SaveTodoRequest request)
    {
        var todo = mapper.Map<Entities.Todo>(request);
        todo.UserId = httpContextAccessor.HttpContext.User.GetId();

        await dbContext.InsertAsync(todo);
        var affectedRows = await dbContext.SaveAsync();

        if (affectedRows > 0)
        {
            var savedTodo = mapper.Map<Todo>(todo);
            return savedTodo;
        }

        return Result.Fail(FailureReasons.ClientError, "no todo added");
    }

    public async Task<Result<Todo>> UpdateAsync(Guid id, SaveTodoRequest request)
    {
        var query = dbContext.GetData<Entities.Todo>(true);
        var todo = await query.FirstOrDefaultAsync(t => t.Id == id);

        if (todo is not null)
        {
            mapper.Map(request, todo);
            var affectedRows = await dbContext.SaveAsync();

            if (affectedRows > 0)
            {
                var savedTodo = mapper.Map<Todo>(todo);
                return savedTodo;
            }

            return Result.Fail(FailureReasons.ClientError, "no todo added");
        }

        return Result.Fail(FailureReasons.ItemNotFound, $"No todo found with id {id}");
    }
}