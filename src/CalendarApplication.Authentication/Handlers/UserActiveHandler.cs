using System.Security.Claims;
using CalendarApplication.Authentication.Entities;
using CalendarApplication.Authentication.Extensions;
using CalendarApplication.Authentication.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CalendarApplication.Authentication.Handlers;

public class UserActiveHandler : AuthorizationHandler<UserActiveRequirement>
{
    private readonly UserManager<ApplicationUser> userManager;

    public UserActiveHandler(UserManager<ApplicationUser> userManager)
    {
        this.userManager = userManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserActiveRequirement requirement)
    {
        if (context.User.Identity.IsAuthenticated)
        {
            var user = await userManager.FindByNameAsync(context.User.GetUserName());
            var lockedOut = await userManager.IsLockedOutAsync(user);
            var securityStamp = context.User.GetClaimValueInternal(ClaimTypes.SerialNumber);

            if (!lockedOut && user.SecurityStamp == securityStamp)
            {
                context.Succeed(requirement);
            }
        }
    }
}