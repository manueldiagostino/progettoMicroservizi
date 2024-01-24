namespace GlobalUtility.Manager.Exceptions;
public class BusinessException : ArgumentException {
	public BusinessException() : base() { }
	public BusinessException(string? message) : base(message) { }
	public BusinessException(string? message, Exception? innerException) : base(message, innerException) { }
	public BusinessException(string? message, string? paramName) : base(message, paramName) { }
	public BusinessException(string? message, string? paramName, Exception? innerException) : base(message, paramName, innerException) { }
}