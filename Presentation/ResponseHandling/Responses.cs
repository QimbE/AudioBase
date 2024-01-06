namespace Presentation.ResponseHandling;

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

    public ResponseWithData(T data)
        : base()
    {
        Data = data;
    }
}