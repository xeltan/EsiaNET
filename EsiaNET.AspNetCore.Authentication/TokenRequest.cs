namespace EsiaNET.AspNetCore.Authentication
{
    /// <summary>
    ///     Token request type enumeration
    /// </summary>
    public enum TokenRequest
    {
        ByAuthCode,
        ByRefresh,
        ByCredential
    }
}