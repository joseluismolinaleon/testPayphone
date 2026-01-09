namespace CustomerValidationSystem.SharedKernel.Errors;

public record NotFoundError(string Code, string Message) : Error(Code, Message)
{
}
