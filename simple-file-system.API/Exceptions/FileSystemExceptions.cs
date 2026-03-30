using System;
using System.Net;

namespace simple_file_system.API.Exceptions;

public abstract class AppException(HttpStatusCode statusCode, string title, string detail)
    : Exception(detail)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
    public string Title { get; } = title;
    public string Detail { get; } = detail;
}
public class ValidationException(string detail)
    : AppException(HttpStatusCode.BadRequest, "Validation Error", detail);

public class NotFoundException(string detail)
    : AppException(HttpStatusCode.NotFound, "Not Found", detail);

public class InvalidOperationException(string detail)
    : AppException(HttpStatusCode.UnprocessableEntity, "Invalid Operation", detail);

public class ConflictException(string detail)
    : AppException(HttpStatusCode.Conflict, "Conflict", detail);

    
