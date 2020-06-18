namespace Domainr.Manager.WebApi.Contracts.V1.Requests
{
    public sealed class GetFilteredRequest
    {
        public int Page { get; set; }

        public int ItemsPerPage { get; set; }

        public string OrderBy { get; set; }

        public string OrderType { get; set; }
    }
}