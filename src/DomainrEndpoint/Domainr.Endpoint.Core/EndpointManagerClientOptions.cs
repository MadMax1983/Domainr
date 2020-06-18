namespace Domainr.Endpoint.Core
{
    public sealed class EndpointManagerClientOptions
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string MicroserviceName { get; set; }

        public string EndpointManagerUrl { get; set; }

        public string EndpointManagerKey { get; set; }

        public string EndpointManagerSecret { get; set; }
    }
}