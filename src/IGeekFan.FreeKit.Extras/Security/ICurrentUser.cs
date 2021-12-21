using System.Security.Claims;

namespace IGeekFan.FreeKit.Extras.Security;

public interface ICurrentUser: ICurrentUser<long?>
{

}
public interface ICurrentUser<T>
{
    T Id { get; }

    string UserName { get; }

    string[] Roles { get; }

    Claim FindClaim(string claimType);

    Claim[] FindClaims(string claimType);

    Claim[] GetAllClaims();

    bool IsInRole(string roleId);
}


public static class CurrentUserExtensions
{
    public static Guid? GetGuidUserId(this ICurrentUser<string> currentUser)
    {
        if (currentUser.Id == null) return null;
        return Guid.Parse(currentUser.Id);
    }
    public static long? GetLongUserId(this ICurrentUser<string> currentUser)
    {
        if (currentUser.Id == null) return null;
        return long.Parse(currentUser.Id);
    }

}
