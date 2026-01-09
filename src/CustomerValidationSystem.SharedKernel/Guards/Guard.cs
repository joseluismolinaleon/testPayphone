using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CustomerValidationSystem.SharedKernel.Guards;

public static class Guard
{
    public static void ThrowIfNull([NotNull] object? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value is null)
        {
            ArgumentNullException.ThrowIfNull(value, paramName);
        }
    }

    public static void ThrowIfNegativeOrZero([NotNull] decimal value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, paramName);
    }

    public static void ThrowIfNegative([NotNull] decimal value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value, paramName);
    }

    public static void ThrowIfNullOrWhiteSpace([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, paramName);
    }

    public static void ThrowIfMonthIsInvalid(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value, 1, paramName);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 12, paramName);
    }

    public static void ThrowIfYearIsInvalid(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, paramName);
        ArgumentOutOfRangeException.ThrowIfLessThan(value, 1, paramName);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 99, paramName);
    }

    public static void ThrowIfNullOrEmpty<T>(IEnumerable<T> value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (!value.Any())
        {
            throw new ArgumentException("Sequence contains no elements", paramName);
        }
    }
}
