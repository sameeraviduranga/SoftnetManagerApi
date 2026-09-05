namespace SoftnetManager.Modules.Identity.Application.Result
{
    public class Result<T>
    {
        public bool IsSuccess { get; init; }
        public T? Data { get; init; }
        public string? Error { get; init; }

        public static Result<T> Success(T? data)
        {
            return new Result<T>
            {
                IsSuccess = true,
                Data = data
            };
        }

        public static Result<T> Fail(string error)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Error = error
            };
        }


    }
}
