using System;

namespace Presentation
{
    public class GlobalApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public List<string> Errors { get; set; }
        public GlobalApiResponse(T data)
        {
            Success = true;
            Data = data;
            Errors = null;
        }
        public GlobalApiResponse()
        {
            Success = false;
            Data = default;
            Errors = null;
        }
    }
}

