using System;

namespace Presentation
{
    public class GlobalApiResponse<T>(T data, bool success = true)
    {
        public bool Success { get; set; } = success;
        public T? Data { get; set; } = data;
        public List<string>? Errors { get; set; }
    }
    public static class ApiResponse
    {
        public static GlobalApiResponse<T> Success<T>(T data) => new(data);
        public static GlobalApiResponse<object?> Success() => new(null);
        public static GlobalApiResponse<object?> Failure(List<string> errors) => new(null, false) { Errors = errors };

    }
}


