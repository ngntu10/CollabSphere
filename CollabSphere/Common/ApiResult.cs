namespace CollabSphere.Common;

public class ApiResult<T>
{
    private ApiResult() { }

    private ApiResult(int statusCode, bool succeeded, T result, IEnumerable<string> errors)
    {
        StatusCode = statusCode;
        Succeeded = succeeded;
        Result = result;
        Errors = errors;
    }

    public int StatusCode { get; set; }

    public bool Succeeded { get; set; }

    public T Result { get; set; }

    public IEnumerable<string> Errors { get; set; }

    public static ApiResult<T> Success(int statusCode, T result)
    {
        return new ApiResult<T>(statusCode, true, result, new List<string>());
    }

    public static ApiResult<T> Failure(int statusCode, IEnumerable<string> errors)
    {
        return new ApiResult<T>(statusCode, false, default, errors);
    }
}
