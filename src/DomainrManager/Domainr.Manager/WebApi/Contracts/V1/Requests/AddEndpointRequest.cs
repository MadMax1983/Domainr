using System.ComponentModel.DataAnnotations;

namespace Domainr.Manager.WebApi.Contracts.V1.Requests
{
    public sealed class AddEndpointRequest
    {
        [Required]
        public string Url { get; set; }

        public string AuthEndpointUrl { get; set; }

        public string ClientKey { get; set; }

        public string ClientSecret { get; set; }
    }
}