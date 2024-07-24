using System.Security.Claims;
using System.Security.Principal;

namespace CalendarApplication.Authentication.Extensions;

public static class ClaimsExtensions
{
    public static Guid GetId(this IPrincipal user)
    {
        var value = GetClaimValueInternal(user, ClaimTypes.NameIdentifier);
        if (Guid.TryParse(value, out var id))
        {
            return id;
        }

        return Guid.Empty;
    }

    public static string GetEmail(this IPrincipal user)
        => GetClaimValueInternal(user, ClaimTypes.Email);

    public static string GetUserName(this IPrincipal user)
        => GetClaimValueInternal(user, ClaimTypes.Name);

    public static string GetClaimValue(this IPrincipal user, string claimType)
        => GetClaimValueInternal(user, claimType);

    internal static string GetClaimValueInternal(this IPrincipal user, string claimType)
    {
        var value = ((ClaimsPrincipal)user).FindFirstValue(claimType);
        return value ?? string.Empty;
    }
}