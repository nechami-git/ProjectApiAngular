using System;

namespace server.Exceptions
{
    // 404
    public class NotFoundException : Exception
    {
        public NotFoundException() { }
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string message, Exception inner) : base(message, inner) { }
    }

    // 400 - business / validation
    public class BusinessException : Exception
    {
        public BusinessException() { }
        public BusinessException(string message) : base(message) { }
        public BusinessException(string message, Exception inner) : base(message, inner) { }
    }

    // 409 - conflict (e.g., duplicate)
    public class ConflictException : Exception
    {
        public ConflictException() { }
        public ConflictException(string message) : base(message) { }
        public ConflictException(string message, Exception inner) : base(message, inner) { }
    }

    // 401
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException() { }
        public UnauthorizedException(string message) : base(message) { }
        public UnauthorizedException(string message, Exception inner) : base(message, inner) { }
    }

    // 500 - internal server
    public class InternalServerException : Exception
    {
        public InternalServerException() { }
        public InternalServerException(string message) : base(message) { }
        public InternalServerException(string message, Exception inner) : base(message, inner) { }
    }
}
