using System.Net;

namespace VideoProjectCore6.Utilities.ErrorHandling.Exceptions;

public class HttpStatusException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorCode { get; }
    public HttpStatusException(string msg, string errorCode, HttpStatusCode statusCode) : base(msg)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}