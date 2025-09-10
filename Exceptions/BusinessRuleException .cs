namespace M01.BaselineAPIProjectController.Exceptions;

public class BusinessRuleException : Exception
{
    public int StatusCode { get; }

    public BusinessRuleException(string message, int statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }
}