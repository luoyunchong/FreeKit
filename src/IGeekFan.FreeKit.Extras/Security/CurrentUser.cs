using System.Security.Claims;
using IGeekFan.FreeKit.Extras.Dependency;
using IGeekFan.FreeKit.Extras.Extensions;
using Microsoft.AspNetCore.Http;

namespace IGeekFan.FreeKit.Extras.Security;

public class CurrentUser : CurrentUser<long>, ICurrentUser, ITransientDependency
{
    public CurrentUser(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
    }

    /// <summary>
    /// 用户Id
    /// </summary>
    public long? Id
    {
        get
        {
            string? userId = ClaimsPrincipal?.FindUserId();
            if (userId == null) return null;
            return userId.ToLong();
        }
    }
}

/// <summary>
/// 当前用户
/// </summary>
/// <typeparam name="T"></typeparam>
public class CurrentUser<T> : ICurrentUser<T>
{
    private static readonly Claim[] EmptyClaimsArray = Array.Empty<Claim>();
    protected readonly ClaimsPrincipal ClaimsPrincipal;
    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        ClaimsPrincipal = httpContextAccessor.HttpContext?.User ?? Thread.CurrentPrincipal as ClaimsPrincipal;
    }

    /// <summary>
    /// 是否登录
    /// </summary>
    public bool IsAuthenticated => ClaimsPrincipal?.FindUserId() != null ? true : false;
    /// <summary>
    /// 用户Id，唯一值
    /// </summary>
    public T Id => throw new Exception("需要重写");
    /// <summary>
    /// 登录名，唯一值
    /// </summary>
    public string? UserName => ClaimsPrincipal?.FindUserName();
    /// <summary>
    /// 昵称
    /// </summary>
    public string? NickName => ClaimsPrincipal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
    /// <summary>
    /// 邮件
    /// </summary>
    public string? Email => ClaimsPrincipal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
    /// <summary>
    /// 角色
    /// </summary>
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
