namespace GlobalUtility.Manager.Exceptions;
public class RepositoryException : ArgumentException {
	public RepositoryException() : base() { }
	public RepositoryException(string? message) : base(message) { }
	public RepositoryException(string? message, Exception? innerException) : base(message, innerException) { }
	public RepositoryException(string? message, string? paramName) : base(message, paramName) { }
	public RepositoryException(string? message, string? paramName, Exception? innerException) : base(message, paramName, innerException) { }
}