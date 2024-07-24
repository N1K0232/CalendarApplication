using CalendarApplication.BusinessLayer.Services.Interfaces;
using CalendarApplication.Shared.Models;
using CalendarApplication.Shared.Models.Requests;
using MinimalHelpers.Routing;
using OperationResults;
using OperationResults.AspNetCore.Http;

namespace CalendarApplication.Endpoints;

public class TodoEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var todoApiGroup = endpoints.MapGroup("/api/todos");

        todoApiGroup.MapDelete("{id:guid}", DeleteAsync)
            .RequireAuthorization("UserActive")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        todoApiGroup.MapGet("{id:guid}", GetAsync)
            .WithName("GetTodo")
            .RequireAuthorization("UserActive")
            .Produces<Todo>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        todoApiGroup.MapGet(string.Empty, GetListAsync)
            .RequireAuthorization("UserActive")
            .Produces<PaginatedList<Todo>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi();

        todoApiGroup.MapPost(string.Empty, InsertAsync)
            .RequireAuthorization("UserActive")
            .Produces<Todo>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi();

        todoApiGroup.MapPut("{id:guid}", UpdateAsync)
            .RequireAuthorization("UserActive")
            .Produces<Todo>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();
    }

    private static async Task<IResult> DeleteAsync(ITodoService todoService, Guid id, HttpContext httpContext)
    {
        var result = await todoService.DeleteAsync(id);
        return httpContext.CreateResponse(result);
    }

    private static async Task<IResult> GetAsync(ITodoService todoService, Guid id, HttpContext httpContext)
    {
        var result = await todoService.GetAsync(id);
        return httpContext.CreateResponse(result);
    }

    private static async Task<IResult> GetListAsync(ITodoService todoService, HttpContext httpContext, string name = null, string orderBy = "Name", int pageIndex = 0, int itemsPerPage = 10)
    {
        var result = await todoService.GetListAsync(name, orderBy, pageIndex, itemsPerPage);
        return httpContext.CreateResponse(result);
    }

    private static async Task<IResult> InsertAsync(ITodoService todoService, SaveTodoRequest request, HttpContext httpContext)
    {
        var result = await todoService.InsertAsync(request);
        return httpContext.CreateResponse(result, "GetTodo", new { id = result.Content?.Id });
    }

    private static async Task<IResult> UpdateAsync(ITodoService todoService, Guid id, SaveTodoRequest request, HttpContext httpContext)
    {
        var result = await todoService.UpdateAsync(id, request);
        return httpContext.CreateResponse(result);
    }
}