namespace CustomerValidationSystem.SharedKernel.Errors;

public record FieldError(string Field, string Code, string Message) : Error(Code, Message);

public record AggregateValidatorError(string Code = nameof(AggregateValidatorError), string Message = "") : Error(Code, Message)
{
    public IReadOnlyList<FieldError> InnerErrors => this._innerErrors;
    private readonly List<FieldError> _innerErrors = [];

    public void AddError(FieldError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        this._innerErrors.Add(error);
    }

    public void AddErrors(IEnumerable<FieldError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        this._innerErrors.AddRange(errors);
    }
}
