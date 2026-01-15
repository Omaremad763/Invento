namespace Presentation
{
    public class GlobalApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }
        // Constructor لحالة النجاح
        public GlobalApiResponse(T data)
        {
            Success = true;
            Data = data;
            Errors = null;
        }
        // Constructor لحالة الفشل
        public GlobalApiResponse(List<string> errors)
        {
            Success = false;
            Data = default;
            Errors = errors;
        }
    }
}

