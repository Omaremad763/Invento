namespace Application.DTOS
{
    public class ResourceParameters
    {
        //Pagination Parameters
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        //filtering Parameters
        public string? SearchTerm { get; set; }

        public Guid? CategoryId { get; set; }

        //Sorting Parameters
        public string? OrderBy { get; set; }
    }
}