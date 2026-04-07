namespace Application.DTOS
{
    public class PaginatedResult<T>
    {
        public IReadOnlyList<T> Data { get; set; }
        public int PageNumber { get; set; }
        public int TotalCount { get; set; }

        #region indicator to put buttons previous and next and remove button next when no more pages

        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPreviousPage => PageNumber > 1;

        #endregion indicator to put buttons previous and next and remove button next when no more pages

        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public PaginatedResult(List<T> data, int count, int pageNumber, int pageSize)
        {
            Data = data;
            TotalCount = count;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}