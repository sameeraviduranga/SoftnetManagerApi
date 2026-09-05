using System.Net;

namespace SoftnetManager.Modules.Identity.Api.Response
{
    public class ApiResponse<T> where T : class
    {
        public bool IsSuccess { get; init; }
        public T? Data { get; init; }
       // public HttpStatusCode statusCode { get; set; } no need
        public string? Message { get; init; }
        public object? Error { get; init; }
        public ApiResponse(bool isSuccess,T? data,string? message,object? error)
        {
            IsSuccess = isSuccess;
            Data = data;
            Message = message;
            Error = error;

        }

        public static ApiResponse<T> Success(T? data,string message = "")
        {
            return new ApiResponse<T>(true, data, message, null);
        }

        public static ApiResponse<T> Fail(object error, string message = "")
        {
            return new ApiResponse<T>(false, default, message, error);
        }

    }
}
