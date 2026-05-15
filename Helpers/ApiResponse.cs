// Models/ApiResponse.cs
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }

    public string MessageCode { get; set; }
    public T Data { get; set; }
    public List<string> Errors { get; set; }
    public DateTime Timestamp { get; set; }

    public ApiResponse()
    {
        Timestamp = DateTime.UtcNow;
        Errors = new List<string>();

    }

    public static ApiResponse<T> SuccessResponse(T data, string messageCode, string message = "Operación exitosa")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            MessageCode = messageCode,
            Data = data
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, string messageCode ,List<string> errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            MessageCode = messageCode,
            Data = default,
            Errors = errors ?? new List<string>()
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, string messageCode)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            MessageCode = messageCode,
            Data = default,
            //Errors = new List<string> { error }
        };
    }
}