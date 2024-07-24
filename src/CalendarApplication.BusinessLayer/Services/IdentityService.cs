using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CalendarApplication.Authentication;
using CalendarApplication.Authentication.Entities;
using CalendarApplication.Authentication.Extensions;
using CalendarApplication.BusinessLayer.Services.Interfaces;
using CalendarApplication.BusinessLayer.Settings;
using CalendarApplication.Shared.Models.Requests;
using CalendarApplication.Shared.Models.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OperationResults;

namespace CalendarApplication.BusinessLayer.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly JwtSettings jwtSettings;

    public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        IOptions<JwtSettings> jwtSettingsOptions)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        jwtSettings = jwtSettingsOptions.Value;
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByNameAsync(request.UserName);
        var signInResult = await signInManager.PasswordSignInAsync(user, request.Password, request.IsPersistent, false);

        if (signInResult.Succeeded)
        {
            var userRoles = await userManager.GetRolesAsync(user);
            await userManager.UpdateSecurityStampAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.SerialNumber, user.SecurityStamp)
            }
            .Union(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var accessToken = CreateToken(claims);
            return new AuthResponse(accessToken);
        }

        if (signInResult.IsLockedOut)
        {
            return Result.Fail(FailureReasons.ClientError, $"You're locked out until {user.LockoutEnd}");
        }

        await userManager.AccessFailedAsync(user);
        return Result.Fail(FailureReasons.ClientError, "Invalid username or password");
    }

    public async Task<Result> RegisterAsync(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            result = await userManager.AddToRoleAsync(user, RoleNames.User);
        }

        return result.Succeeded ? Result.Ok() : Result.Fail(FailureReasons.ClientError, result.GetErrors());
    }

    private string CreateToken(IEnumerable<Claim> claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecurityKey));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;
        var expiration = now.AddHours(1);

        var jwtSecurityToken = new JwtSecurityToken
        (
            jwtSettings.Issuer,
            jwtSettings.Audience,
            claims,
            now,
            expiration,
            signingCredentials
        );

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        return jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
    }
}