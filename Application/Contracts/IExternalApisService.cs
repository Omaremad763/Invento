namespace Application.Contracts
{
    public interface IExternalApisService
    {
        Task<(bool IsValid, string CompanyName)> ValidateVatAsync(string countryCode, string vatNumber);
    }
}