// Prototyping extended expression trees for C#.
//
// bartde - January 2022

#nullable enable

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <param name="handler">The handler to append to.</param>
    /// <param name="value">The literal to append.</param>
    public delegate void AppendLiteral<THandler>(ref THandler handler, string value);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <param name="handler">The handler to append to.</param>
    /// <param name="value">The literal to append.</param>
    /// <returns>true if append should continue; otherwise, false.</returns>
    public delegate bool TryAppendLiteral<THandler>(ref THandler handler, string value);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TValue">The type of the value to append.</typeparam>
    /// <param name="handler">The handler to append to.</param>
    /// <param name="value">The value to append.</param>
    public delegate void AppendFormatted<THandler, TValue>(ref THandler handler, TValue value);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TValue">The type of the value to append.</typeparam>
    /// <param name="handler">The handler to append to.</param>
    /// <param name="value">The value to append.</param>
    /// <returns>true if append should continue; otherwise, false.</returns>
    public delegate bool TryAppendFormatted<THandler, TValue>(ref THandler handler, TValue value);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TValue">The type of the value to append.</typeparam>
    /// <param name="handler">The handler to append to.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="alignment">The alignment used for the formatting.</param>
    public delegate void AppendFormattedAlignment<THandler, TValue>(ref THandler handler, TValue value, int alignment);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TValue">The type of the value to append.</typeparam>
    /// <param name="handler">The handler to append to.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="alignment">The alignment used for the formatting.</param>
    /// <returns>true if append should continue; otherwise, false.</returns>
    public delegate bool TryAppendFormattedAlignment<THandler, TValue>(ref THandler handler, TValue value, int alignment);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TValue">The type of the value to append.</typeparam>
    /// <param name="handler">The handler to append to.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="format">The format string used for the formatting.</param>
    public delegate void AppendFormattedFormat<THandler, TValue>(ref THandler handler, TValue value, string format);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TValue">The type of the value to append.</typeparam>
    /// <param name="handler">The handler to append to.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="format">The format string used for the formatting.</param>
    /// <returns>true if append should continue; otherwise, false.</returns>
    public delegate bool TryAppendFormattedFormat<THandler, TValue>(ref THandler handler, TValue value, string format);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TValue">The type of the value to append.</typeparam>
    /// <param name="handler">The handler to append to.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="alignment">The alignment used for the formatting.</param>
    /// <param name="format">The format string used for the formatting.</param>
    public delegate void AppendFormattedAlignmentFormat<THandler, TValue>(ref THandler handler, TValue value, int alignment, string format);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TValue">The type of the value to append.</typeparam>
    /// <param name="handler">The handler to append to.</param>
    /// <param name="value">The value to append.</param>
    /// <param name="alignment">The alignment used for the formatting.</param>
    /// <param name="format">The format string used for the formatting.</param>
    /// <returns>true if append should continue; otherwise, false.</returns>
    public delegate bool TryAppendFormattedAlignmentFormat<THandler, TValue>(ref THandler handler, TValue value, int alignment, string format);
}
