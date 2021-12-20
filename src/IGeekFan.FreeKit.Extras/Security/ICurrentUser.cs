using System.Security.Claims;

namespace IGeekFan.FreeKit.Extras.Security;

public interface ICurrentUser
{
    string Id { get; }

    string UserName { get; }

    string[] Roles { get; }

    Claim FindClaim(string claimType);

    Claim[] FindClaims(string claimType);

    Claim[] GetAllClaims();

    bool IsInRole(string roleId);
}


public static class CurrentUserExtensions
{
    public static Guid? GetGuidUserId(this ICurrentUser currentUser)
    {
        if (currentUser.Id == null) return null;
        return Guid.Parse(currentUser.Id);
    }
    public static long? GetLongUserId(this ICurrentUser currentUser)
    {
        if (currentUser.Id == null) return null;
        return long.Parse(currentUser.Id);
    }

}
