using System;

namespace Domainr.Manager.Core.Data.Models
{
    public sealed class Endpoint
    {
        public Guid Id { get; set; }

        public string Url { get; set; }

        public string AuthEndpointUrl { get; set; }

        public string ClientKey { get; set; }

        public string ClientSecret { get; set; }
    }
}