using System.Text.Json.Serialization;

namespace Presentation.ResponseHandling.Response;

/// <summary>
/// Base success response (No data attached)
/// </summary>
public class BaseResponse
{
    public string Message { get; protected set; } = "Success";
    
    public BaseResponse()
    {
        Message = "Success";
    }
}

/// <summary>
/// Response with any data
/// </summary>
public class ResponseWithData<T> : BaseResponse
{
    public T Data { get; protected set; }

    [JsonConstructor]
    public ResponseWithData(T data)
        : base()
    {
        Data = data;
    }
}