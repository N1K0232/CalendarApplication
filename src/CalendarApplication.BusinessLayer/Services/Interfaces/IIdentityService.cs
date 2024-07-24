using CalendarApplication.Shared.Models.Requests;
using CalendarApplication.Shared.Models.Responses;
using OperationResults;

namespace CalendarApplication.BusinessLayer.Services.Interfaces;

public interface IIdentityService
{
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);

    Task<Result> RegisterAsync(RegisterRequest request);
}