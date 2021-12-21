namespace IGeekFan.FreeKit.Extras.Security;

[Serializable]
public class Tokens
{
    public Tokens(string accessToken, string refreshToken)
    {
        AccessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
        RefreshToken = refreshToken ?? throw new ArgumentNullException(nameof(refreshToken));
    }

    public string AccessToken { get; private set; }

    public string RefreshToken { get; private set; }

    public override string ToString()
    {
        return $"Tokns AccessToken:{AccessToken},RefreshToken:{RefreshToken}";
    }
}
