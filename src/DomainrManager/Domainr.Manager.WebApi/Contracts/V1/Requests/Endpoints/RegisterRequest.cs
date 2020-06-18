namespace Domainr.Manager.WebApi.Contracts.V1.Requests.Endpoints
{
    public sealed class RegisterRequest
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public string AuthEndpointUrl { get; set; }

        public string ClientKey { get; set; }

        public string ClientSecret { get; set; }
    }
}