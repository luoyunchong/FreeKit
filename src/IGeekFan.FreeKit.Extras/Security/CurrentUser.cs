using System.Security.Claims;
using IGeekFan.FreeKit.Extras.Dependency;
using Microsoft.AspNetCore.Http;

namespace IGeekFan.FreeKit.Extras.Security;

public class CurrentUser : ICurrentUser, ITransientDependency
{
    private static readonly Claim[] EmptyClaimsArray = Array.Empty<Claim>();
    protected readonly ClaimsPrincipal ClaimsPrincipal;
    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        ClaimsPrincipal = httpContextAccessor.HttpContext?.User ?? Thread.CurrentPrincipal as ClaimsPrincipal;
    }

    public bool IsAuthenticated => ClaimsPrincipal?.FindUserId() != null ? true : false;
    public string Id => ClaimsPrincipal?.FindUserId();

    public string? UserName => ClaimsPrincipal?.FindUserName();
    public string? NickName => ClaimsPrincipal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
    public string? Email => ClaimsPrincipal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

    public string[] Roles => FindClaims(ClaimTypes.Role).Select(c => c.Value.ToString()).ToArray();

    public virtual Claim? FindClaim(string claimType)
    {
        return ClaimsPrincipal?.Claims.FirstOrDefault(c => c.Type == claimType);
    }

    public virtual Claim[] FindClaims(string claimType)
    {
        return ClaimsPrincipal?.Claims.Where(c => c.Type == claimType).ToArray() ?? EmptyClaimsArray;
    }

    public virtual Claim[] GetAllClaims()
    {
        return ClaimsPrincipal?.Claims.ToArray() ?? EmptyClaimsArray;
    }

    public virtual bool IsInRole(string roleId)
    {
        return FindClaims(ClaimTypes.Role).Any(c => c.Value == roleId);
    }
}
