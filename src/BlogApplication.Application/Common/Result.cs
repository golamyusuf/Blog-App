namespace BlogApplication.Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> Errors { get; set; } = new();

    public static Result<T> Success(T data)
    {
        return new Result<T>
        {
            IsSuccess = true,
            Data = data
        };
    }

    public static Result<T> Failure(string errorMessage)
    {
        return new Result<T>
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            Errors = new List<string> { errorMessage }
        };
    }

    public static Result<T> Failure(List<string> errors)
    {
        return new Result<T>
        {
            IsSuccess = false,
            ErrorMessage = string.Join(", ", errors),
            Errors = errors
        };
    }
}
