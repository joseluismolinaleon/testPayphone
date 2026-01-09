namespace CustomerValidationSystem.Application.Common.Exceptions;

/// <summary>
/// Excepción lanzada cuando un recurso solicitado no se encuentra.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
    
    public NotFoundException(string message, Exception innerException) 
        : base(message, innerException) { }
}
