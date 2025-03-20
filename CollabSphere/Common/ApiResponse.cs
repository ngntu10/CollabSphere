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
        Message = "";
    }

    private ApiResponse(int statusCode, bool succeeded, T result, IEnumerable<string> errors, string message)
    {
        StatusCode = statusCode;
        Succeeded = succeeded;
        Result = result;
        Errors = errors;
        Message = message;
    }

    public int StatusCode { get; set; }

    public bool Succeeded { get; set; }

    public T Result { get; set; }

    public IEnumerable<string> Errors { get; set; }

    public string Message { get; set; }

    public static ApiResponse<T> Success(int statusCode, T result, string message)
    {
        return new ApiResponse<T>(statusCode, true, result, new List<string>(), message);
    }

    public static ApiResponse<T> Failure(int statusCode, IEnumerable<string> errors)
    {
        return new ApiResponse<T>(statusCode, false, default, errors);
    }
}
