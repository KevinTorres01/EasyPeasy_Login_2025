namespace EasyPeasy_Login.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class UserNotFoundException : DomainException
{
    public UserNotFoundException(string username)
        : base($"User '{username}' not found") { }
}

public class InvalidCredentialsException : DomainException
{
    public InvalidCredentialsException(string message = "Invalid username or password")
        : base(message){ }
}

public class SessionExpiredException : DomainException
{
    public SessionExpiredException()
        : base("Session has expired") { }
}