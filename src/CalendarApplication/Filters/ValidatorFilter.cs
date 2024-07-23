using System.Diagnostics;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using OperationResults.AspNetCore.Http;

namespace CalendarApplication.Filters;

public class ValidatorFilter<T> : IEndpointFilter where T : class
{
    private readonly IValidator<T> validator;
    private readonly OperationResultOptions options;

    public ValidatorFilter(IValidator<T> validator, OperationResultOptions options)
    {
        this.validator = validator;
        this.options = options;
    }

    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.Arguments.FirstOrDefault(a => a.GetType() == typeof(T)) is T input)
        {
            var validationResult = await validator.ValidateAsync(input);
            if (!validationResult.IsValid)
            {
                var httpContext = context.HttpContext;
                var contentType = "application/problem+json; charset=utf-8";
                var statusCode = StatusCodes.Status400BadRequest;

                var problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Type = $"https://httpstatuses.io/{statusCode}",
                    Title = "One or more validation errors occurred",
                    Instance = httpContext.Request.Path
                };

                problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;
                LoadErrors(problemDetails, validationResult);

                return TypedResults.Json(problemDetails, statusCode: statusCode, contentType: contentType);
            }

            return await next(context);
        }

        return TypedResults.BadRequest();
    }

    private void LoadErrors(ProblemDetails problemDetails, ValidationResult validationResult)
    {
        if (options.ErrorResponseFormat == ErrorResponseFormat.Default)
        {
            problemDetails.Extensions["errors"] = validationResult.ToDictionary();
        }
        else
        {
            var errors = validationResult.Errors.Select(e => new { Name = e.PropertyName, Message = e.ErrorMessage });
            problemDetails.Extensions["errors"] = errors;
        }
    }
}