// Prototyping extended expression trees for C#.
//
// bartde - January 2022

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    // NB: The tests cross-check the outcome of evaluating a lambda expression - specified as a string in the
    //     test cases - in two ways. First, by converting the lambda expression to a delegate type and running
    //     IL code produced by the compiler. Second, by converting the lambda expression to an expression tree
    //     using the extended expression tree support in our modified Roslyn build and compiling the expression
    //     at runtime (therefore invoking our Reduce methods).
    //
    //     It is assumed that the outcome has proper equality defined (i.e. EqualityComparer<T>.Default should
    //     return a meaningful equality comparer to assert evaluation outcomes against each other). If the
    //     evaluation results in an exception, its type is cross-checked.
    //
    //     In addition to cross-checking the evaluation outcome, a log is maintained and cross-checked, which
    //     is useful to assert the order of side-effects. The code fragments can write to this log by means of
    //     the Log method and the Return method (to prepend returning a value of type T with a logging side-
    //     effect).

    partial class CompilerTests
    {
        [TestMethod]
        public void CrossCheck_InterpolatedStringHandlers_Basic_LiteralOnly()
        {
            var f = Compile<Func<string>>("() => { BasicInterpolatedStringHandler h = $\"Hello\"; return h.GetFormattedText(); }", typeof(BasicInterpolatedStringHandler).Assembly);
            f();
        }

        [TestMethod]
        public void CrossCheck_InterpolatedStringHandlers_Basic_OneValue()
        {
            var f = Compile<Func<int, string>>("x => { BasicInterpolatedStringHandler h = $\"x = {Return(x)}\"; return h.GetFormattedText(); }", typeof(BasicInterpolatedStringHandler).Assembly);
            f(42);
        }

        [TestMethod]
        public void CrossCheck_InterpolatedStringHandlers_Basic_OneValue_Alignment()
        {
            var f = Compile<Func<int, string>>("x => { BasicInterpolatedStringHandler h = $\"x = {Return(x),5}\"; return h.GetFormattedText(); }", typeof(BasicInterpolatedStringHandler).Assembly);
            f(42);
        }

        [TestMethod]
        public void CrossCheck_InterpolatedStringHandlers_Basic_OneValue_Format()
        {
            var f = Compile<Func<int, string>>("x => { BasicInterpolatedStringHandler h = $\"x = {Return(x):X}\"; return h.GetFormattedText(); }", typeof(BasicInterpolatedStringHandler).Assembly);
            f(42);
        }

        [TestMethod]
        public void CrossCheck_InterpolatedStringHandlers_Basic_OneValue_Alignment_Format()
        {
            var f = Compile<Func<int, string>>("x => { BasicInterpolatedStringHandler h = $\"x = {Return(x),5:X}\"; return h.GetFormattedText(); }", typeof(BasicInterpolatedStringHandler).Assembly);
            f(42);
        }

        [TestMethod]
        public void CrossCheck_InterpolatedStringHandlers_Basic_ManyValues()
        {
            var f = Compile<Func<int, bool, string, string>>("(x, b, s) => { BasicInterpolatedStringHandler h = $\"x = {Return(x)}; b = {Return(b)}; s = {Return(s)}\"; return h.GetFormattedText(); }", typeof(BasicInterpolatedStringHandler).Assembly);
            f(42, true, "bar");
        }

        [TestMethod]
        public void CrossCheck_InterpolatedStringHandlers_CtorOutBool()
        {
            var f0 = Compile<Func<string>>("() => { InterpolatedStringHandlerCtorOutBool h = $\"Hello\"; return h.GetFormattedText(); }", typeof(InterpolatedStringHandlerCtorOutBool).Assembly);
            f0();

            var f1 = Compile<Func<int, string>>("x => { InterpolatedStringHandlerCtorOutBool h = $\"x = {Return(x)}\"; return h.GetFormattedText(); }", typeof(InterpolatedStringHandlerCtorOutBool).Assembly);
            f1(42);

            var f2 = Compile<Func<int, double, string>>("(x, y) => { InterpolatedStringHandlerCtorOutBool h = $\"x = {Return(x)}, y = {Return(y)}\"; return h.GetFormattedText(); }", typeof(InterpolatedStringHandlerCtorOutBool).Assembly);
            f2(42, Math.PI);
        }

        [TestMethod]
        public void CrossCheck_InterpolatedStringHandlers_AppendReturnBool_AppendLiteral()
        {
            var f0 = Compile<Func<string>>("() => { InterpolatedStringHandlerAppendReturnBool h = $\"Hello\"; return h.GetFormattedText(); }", typeof(InterpolatedStringHandlerAppendReturnBool).Assembly);
            f0();

            var f1 = Compile<Func<string>>("() => { InterpolatedStringHandlerAppendReturnBool h = $\"STOP\"; return h.GetFormattedText(); }", typeof(InterpolatedStringHandlerAppendReturnBool).Assembly);
            f1();

            var f2 = Compile<Func<int, string>>("x => { InterpolatedStringHandlerAppendReturnBool h = $\"Hello{Return(x)}\"; return h.GetFormattedText(); }", typeof(InterpolatedStringHandlerAppendReturnBool).Assembly);
            f2(0);

            var f3 = Compile<Func<int, string>>("x => { InterpolatedStringHandlerAppendReturnBool h = $\"STOP{Return(1 / x)}\"; return h.GetFormattedText(); }", typeof(InterpolatedStringHandlerAppendReturnBool).Assembly);
            f3(0);

            var f4 = Compile<Func<int, string>>("x => { InterpolatedStringHandlerAppendReturnBool h = $\"Hello{Return(x)}STOP{Return(1 / x)}\"; return h.GetFormattedText(); }", typeof(InterpolatedStringHandlerAppendReturnBool).Assembly);
            f4(0);
        }

        [TestMethod]
        public void CrossCheck_InterpolatedStringHandlers_AppendReturnBool_AppendFormatted()
        {
            var f1 = Compile<Func<object, string>>("o => { InterpolatedStringHandlerAppendReturnBool h = $\"o = {Return(o)}\"; return h.GetFormattedText(); }", typeof(InterpolatedStringHandlerAppendReturnBool).Assembly);
            f1(0);
            f1(null);

            var f2 = Compile<Func<object, int, string>>("(o, x) => { InterpolatedStringHandlerAppendReturnBool h = $\"o = {Return(o)}, x = {Return(1 / x)}\"; return h.GetFormattedText(); }", typeof(InterpolatedStringHandlerAppendReturnBool).Assembly);
            f2(null, 0);

            var f3 = Compile<Func<object, int, string>>("(o, x) => { InterpolatedStringHandlerAppendReturnBool h = $\"o = {Return(o),-1}, x = {Return(1 / x)}\"; return h.GetFormattedText(); }", typeof(InterpolatedStringHandlerAppendReturnBool).Assembly);
            f3("foo", 0);

            var f4 = Compile<Func<object, int, string>>("(o, x) => { InterpolatedStringHandlerAppendReturnBool h = $\"o = {Return(o):STOP}, x = {Return(1 / x)}\"; return h.GetFormattedText(); }", typeof(InterpolatedStringHandlerAppendReturnBool).Assembly);
            f4("foo", 0);

            var f5 = Compile<Func<object, int, string>>("(o, x) => { InterpolatedStringHandlerAppendReturnBool h = $\"o = {Return(o),-1:STOP}, x = {Return(1 / x)}\"; return h.GetFormattedText(); }", typeof(InterpolatedStringHandlerAppendReturnBool).Assembly);
            f5("foo", 0);
        }

        [TestMethod]
        public void CrossCheck_InterpolatedStringHandlers_Call()
        {
            var f = Compile<Func<int, bool, string, string>>("(x, b, s) => new Printer(t => Log(t)).Print(123, $\"x = {Return(x)}; b = {Return(b)}; s = {Return(s)}\")", typeof(Printer).Assembly);
            f(42, true, "bar");
        }
    }
}

[InterpolatedStringHandler]
public struct BasicInterpolatedStringHandler
{
    private readonly StringBuilder builder;
    private readonly List<string> log;

    public BasicInterpolatedStringHandler(int literalLength, int formattedCount)
    {
        builder = new StringBuilder(literalLength);

        log = new List<string>();
        Log($".ctor({literalLength}, {formattedCount})");
    }

    public void AppendLiteral(string s)
    {
        Log($"AppendLiteral({s})");
        builder.Append(s);
    }

    public void AppendFormatted<T>(T t)
    {
        Log($"AppendFormatted({t})");
        builder.Append(t?.ToString());
    }

    public void AppendFormatted<T>(T t, int alignment)
    {
        Log($"AppendFormatted({t}, {alignment})");
        builder.AppendFormat($"{{0,{alignment.ToString(CultureInfo.InvariantCulture)}}}", t);
    }

    public void AppendFormatted<T>(T t, string format)
    {
        Log($"AppendFormatted({t}, {format})");
        builder.AppendFormat($"{{0:{format}}}", t);
    }

    public void AppendFormatted<T>(T t, int alignment, string format)
    {
        Log($"AppendFormatted({t}, {alignment}, {format})");
        builder.AppendFormat($"{{0,{alignment.ToString(CultureInfo.InvariantCulture)}:{format}}}", t);
    }

    public string GetFormattedText() => builder.ToString() + " (" + string.Join("; ", log) + ")";

    private void Log(string s) => log.Add(s);
}

[InterpolatedStringHandler]
public struct InterpolatedStringHandlerCtorOutBool
{
    private readonly StringBuilder builder;
    private readonly List<string> log;

    public InterpolatedStringHandlerCtorOutBool(int literalLength, int formattedCount, out bool shouldAppend)
    {
        shouldAppend = formattedCount % 2 == 0;

        builder = new StringBuilder(literalLength);
        log = new List<string>();
        Log($".ctor({literalLength}, {formattedCount}, out {shouldAppend})");
    }

    public void AppendLiteral(string s)
    {
        Log($"AppendLiteral({s})");
        builder.Append(s);
    }

    public void AppendFormatted<T>(T t)
    {
        Log($"AppendFormatted({t})");
        builder.Append(t?.ToString());
    }

    public void AppendFormatted<T>(T t, int alignment)
    {
        Log($"AppendFormatted({t}, {alignment})");
        builder.AppendFormat($"{{0,{alignment.ToString(CultureInfo.InvariantCulture)}}}", t);
    }

    public void AppendFormatted<T>(T t, string format)
    {
        Log($"AppendFormatted({t}, {format})");
        builder.AppendFormat($"{{0:{format}}}", t);
    }

    public void AppendFormatted<T>(T t, int alignment, string format)
    {
        Log($"AppendFormatted({t}, {alignment}, {format})");
        builder.AppendFormat($"{{0,{alignment.ToString(CultureInfo.InvariantCulture)}:{format}}}", t);
    }

    public string GetFormattedText() => builder.ToString() + " (" + string.Join("; ", log) + ")";

    private void Log(string s) => log.Add(s);
}

[InterpolatedStringHandler]
public struct InterpolatedStringHandlerAppendReturnBool
{
    private readonly StringBuilder builder;
    private readonly List<string> log;

    public InterpolatedStringHandlerAppendReturnBool(int literalLength, int formattedCount)
    {
        builder = new StringBuilder(literalLength);
        log = new List<string>();
        Log($".ctor({literalLength}, {formattedCount})");
    }

    public bool AppendLiteral(string s)
    {
        Log($"AppendLiteral({s})");
        builder.Append(s);
        return s != "STOP";
    }

    public bool AppendFormatted<T>(T t)
    {
        Log($"AppendFormatted({t})");
        builder.Append(t?.ToString());
        return t != null;
    }

    public bool AppendFormatted<T>(T t, int alignment)
    {
        Log($"AppendFormatted({t}, {alignment})");
        builder.AppendFormat($"{{0,{alignment.ToString(CultureInfo.InvariantCulture)}}}", t);
        return t != null && alignment != -1;
    }

    public bool AppendFormatted<T>(T t, string format)
    {
        Log($"AppendFormatted({t}, {format})");
        builder.AppendFormat($"{{0:{format}}}", t);
        return t != null && format != "STOP";
    }

    public bool AppendFormatted<T>(T t, int alignment, string format)
    {
        Log($"AppendFormatted({t}, {alignment}, {format})");
        builder.AppendFormat($"{{0,{alignment.ToString(CultureInfo.InvariantCulture)}:{format}}}", t);
        return t != null && alignment != -1 && format != "STOP";
    }

    public string GetFormattedText() => builder.ToString() + " (" + string.Join("; ", log) + ")";

    private void Log(string s) => log.Add(s);
}

public class Printer
{
    internal readonly Action<string> _log;

    public Printer(Action<string> log) => _log = log;

    public string Print(int x, [InterpolatedStringHandlerArgument("", "x")] PrinterInterpolatedStringHandler builder)
    {
        _log($"Print({x})");
        return builder.GetFormattedText();
    }
}

[InterpolatedStringHandler]
public struct PrinterInterpolatedStringHandler
{
    private readonly Action<string> _log;
    private readonly StringBuilder _builder;

    public PrinterInterpolatedStringHandler(int literalLength, int formattedCount, Printer printer, int x)
    {
        _log = printer._log;
        _builder = new StringBuilder(literalLength);
        _log($".ctor({literalLength}, {formattedCount}, {printer}, {x})");
    }

    public void AppendLiteral(string s)
    {
        _log($"AppendLiteral({s})");
        _builder.Append(s);
    }

    public void AppendFormatted<T>(T t)
    {
        _log($"AppendFormatted({t})");
        _builder.Append(t?.ToString());
    }

    internal string GetFormattedText() => _builder.ToString();
}
