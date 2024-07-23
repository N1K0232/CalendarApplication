using System.Net.Http.Headers;
using System.Text;
using CalendarApplication.BusinessLayer.Settings;
using CalendarApplication.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using TinyHelpers.Extensions;

namespace CalendarApplication.Swagger;

public class SwaggerBasicAuthenticationMiddleware
{
    private readonly RequestDelegate next;
    private readonly SwaggerSettings swaggerSettings;

    public SwaggerBasicAuthenticationMiddleware(RequestDelegate next, IOptions<SwaggerSettings> swaggerSettingsOptions)
    {
        this.next = next;
        swaggerSettings = swaggerSettingsOptions.Value;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (httpContext.IsSwaggerRequest() && swaggerSettings.UserName.HasValue() && swaggerSettings.Password.HasValue())
        {
            string authenticationHeader = httpContext.Request.Headers[HeaderNames.Authorization];
            if (authenticationHeader?.StartsWith("Basic ") ?? false)
            {
                var header = AuthenticationHeaderValue.Parse(authenticationHeader);
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(header.Parameter)).Split(':', count: 2);

                var userName = credentials.ElementAtOrDefault(0);
                var password = credentials.ElementAtOrDefault(1);

                if (userName == swaggerSettings.UserName && password == swaggerSettings.Password)
                {
                    await next.Invoke(httpContext);
                    return;
                }
            }

            httpContext.Response.Headers.WWWAuthenticate = new StringValues("Basic");
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
        else
        {
            await next.Invoke(httpContext);
        }
    }
}