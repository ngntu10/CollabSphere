namespace CollabSphere.Common;

public class ApiResponse<T>
{
    private ApiResponse() { }

    private ApiResponse(int statusCode, bool succeeded, T result, IEnumerable<string> errors)
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

    public static ApiResponse<T> Success(int statusCode, T result)
    {
        return new ApiResponse<T>(statusCode, true, result, new List<string>());
    }

    public static ApiResponse<T> Failure(int statusCode, IEnumerable<string> errors)
    {
        return new ApiResponse<T>(statusCode, false, default, errors);
    }
}
