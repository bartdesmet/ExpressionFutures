// Prototyping extended expression trees for C#.
//
// bartde - September 2022

using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace Microsoft.CSharp.Expressions
{
    internal static class WellKnownMembers
    {
        private static MethodInfo? s_floatIsNaN, s_doubleIsNaN;
        public static MethodInfo FloatIsNaN => s_floatIsNaN ??= typeof(float).GetMethod(nameof(float.IsNaN), new[] { typeof(float) })!;
        public static MethodInfo DoubleIsNaN => s_doubleIsNaN ??= typeof(double).GetMethod(nameof(double.IsNaN), new[] { typeof(double) })!;

        private static MethodInfo? s_getSubArray, s_withByRef, s_preAssignByRef, s_postAssignByRef;
        public static MethodInfo GetSubArray => s_getSubArray ??= typeof(RuntimeOpsEx).GetMethod(nameof(RuntimeOpsEx.GetSubArray), BindingFlags.Public | BindingFlags.Static)!;
        public static MethodInfo WithByRef => s_withByRef ??= typeof(RuntimeOpsEx).GetMethod(nameof(RuntimeOpsEx.WithByRef))!;
        public static MethodInfo PreAssignByRef => s_preAssignByRef ??= typeof(RuntimeOpsEx).GetMethod(nameof(RuntimeOpsEx.PreAssignByRef))!;
        public static MethodInfo PostAssignByRef => s_postAssignByRef ??= typeof(RuntimeOpsEx).GetMethod(nameof(RuntimeOpsEx.PostAssignByRef))!;

        private static MethodInfo? s_concat_string_string, s_concat_string_object;
        public static MethodInfo ConcatStringString => s_concat_string_string ??= typeof(string).GetMethod(nameof(string.Concat), new[] { typeof(string), typeof(string) })!;
        public static MethodInfo ConcatStringObject => s_concat_string_object ??= typeof(string).GetMethod(nameof(string.Concat), new[] { typeof(string), typeof(object) })!;

        private static MethodInfo? s_delegate_combine, s_delegate_remove;
        public static MethodInfo DelegateCombine => s_delegate_combine ??= typeof(Delegate).GetMethod(nameof(Delegate.Combine), new[] { typeof(Delegate), typeof(Delegate) })!;
        public static MethodInfo DelegateRemove => s_delegate_remove ??= typeof(Delegate).GetMethod(nameof(Delegate.Remove), new[] { typeof(Delegate), typeof(Delegate) })!;

        private static MethodInfo? s_ediCapture, s_ediThrow;
        public static MethodInfo ExceptionDispatchInfoCapture => s_ediCapture ??= typeof(ExceptionDispatchInfo).GetMethod(nameof(ExceptionDispatchInfo.Capture), BindingFlags.Public | BindingFlags.Static)!;
        public static MethodInfo ExceptionDispatchInfoThrow => s_ediThrow ??= typeof(ExceptionDispatchInfo).GetMethod(nameof(ExceptionDispatchInfo.Throw), BindingFlags.Public | BindingFlags.Instance)!;

        private static MethodInfo? s_IEnumerable_GetEnumerator;
        public static MethodInfo IEnumerable_GetEnumerator => s_IEnumerable_GetEnumerator ??= typeof(IEnumerable).GetMethod(nameof(IEnumerable.GetEnumerator))!;

        private static MethodInfo? s_IEnumerator_MoveNext;
        public static MethodInfo IEnumerator_MoveNext => s_IEnumerator_MoveNext ??= typeof(IEnumerator).GetMethod(nameof(IEnumerator.MoveNext))!;

        private static PropertyInfo? s_IEnumerator_Current;
        public static PropertyInfo IEnumerator_Current => s_IEnumerator_Current ??= typeof(IEnumerator).GetProperty(nameof(IEnumerator.Current))!;

        private static MethodInfo? s_array_getUpperBound, s_array_getLowerBound;
        public static MethodInfo ArrayGetUpperBound => s_array_getUpperBound ??= typeof(Array).GetMethod(nameof(Array.GetUpperBound))!;
        public static MethodInfo ArrayGetLowerBound => s_array_getLowerBound ??= typeof(Array).GetMethod(nameof(Array.GetLowerBound))!;

        private static PropertyInfo? s_string_chars;
        public static PropertyInfo StringChars => s_string_chars ??= typeof(string).GetProperty("Chars")!;

        private static MethodInfo? s_string_getEnumerator;
        public static MethodInfo StringGetEnumerator => s_string_getEnumerator ??= typeof(string).GetMethod(nameof(string.GetEnumerator))!;

        private static MethodInfo? s_index_from_end, s_index_getOffset;
        public static MethodInfo IndexFromEnd => s_index_from_end ??= typeof(Index).GetNonGenericMethod(nameof(Index.FromEnd), BindingFlags.Public | BindingFlags.Static, new[] { typeof(int) })!;
        public static MethodInfo IndexGetOffset => s_index_getOffset ??= typeof(Index).GetNonGenericMethod(nameof(Index.GetOffset), BindingFlags.Public | BindingFlags.Instance, new[] { typeof(int) })!;

        private static PropertyInfo? s_range_start, s_range_end;
        private static MethodInfo? s_range_all, s_range_end_at, s_range_start_at;
        private static ConstructorInfo? s_range_ctor;
        public static PropertyInfo RangeStart => s_range_start ??= typeof(Range).GetProperty(nameof(Range.Start), BindingFlags.Public | BindingFlags.Instance)!;
        public static PropertyInfo RangeEnd => s_range_end ??= typeof(Range).GetProperty(nameof(Range.End), BindingFlags.Public | BindingFlags.Instance)!;
        public static MethodInfo RangeAll => s_range_all ??= typeof(Range).GetProperty(nameof(Range.All))!.GetGetMethod()!;
        public static MethodInfo RangeEndAt => s_range_end_at ??= typeof(Range).GetNonGenericMethod(nameof(Range.EndAt), BindingFlags.Public | BindingFlags.Static, new[] { typeof(Index) })!;
        public static MethodInfo RangeStartAt => s_range_start_at ??= typeof(Range).GetNonGenericMethod(nameof(Range.StartAt), BindingFlags.Public | BindingFlags.Static, new[] { typeof(Index) })!;
        public static ConstructorInfo RangeCtor => s_range_ctor ??= typeof(Range).GetConstructor(new[] { typeof(Index), typeof(Index) })!;

        private static MethodInfo? s_monitor_enter, s_monitor_exit;
        public static MethodInfo MonitorEnter => s_monitor_enter ??= typeof(Monitor).GetMethod(nameof(Monitor.Enter), new[] { typeof(object), typeof(bool).MakeByRefType() })!;
        public static MethodInfo MonitorExit => s_monitor_exit ??= typeof(Monitor).GetMethod(nameof(Monitor.Exit), new[] { typeof(object) })!;

        private static MethodInfo? s_itupleGetLength, s_itupleGetItem;
        public static MethodInfo ITupleGetLength => s_itupleGetLength ??= typeof(ITuple).GetProperty(nameof(ITuple.Length))!.GetGetMethod()!;
        public static MethodInfo ITupleGetItem => s_itupleGetItem ??= typeof(ITuple).GetProperty("Item")!.GetGetMethod()!;

        private static MethodInfo? s_formattableStringFactoryCreate;
        public static MethodInfo FormattableStringFactoryCreate => s_formattableStringFactoryCreate ??= typeof(FormattableStringFactory).GetNonGenericMethod(nameof(FormattableStringFactory.Create), BindingFlags.Public | BindingFlags.Static, new[] { typeof(string), typeof(object[]) })!;

        private static ConstructorInfo? s_switchExpressionExceptionCtor;
        public static ConstructorInfo SwitchExpressionExceptionCtor => s_switchExpressionExceptionCtor ??= typeof(SwitchExpressionException).GetConstructor(new[] { typeof(object) })!;

        private static MethodInfo? s_disposeMethod, s_disposeAsyncMethod;
        public static MethodInfo DisposeMethod => s_disposeMethod ??= typeof(IDisposable).GetMethod(nameof(IDisposable.Dispose))!;
        public static MethodInfo DisposeAsyncMethod => s_disposeAsyncMethod ??= typeof(IAsyncDisposable).GetMethod(nameof(IAsyncDisposable.DisposeAsync))!;
    }
}
