namespace Presentation;

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
public class ResponseWithData : BaseResponse
{
    public object Data { get; protected set; }

    public ResponseWithData(object data)
        : base()
    {
        Data = data;
    }
}