namespace FMS.Core.Models
{
    public class SsoApiModel
    {
        public string Url { get; set; }
        public string ClientId { get; set; }
        public string ClientIdWeb { get; set; }
        public SsoEndpoint Endpoint { get; set; }
    }

    public class SsoEndpoint
    {
        public string Application { get; set; }
        public string ValidateToken { get; set; }
        public string RefreshTokenEndpoint { get; set; }
        public string Logout { get; set; }
    }
}
