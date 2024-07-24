using CalendarApplication.BusinessLayer.Services.Interfaces;
using CalendarApplication.Shared.Models;
using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;

namespace CalendarApplication.Endpoints;

public class MeEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var meEndpoint = endpoints.MapGroup("/api/me");

        meEndpoint.MapGet(string.Empty, GetAsync)
            .RequireAuthorization("UserActive")
            .Produces<User>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi();
    }

    private static async Task<IResult> GetAsync(IMeService meService, HttpContext httpContext)
    {
        var result = await meService.GetAsync();
        return httpContext.CreateResponse(result);
    }
}