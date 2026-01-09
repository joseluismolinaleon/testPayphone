using System.Collections.Immutable;

namespace CustomerValidationSystem.SharedKernel.Errors;

public record Error(string Code, string Message)
{

    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "Un valor Null fue ingresado");

    public string? Code { get; protected set; } = Code;
    public string? Message { get; protected set; } = Message;
    public ImmutableArray<object> Parameters { get; protected set; } = [];

    public Error WithParameters(params object[] parameters)
    {
        this.Parameters = [.. parameters];
        return this;
    }
}
