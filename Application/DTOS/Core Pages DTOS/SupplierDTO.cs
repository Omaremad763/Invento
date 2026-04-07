namespace Application.DTOS
{
    public record SupplierDto(
        Guid Id,
        string Name,
        string ContactEmail,
        string PhoneNumber,
        string? Vatstatus
    );
    public record AddSupplierDto
(
    string Name,
    string ContactEmail,
    string PhoneNumber
);

    public record UpdateSupplierDto(
    Guid Id,
    string Name,
    string ContactEmail,
    string PhoneNumber
);
}