using CalendarApplication.Shared.Models;
using OperationResults;

namespace CalendarApplication.BusinessLayer.Services.Interfaces;

public interface IMeService
{
    Task<Result<User>> GetAsync();
}