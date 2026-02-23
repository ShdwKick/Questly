using System.Numerics;
using System.Runtime.CompilerServices;

namespace DataModels.Extensions;

/// <summary>
/// Extension methods for reference type validation with proper attribute usage.
/// </summary>
public static class ReferenceTypeValidationExtension
{
    
    public static bool IsNull<T>(this T instance) where T : class => instance == null;
    
    public static bool IsNotNull<T>(this T instance) where T : class => instance != null;
    
    /// <summary>
    /// Throws <see cref="ArgumentException"/> if <paramref name="value"/> is null <paramref name="other"/>.
    /// </summary>
    public static void RequiredNotNull<T>(
        this T value,
        [CallerArgumentExpression(nameof(value))]
        string? paramName = null,
        string? message = null)
        where T :  class{
        
        if (value.IsNotNull())
            return;
        
        throw new ArgumentException(
            $"Value of {paramName} must be not null",
            paramName);
    }
    
    
}