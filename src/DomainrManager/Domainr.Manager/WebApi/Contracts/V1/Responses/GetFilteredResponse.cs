using System;

namespace Domainr.Manager.WebApi.Contracts.V1.Responses
{
    public sealed class GetFilteredResponse
    {
        public GetFilteredResponse(Guid id, string url)
        {
            Id = id;

            Url = url;
        }

        public Guid Id { get; }

        public string Url { get; }
    }
}