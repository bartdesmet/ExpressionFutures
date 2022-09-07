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
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandler<THandler>(int literalLength, int formattedCount);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandler<THandler, TArg1>(int literalLength, int formattedCount, TArg1 arg1);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandler<THandler, TArg1, TArg2>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandler<THandler, TArg1, TArg2, TArg3>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandler<THandler, TArg1, TArg2, TArg3, TArg4>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandler<THandler, TArg1, TArg2, TArg3, TArg4, TArg5>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="arg6">The sixth argument to pass to construct the handler.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandler<THandler, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="arg6">The sixth argument to pass to construct the handler.</param>
    /// <param name="arg7">The seventh argument to pass to construct the handler.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandler<THandler, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="arg6">The sixth argument to pass to construct the handler.</param>
    /// <param name="arg7">The seventh argument to pass to construct the handler.</param>
    /// <param name="arg8">The eighth argument to pass to construct the handler.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandler<THandler, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg9">The type of the ninth argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="arg6">The sixth argument to pass to construct the handler.</param>
    /// <param name="arg7">The seventh argument to pass to construct the handler.</param>
    /// <param name="arg8">The eighth argument to pass to construct the handler.</param>
    /// <param name="arg9">The ninth argument to pass to construct the handler.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandler<THandler, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg9">The type of the ninth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg10">The type of the tenth argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="arg6">The sixth argument to pass to construct the handler.</param>
    /// <param name="arg7">The seventh argument to pass to construct the handler.</param>
    /// <param name="arg8">The eighth argument to pass to construct the handler.</param>
    /// <param name="arg9">The ninth argument to pass to construct the handler.</param>
    /// <param name="arg10">The tenth argument to pass to construct the handler.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandler<THandler, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg9">The type of the ninth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg10">The type of the tenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg11">The type of the eleventh argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="arg6">The sixth argument to pass to construct the handler.</param>
    /// <param name="arg7">The seventh argument to pass to construct the handler.</param>
    /// <param name="arg8">The eighth argument to pass to construct the handler.</param>
    /// <param name="arg9">The ninth argument to pass to construct the handler.</param>
    /// <param name="arg10">The tenth argument to pass to construct the handler.</param>
    /// <param name="arg11">The eleventh argument to pass to construct the handler.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandler<THandler, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg9">The type of the ninth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg10">The type of the tenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg11">The type of the eleventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg12">The type of the twelfth argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="arg6">The sixth argument to pass to construct the handler.</param>
    /// <param name="arg7">The seventh argument to pass to construct the handler.</param>
    /// <param name="arg8">The eighth argument to pass to construct the handler.</param>
    /// <param name="arg9">The ninth argument to pass to construct the handler.</param>
    /// <param name="arg10">The tenth argument to pass to construct the handler.</param>
    /// <param name="arg11">The eleventh argument to pass to construct the handler.</param>
    /// <param name="arg12">The twelfth argument to pass to construct the handler.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandler<THandler, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg9">The type of the ninth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg10">The type of the tenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg11">The type of the eleventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg12">The type of the twelfth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg13">The type of the thirteenth argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="arg6">The sixth argument to pass to construct the handler.</param>
    /// <param name="arg7">The seventh argument to pass to construct the handler.</param>
    /// <param name="arg8">The eighth argument to pass to construct the handler.</param>
    /// <param name="arg9">The ninth argument to pass to construct the handler.</param>
    /// <param name="arg10">The tenth argument to pass to construct the handler.</param>
    /// <param name="arg11">The eleventh argument to pass to construct the handler.</param>
    /// <param name="arg12">The twelfth argument to pass to construct the handler.</param>
    /// <param name="arg13">The thirteenth argument to pass to construct the handler.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandler<THandler, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg9">The type of the ninth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg10">The type of the tenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg11">The type of the eleventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg12">The type of the twelfth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg13">The type of the thirteenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg14">The type of the fourteenth argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="arg6">The sixth argument to pass to construct the handler.</param>
    /// <param name="arg7">The seventh argument to pass to construct the handler.</param>
    /// <param name="arg8">The eighth argument to pass to construct the handler.</param>
    /// <param name="arg9">The ninth argument to pass to construct the handler.</param>
    /// <param name="arg10">The tenth argument to pass to construct the handler.</param>
    /// <param name="arg11">The eleventh argument to pass to construct the handler.</param>
    /// <param name="arg12">The twelfth argument to pass to construct the handler.</param>
    /// <param name="arg13">The thirteenth argument to pass to construct the handler.</param>
    /// <param name="arg14">The fourteenth argument to pass to construct the handler.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandler<THandler, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg9">The type of the ninth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg10">The type of the tenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg11">The type of the eleventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg12">The type of the twelfth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg13">The type of the thirteenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg14">The type of the fourteenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg15">The type of the fifteenth argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="arg6">The sixth argument to pass to construct the handler.</param>
    /// <param name="arg7">The seventh argument to pass to construct the handler.</param>
    /// <param name="arg8">The eighth argument to pass to construct the handler.</param>
    /// <param name="arg9">The ninth argument to pass to construct the handler.</param>
    /// <param name="arg10">The tenth argument to pass to construct the handler.</param>
    /// <param name="arg11">The eleventh argument to pass to construct the handler.</param>
    /// <param name="arg12">The twelfth argument to pass to construct the handler.</param>
    /// <param name="arg13">The thirteenth argument to pass to construct the handler.</param>
    /// <param name="arg14">The fourteenth argument to pass to construct the handler.</param>
    /// <param name="arg15">The fifteenth argument to pass to construct the handler.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandler<THandler, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg9">The type of the ninth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg10">The type of the tenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg11">The type of the eleventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg12">The type of the twelfth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg13">The type of the thirteenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg14">The type of the fourteenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg15">The type of the fifteenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg16">The type of the sixteenth argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="arg6">The sixth argument to pass to construct the handler.</param>
    /// <param name="arg7">The seventh argument to pass to construct the handler.</param>
    /// <param name="arg8">The eighth argument to pass to construct the handler.</param>
    /// <param name="arg9">The ninth argument to pass to construct the handler.</param>
    /// <param name="arg10">The tenth argument to pass to construct the handler.</param>
    /// <param name="arg11">The eleventh argument to pass to construct the handler.</param>
    /// <param name="arg12">The twelfth argument to pass to construct the handler.</param>
    /// <param name="arg13">The thirteenth argument to pass to construct the handler.</param>
    /// <param name="arg14">The fourteenth argument to pass to construct the handler.</param>
    /// <param name="arg15">The fifteenth argument to pass to construct the handler.</param>
    /// <param name="arg16">The sixteenth argument to pass to construct the handler.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandler<THandler, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15, TArg16 arg16);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="appendShouldProceed">A Boolean value indicating whether subsequent append calls should be made.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandlerWithShouldAppend<THandler>(int literalLength, int formattedCount, out bool appendShouldProceed);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="appendShouldProceed">A Boolean value indicating whether subsequent append calls should be made.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandlerWithShouldAppend<THandler, TArg1>(int literalLength, int formattedCount, TArg1 arg1, out bool appendShouldProceed);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="appendShouldProceed">A Boolean value indicating whether subsequent append calls should be made.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandlerWithShouldAppend<THandler, TArg1, TArg2>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, out bool appendShouldProceed);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="appendShouldProceed">A Boolean value indicating whether subsequent append calls should be made.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandlerWithShouldAppend<THandler, TArg1, TArg2, TArg3>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, out bool appendShouldProceed);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="appendShouldProceed">A Boolean value indicating whether subsequent append calls should be made.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandlerWithShouldAppend<THandler, TArg1, TArg2, TArg3, TArg4>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, out bool appendShouldProceed);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="appendShouldProceed">A Boolean value indicating whether subsequent append calls should be made.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandlerWithShouldAppend<THandler, TArg1, TArg2, TArg3, TArg4, TArg5>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, out bool appendShouldProceed);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="arg6">The sixth argument to pass to construct the handler.</param>
    /// <param name="appendShouldProceed">A Boolean value indicating whether subsequent append calls should be made.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandlerWithShouldAppend<THandler, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, out bool appendShouldProceed);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="arg6">The sixth argument to pass to construct the handler.</param>
    /// <param name="arg7">The seventh argument to pass to construct the handler.</param>
    /// <param name="appendShouldProceed">A Boolean value indicating whether subsequent append calls should be made.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandlerWithShouldAppend<THandler, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, out bool appendShouldProceed);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="arg6">The sixth argument to pass to construct the handler.</param>
    /// <param name="arg7">The seventh argument to pass to construct the handler.</param>
    /// <param name="arg8">The eighth argument to pass to construct the handler.</param>
    /// <param name="appendShouldProceed">A Boolean value indicating whether subsequent append calls should be made.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandlerWithShouldAppend<THandler, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, out bool appendShouldProceed);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg9">The type of the ninth argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="arg6">The sixth argument to pass to construct the handler.</param>
    /// <param name="arg7">The seventh argument to pass to construct the handler.</param>
    /// <param name="arg8">The eighth argument to pass to construct the handler.</param>
    /// <param name="arg9">The ninth argument to pass to construct the handler.</param>
    /// <param name="appendShouldProceed">A Boolean value indicating whether subsequent append calls should be made.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandlerWithShouldAppend<THandler, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, out bool appendShouldProceed);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg9">The type of the ninth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg10">The type of the tenth argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="arg6">The sixth argument to pass to construct the handler.</param>
    /// <param name="arg7">The seventh argument to pass to construct the handler.</param>
    /// <param name="arg8">The eighth argument to pass to construct the handler.</param>
    /// <param name="arg9">The ninth argument to pass to construct the handler.</param>
    /// <param name="arg10">The tenth argument to pass to construct the handler.</param>
    /// <param name="appendShouldProceed">A Boolean value indicating whether subsequent append calls should be made.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandlerWithShouldAppend<THandler, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, out bool appendShouldProceed);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg9">The type of the ninth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg10">The type of the tenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg11">The type of the eleventh argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="arg6">The sixth argument to pass to construct the handler.</param>
    /// <param name="arg7">The seventh argument to pass to construct the handler.</param>
    /// <param name="arg8">The eighth argument to pass to construct the handler.</param>
    /// <param name="arg9">The ninth argument to pass to construct the handler.</param>
    /// <param name="arg10">The tenth argument to pass to construct the handler.</param>
    /// <param name="arg11">The eleventh argument to pass to construct the handler.</param>
    /// <param name="appendShouldProceed">A Boolean value indicating whether subsequent append calls should be made.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandlerWithShouldAppend<THandler, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, out bool appendShouldProceed);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg9">The type of the ninth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg10">The type of the tenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg11">The type of the eleventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg12">The type of the twelfth argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="arg6">The sixth argument to pass to construct the handler.</param>
    /// <param name="arg7">The seventh argument to pass to construct the handler.</param>
    /// <param name="arg8">The eighth argument to pass to construct the handler.</param>
    /// <param name="arg9">The ninth argument to pass to construct the handler.</param>
    /// <param name="arg10">The tenth argument to pass to construct the handler.</param>
    /// <param name="arg11">The eleventh argument to pass to construct the handler.</param>
    /// <param name="arg12">The twelfth argument to pass to construct the handler.</param>
    /// <param name="appendShouldProceed">A Boolean value indicating whether subsequent append calls should be made.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandlerWithShouldAppend<THandler, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, out bool appendShouldProceed);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg9">The type of the ninth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg10">The type of the tenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg11">The type of the eleventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg12">The type of the twelfth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg13">The type of the thirteenth argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="arg6">The sixth argument to pass to construct the handler.</param>
    /// <param name="arg7">The seventh argument to pass to construct the handler.</param>
    /// <param name="arg8">The eighth argument to pass to construct the handler.</param>
    /// <param name="arg9">The ninth argument to pass to construct the handler.</param>
    /// <param name="arg10">The tenth argument to pass to construct the handler.</param>
    /// <param name="arg11">The eleventh argument to pass to construct the handler.</param>
    /// <param name="arg12">The twelfth argument to pass to construct the handler.</param>
    /// <param name="arg13">The thirteenth argument to pass to construct the handler.</param>
    /// <param name="appendShouldProceed">A Boolean value indicating whether subsequent append calls should be made.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandlerWithShouldAppend<THandler, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, out bool appendShouldProceed);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg9">The type of the ninth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg10">The type of the tenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg11">The type of the eleventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg12">The type of the twelfth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg13">The type of the thirteenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg14">The type of the fourteenth argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="arg6">The sixth argument to pass to construct the handler.</param>
    /// <param name="arg7">The seventh argument to pass to construct the handler.</param>
    /// <param name="arg8">The eighth argument to pass to construct the handler.</param>
    /// <param name="arg9">The ninth argument to pass to construct the handler.</param>
    /// <param name="arg10">The tenth argument to pass to construct the handler.</param>
    /// <param name="arg11">The eleventh argument to pass to construct the handler.</param>
    /// <param name="arg12">The twelfth argument to pass to construct the handler.</param>
    /// <param name="arg13">The thirteenth argument to pass to construct the handler.</param>
    /// <param name="arg14">The fourteenth argument to pass to construct the handler.</param>
    /// <param name="appendShouldProceed">A Boolean value indicating whether subsequent append calls should be made.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandlerWithShouldAppend<THandler, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, out bool appendShouldProceed);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg9">The type of the ninth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg10">The type of the tenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg11">The type of the eleventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg12">The type of the twelfth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg13">The type of the thirteenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg14">The type of the fourteenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg15">The type of the fifteenth argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="arg6">The sixth argument to pass to construct the handler.</param>
    /// <param name="arg7">The seventh argument to pass to construct the handler.</param>
    /// <param name="arg8">The eighth argument to pass to construct the handler.</param>
    /// <param name="arg9">The ninth argument to pass to construct the handler.</param>
    /// <param name="arg10">The tenth argument to pass to construct the handler.</param>
    /// <param name="arg11">The eleventh argument to pass to construct the handler.</param>
    /// <param name="arg12">The twelfth argument to pass to construct the handler.</param>
    /// <param name="arg13">The thirteenth argument to pass to construct the handler.</param>
    /// <param name="arg14">The fourteenth argument to pass to construct the handler.</param>
    /// <param name="arg15">The fifteenth argument to pass to construct the handler.</param>
    /// <param name="appendShouldProceed">A Boolean value indicating whether subsequent append calls should be made.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandlerWithShouldAppend<THandler, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15, out bool appendShouldProceed);

    /// <summary>
    /// This API supports the product infrastructure and is not intended to be used directly from your code.
    /// Provides a delegate type used for interpolated string handlers.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg9">The type of the ninth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg10">The type of the tenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg11">The type of the eleventh argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg12">The type of the twelfth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg13">The type of the thirteenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg14">The type of the fourteenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg15">The type of the fifteenth argument to pass to construct the handler.</typeparam>
    /// <typeparam name="TArg16">The type of the sixteenth argument to pass to construct the handler.</typeparam>
    /// <param name="literalLength">The combined length of the string literals in the string interpolations.</param>
    /// <param name="formattedCount">The number of string interpolation holes.</param>
    /// <param name="arg1">The first argument to pass to construct the handler.</param>
    /// <param name="arg2">The second argument to pass to construct the handler.</param>
    /// <param name="arg3">The third argument to pass to construct the handler.</param>
    /// <param name="arg4">The fourth argument to pass to construct the handler.</param>
    /// <param name="arg5">The fifth argument to pass to construct the handler.</param>
    /// <param name="arg6">The sixth argument to pass to construct the handler.</param>
    /// <param name="arg7">The seventh argument to pass to construct the handler.</param>
    /// <param name="arg8">The eighth argument to pass to construct the handler.</param>
    /// <param name="arg9">The ninth argument to pass to construct the handler.</param>
    /// <param name="arg10">The tenth argument to pass to construct the handler.</param>
    /// <param name="arg11">The eleventh argument to pass to construct the handler.</param>
    /// <param name="arg12">The twelfth argument to pass to construct the handler.</param>
    /// <param name="arg13">The thirteenth argument to pass to construct the handler.</param>
    /// <param name="arg14">The fourteenth argument to pass to construct the handler.</param>
    /// <param name="arg15">The fifteenth argument to pass to construct the handler.</param>
    /// <param name="arg16">The sixteenth argument to pass to construct the handler.</param>
    /// <param name="appendShouldProceed">A Boolean value indicating whether subsequent append calls should be made.</param>
    /// <returns>The instance of the handler.</returns>
    public delegate THandler ConstructInterpolatedStringHandlerWithShouldAppend<THandler, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TArg11, TArg12, TArg13, TArg14, TArg15, TArg16>(int literalLength, int formattedCount, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10, TArg11 arg11, TArg12 arg12, TArg13 arg13, TArg14 arg14, TArg15 arg15, TArg16 arg16, out bool appendShouldProceed);

    internal static class ConstructInterpolatedStringHandlerDelegateHelpers
    {
        internal static Type? GetConstructInterpolatedStringHandlerType(Type[] types) =>
            types.Length switch
            {
                1 => typeof(ConstructInterpolatedStringHandler<>).MakeGenericType(types),
                2 => typeof(ConstructInterpolatedStringHandler<,>).MakeGenericType(types),
                3 => typeof(ConstructInterpolatedStringHandler<,,>).MakeGenericType(types),
                4 => typeof(ConstructInterpolatedStringHandler<,,,>).MakeGenericType(types),
                5 => typeof(ConstructInterpolatedStringHandler<,,,,>).MakeGenericType(types),
                6 => typeof(ConstructInterpolatedStringHandler<,,,,,>).MakeGenericType(types),
                7 => typeof(ConstructInterpolatedStringHandler<,,,,,,>).MakeGenericType(types),
                8 => typeof(ConstructInterpolatedStringHandler<,,,,,,,>).MakeGenericType(types),
                9 => typeof(ConstructInterpolatedStringHandler<,,,,,,,,>).MakeGenericType(types),
                10 => typeof(ConstructInterpolatedStringHandler<,,,,,,,,,>).MakeGenericType(types),
                11 => typeof(ConstructInterpolatedStringHandler<,,,,,,,,,,>).MakeGenericType(types),
                12 => typeof(ConstructInterpolatedStringHandler<,,,,,,,,,,,>).MakeGenericType(types),
                13 => typeof(ConstructInterpolatedStringHandler<,,,,,,,,,,,,>).MakeGenericType(types),
                14 => typeof(ConstructInterpolatedStringHandler<,,,,,,,,,,,,,>).MakeGenericType(types),
                15 => typeof(ConstructInterpolatedStringHandler<,,,,,,,,,,,,,,>).MakeGenericType(types),
                16 => typeof(ConstructInterpolatedStringHandler<,,,,,,,,,,,,,,,>).MakeGenericType(types),
                17 => typeof(ConstructInterpolatedStringHandler<,,,,,,,,,,,,,,,,>).MakeGenericType(types),
                _ => null
            };

        internal static Type? GetConstructInterpolatedStringHandlerTypeWithShouldAppend(Type[] types) =>
            types.Length switch
            {
                1 => typeof(ConstructInterpolatedStringHandlerWithShouldAppend<>).MakeGenericType(types),
                2 => typeof(ConstructInterpolatedStringHandlerWithShouldAppend<,>).MakeGenericType(types),
                3 => typeof(ConstructInterpolatedStringHandlerWithShouldAppend<,,>).MakeGenericType(types),
                4 => typeof(ConstructInterpolatedStringHandlerWithShouldAppend<,,,>).MakeGenericType(types),
                5 => typeof(ConstructInterpolatedStringHandlerWithShouldAppend<,,,,>).MakeGenericType(types),
                6 => typeof(ConstructInterpolatedStringHandlerWithShouldAppend<,,,,,>).MakeGenericType(types),
                7 => typeof(ConstructInterpolatedStringHandlerWithShouldAppend<,,,,,,>).MakeGenericType(types),
                8 => typeof(ConstructInterpolatedStringHandlerWithShouldAppend<,,,,,,,>).MakeGenericType(types),
                9 => typeof(ConstructInterpolatedStringHandlerWithShouldAppend<,,,,,,,,>).MakeGenericType(types),
                10 => typeof(ConstructInterpolatedStringHandlerWithShouldAppend<,,,,,,,,,>).MakeGenericType(types),
                11 => typeof(ConstructInterpolatedStringHandlerWithShouldAppend<,,,,,,,,,,>).MakeGenericType(types),
                12 => typeof(ConstructInterpolatedStringHandlerWithShouldAppend<,,,,,,,,,,,>).MakeGenericType(types),
                13 => typeof(ConstructInterpolatedStringHandlerWithShouldAppend<,,,,,,,,,,,,>).MakeGenericType(types),
                14 => typeof(ConstructInterpolatedStringHandlerWithShouldAppend<,,,,,,,,,,,,,>).MakeGenericType(types),
                15 => typeof(ConstructInterpolatedStringHandlerWithShouldAppend<,,,,,,,,,,,,,,>).MakeGenericType(types),
                16 => typeof(ConstructInterpolatedStringHandlerWithShouldAppend<,,,,,,,,,,,,,,,>).MakeGenericType(types),
                17 => typeof(ConstructInterpolatedStringHandlerWithShouldAppend<,,,,,,,,,,,,,,,,>).MakeGenericType(types),
                _ => null
            };
    }
}
