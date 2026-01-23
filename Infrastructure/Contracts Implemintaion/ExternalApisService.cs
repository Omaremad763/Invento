using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;
using Application.DTOS;

using AutoMapper;

using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Contracts_Implemintaion
{
    public class ExternalApisService:IExternalApisService
    {
        private const string ViesApiUrl = "https://ec.europa.eu/taxation_customs/vies/rest-api/ms/{0}/vat/{1}";
        private readonly HttpClient _httpClient;

        public ExternalApisService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<(bool IsValid, string CompanyName)> ValidateVatAsync(string countryCode, string vatNumber)
        {

            var url = string.Format(ViesApiUrl, countryCode, vatNumber);

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return (false, "Service Unavailable");

            var result = await response.Content.ReadFromJsonAsync<ViesResponseDTO>();
            return (result?.IsValid ?? false, result?.Name ?? "Unknown");
        }
    }
}
