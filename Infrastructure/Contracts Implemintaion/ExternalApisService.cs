using System.Net.Http.Json;

using Application.Contracts;
using Application.DTOS;

namespace Infrastructure.Contracts_Implemintaion
{
    public class ExternalApisService(HttpClient httpClient) : IExternalApisService
    {
        private const string ViesApiUrl = "https://ec.europa.eu/taxation_customs/vies/rest-api/ms/{0}/vat/{1}";
        private readonly HttpClient _httpClient = httpClient;

        public async Task<(bool IsValid, string CompanyName)> ValidateVatAsync(string countryCode, string vatNumber)
        {
            var url = string.Format(ViesApiUrl, countryCode, vatNumber);

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return (false, "Service Unavailable");

            var result = await response.Content.ReadFromJsonAsync<ViesResponseDto>();
            return (result?.IsValid ?? false, result?.Name ?? "Unknown");
        }
    }
}