using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace DataModels.Extensions;

/// <summary>
/// Extension methods for numeric validation with proper attribute usage.
/// </summary>
public static class NumericValidationExtensions
{
    /// <summary>
    /// Throws <see cref="ArgumentException"/> if <paramref name="value"/> is not greater than <paramref name="other"/>.
    /// </summary>
    public static void RequiredGreaterThan<T>(
        this T value,
        T other,
        [CallerArgumentExpression(nameof(value))]
        string? paramName = null,
        string? message = null)
        where T : INumber<T>
    {
        if (value > other) return;

        ThrowArgumentError(
            paramName,
            message ?? $"Value must be greater than {other}",
            value,
            other,
            ">");
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> if <paramref name="value"/> is not greater than or equal to <paramref name="other"/>.
    /// </summary>
    public static void RequiredGreaterThanOrEqual<T>(
        this T value,
        T other,
        [CallerArgumentExpression(nameof(value))]
        string? paramName = null,
        string? message = null)
        where T : INumber<T>
    {
        if (value >= other) return;

        ThrowArgumentError(
            paramName,
            message ?? $"Value must be greater than or equal to {other}",
            value,
            other,
            ">=");
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> if <paramref name="value"/> is not less than <paramref name="other"/>.
    /// </summary>
    public static void RequiredLessThan<T>(
        this T value,
        T other,
        [CallerArgumentExpression(nameof(value))]
        string? paramName = null,
        string? message = null)
        where T : INumber<T>
    {
        if (value < other) return;

        ThrowArgumentError(
            paramName,
            message ?? $"Value must be less than {other}",
            value,
            other,
            "<");
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> if <paramref name="value"/> is not less than or equal to <paramref name="other"/>.
    /// </summary>
    public static void RequiredLessThanOrEqual<T>(
        this T value,
        T other,
        [CallerArgumentExpression(nameof(value))]
        string? paramName = null,
        string? message = null)
        where T : INumber<T>
    {
        if (value <= other) return;

        ThrowArgumentError(
            paramName,
            message ?? $"Value must be less than or equal to {other}",
            value,
            other,
            "<=");
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> if <paramref name="value"/> is outside the range [<paramref name="min"/>, <paramref name="max"/>].
    /// </summary>
    public static void RequiredInRange<T>(
        this T value,
        T min,
        T max,
        [CallerArgumentExpression(nameof(value))]
        string? paramName = null,
        string? message = null)
        where T : INumber<T>
    {
        if (value >= min && value <= max) return;

        throw new ArgumentException(
            message ?? $"Value {value} must be in range [{min}..{max}]",
            paramName);
    }

    [DoesNotReturn]
    private static void ThrowArgumentError<T>(
        string? paramName,
        string message,
        T value,
        T threshold,
        string comparisonOperator)
        where T : INumber<T>
    {
        throw new ArgumentException(
            $"{message} (actual: {value} {comparisonOperator} {threshold})",
            paramName);
    }
}