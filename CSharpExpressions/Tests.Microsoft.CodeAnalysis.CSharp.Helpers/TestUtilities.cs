// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using ClrTest.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CSharp.Expressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using CSharpDynamic = Microsoft.CSharp.RuntimeBinder;

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    public static class TestUtilities
    {
        // NB: This domain is used to load custom-built Roslyn assemblies when invoking TestUtilities from the T4
        //     text template when generating tests. The problem is that the T4 engine is loaded in VS, with the Roslyn
        //     binaries on the AppDomain's probing path. If we don't tweak this, it will pick up the v1 RTM binaries
        //     from "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\PrivateAssemblies".
        private static AppDomain s_roslyn;

        public static void InitializeDomain(string path)
        {
            var setup = new AppDomainSetup
            {
                ApplicationBase = path,
            };

            s_roslyn = AppDomain.CreateDomain("RoslynHost", null, setup);
        }

        public static void UnloadDomain()
        {
            if (s_roslyn != null)
            {
                AppDomain.Unload(s_roslyn);
                // NB: not setting to null, so subsequent invocations fail DoCallBack upon misuse
            }
        }

        public static string GetDebugView(string expr, bool reduce = false)
        {
            if (s_roslyn != null)
            {
                return GetDebugViewMarshal(expr, reduce);
            }

            return GetDebugViewCore(expr, reduce);
        }

        private static string GetDebugViewMarshal(string expr, bool reduce)
        {
            s_roslyn.SetData("expr", expr);
            s_roslyn.SetData("reduce", reduce);

            s_roslyn.DoCallBack(GetDebugViewCallback);

            var res = (string)s_roslyn.GetData("debugView");
            return res;
        }

        private static void GetDebugViewCallback()
        {
            var expr = (string)AppDomain.CurrentDomain.GetData("expr");
            var red = AppDomain.CurrentDomain.GetData("reduce");
            var reduce = red != null ? (bool)red : false;
            var res = GetDebugViewCore(expr, reduce);
            AppDomain.CurrentDomain.SetData("debugView", res);
        }

        private static string GetDebugViewCore(string expr, bool reduce)
        {
            var exp = (Expression)Eval(expr);

            var reduced = Reducer.Instance.Visit(exp); // NB: This is used to detect unexpected reduction failures

            var res = exp;
            if (reduce)
            {
                res = reduced;
            }

            return res.DebugView().ToString();
        }

        public static string ToCSharp(string expr, bool reduce = false)
        {
            if (s_roslyn != null)
            {
                return ToCSharpMarshal(expr, reduce);
            }

            return ToCSharpCore(expr, reduce);
        }

        private static string ToCSharpMarshal(string expr, bool reduce)
        {
            s_roslyn.SetData("expr", expr);
            s_roslyn.SetData("reduce", reduce);

            s_roslyn.DoCallBack(ToCSharpCallback);

            var res = (string)s_roslyn.GetData("csharp");
            return res;
        }

        private static void ToCSharpCallback()
        {
            var expr = (string)AppDomain.CurrentDomain.GetData("expr");
            var red = AppDomain.CurrentDomain.GetData("reduce");
            var reduce = red != null ? (bool)red : false;
            var res = ToCSharpCore(expr, reduce);
            AppDomain.CurrentDomain.SetData("csharp", res);
        }

        private static string ToCSharpCore(string expr, bool reduce)
        {
            var exp = (Expression)Eval(expr);

            var reduced = Reducer.Instance.Visit(exp); // NB: This is used to detect unexpected reduction failures

            var res = exp;
            if (reduce)
            {
                res = reduced;
            }

            return res.ToCSharp().ToString();
        }

        public static object Eval(string expr, bool includingExpressions = true)
        {
            return Eval(expr, out _, includingExpressions);
        }

        public static object Eval(string expr, out SemanticModel sem, bool includingExpressions = true, bool trimCR = false)
        {
            // TODO: Investigate using the scripting APIs here instead.

            expr = Indent(expr);

            var typeName = "Expressions";
            var propName = "Expression";

            var exprProp = $"    public static Expression {propName} => {expr};";

            var src = $@"
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public static class {typeName}
{{
{exprProp}
}}
".Trim('\r', '\n');

            var testCode = @"
using System;

public record class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}

public struct Point
{
    public int X { get; set; }
    public int Y { get; set; }
}
";

            if (trimCR)
            {
                src = src.Replace("\r\n", "\n");
            }

            var tree = CSharpSyntaxTree.ParseText(src);

            tree = Format(tree, trimCR);

            var csc = CSharpCompilation
                // A class library `Expressions` which will be emitted in memory
                .Create("Expressions")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, warningLevel: 0))

                // BCL assemblies
                .AddReferences(MetadataReference.CreateFromFile(typeof(int).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(Expression).Assembly.Location))

                // BCL extensions for C# 7
                .AddReferences(new[] { MetadataReference.CreateFromFile(typeof(ValueTuple).Assembly.Location) })

                // BCL extensions for C# 8
                .AddReferences(new[] { MetadataReference.CreateFromFile(typeof(Index).Assembly.Location) })

                // Our custom assembly
                .AddReferences(includingExpressions ? new[] { MetadataReference.CreateFromFile(typeof(CSharpExpression).Assembly.Location) } : Array.Empty<MetadataReference>())

                // Support for dynamic
                .AddReferences(MetadataReference.CreateFromFile(typeof(CSharpDynamic.Binder).Assembly.Location))

                // Helper types
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(testCode))

                // Generated test code based on `expr`
                .AddSyntaxTrees(tree);

            var asm = default(Assembly);

            using (var ms = new MemoryStream())
            {
                var res = default(EmitResult);

                try
                {
                    // NB: This can fail because we're testing custom builds of the compiler.
                    res = csc.Emit(ms);
                    sem = csc.GetSemanticModel(tree, false);
                }
                catch (Exception ex)
                {
                    throw new Exception("Fatal compiler error!", ex); // TODO: add the program text
                }

                if (!res.Success)
                {
                    var diag = string.Join("\r\n", res.Diagnostics.Select(d => d.ToString()));
                    throw new InvalidProgramException(diag);
                }

                ms.Position = 0;

                asm = Assembly.Load(ms.ToArray());
            }

            var typ = asm.GetType(typeName);
            var prp = typ.GetProperty(propName);

            return prp.GetValue(null);
        }

        private static SyntaxTree Format(SyntaxTree tree, bool trimCR)
        {
            try
            {
                var ws = new AdhocWorkspace();
                var root = tree.GetRoot();
                var newRoot = Formatter.Format(root, ws);
                var newTree = tree.WithRootAndOptions(newRoot, tree.Options);

                // NB: Likely we should fix the issue at the level of the RTF textbox instead of jumping
                //     through hoops here.
                if (trimCR)
                {
                    var src = newTree.ToString();
                    src = src.Replace("\r\n", "\n");
                    newTree = CSharpSyntaxTree.ParseText(src);
                }

                return newTree;
            }
            catch (NotSupportedException)
            {
                // NB: This happens when running tests due to missing files in the test runner folder.
                return tree;
            }
            catch (InvalidOperationException)
            {
                // NB: This happens in Debug build due to Format's use of WaitAndGetResult which requires
                //     a UI thread. See src\Workspaces\Core\Portable\Utilities\TaskExtensions.cs in Roslyn.
                return tree;
            }
        }

        private static string Indent(string code)
        {
            var lines = code.Split('\n');

            if (lines.Length == 1)
            {
                return code;
            }

            var sb = new StringBuilder();

            sb.Append(lines[0] + "\n");

            for (var i = 1; i < lines.Length; i++)
            {
                sb.Append("    " + lines[i]);

                if (i != lines.Length - 1)
                {
                    sb.Append("\n");
                }
            }

            return sb.ToString();
        }

        public static string GetMethodIL(this Delegate compiled)
        {
            var sb = new StringBuilder();

            AppendMethod(compiled, sb);

            return sb.ToString();
        }

        private static void AppendMethod(Delegate compiled, StringBuilder sb)
        {
            AppendMethod(compiled.Method, sb);

            if (compiled.Target is Closure closure)
            {
                var objects = new Queue<object>((closure.Locals ?? Array.Empty<object>()).Concat(closure.Constants ?? Array.Empty<object>()));
                while (objects.Count > 0)
                {
                    var o = objects.Dequeue();

                    var m = o as MethodBase;
                    if (m != null)
                    {
                        sb.AppendLine();
                        AppendMethod(m, sb);
                    }

                    if (o is object[] a)
                    {
                        foreach (var c in a)
                        {
                            objects.Enqueue(c);
                        }
                    }
                }
            }
        }

        private static void AppendMethod(MethodBase method, StringBuilder sb)
        {
            var res = MethodBodyInfo.Create(method);

            sb.AppendLine($"{res.MethodToString} // 0X{res.Identity.ToString("X2")}");
            sb.AppendLine("{");
            foreach (var instr in res.Instructions)
            {
                sb.AppendLine("   " + instr);
            }
            sb.AppendLine("}");
        }

        private static void CheckNotNull(object o)
        {
            if (o == null)
                throw new InvalidOperationException("Could not find IL code.");
        }

        public static FuncEval<TDelegate> FuncEval<TDelegate>(string expr)
        {
            // TODO: Investigate using the scripting APIs here instead.

            var typeName = "Expressions";

            var deleName = typeof(TDelegate).ToCSharp("System");

            var exprName = "Expression";
            var funcName = "Function";

            var exprProp = $"public static Expression<{deleName}> {exprName} => {expr};";
            var funcProp = $"public static {deleName} {funcName} => {expr};";

            var src = $@"
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public static class {typeName}
{{
    public static readonly List<string> s_Log = new List<string>();

    public static T Return<T>(T value)
    {{
        s_Log.Add(value?.ToString());
        return value;
    }}

    public static T Return<T>(T value, string message)
    {{
        s_Log.Add($""{{value}} - {{message}}"");
        return value;
    }}

    public static T Log<T>(T value)
    {{
        return Return<T>(value); // just an alias for now
    }}

    public static T Await<T>(Func<Task<T>> f)
    {{
        return f().Result;
    }}

    public static void AwaitVoid(Func<Task> f)
    {{
        f().Wait();
    }}

    {exprProp}
    {funcProp}
}}
";

            var tree = CSharpSyntaxTree.ParseText(src);

            var csc = CSharpCompilation
                // A class library `Expressions` which will be emitted in memory
                .Create("Expressions")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, warningLevel: 0))

                // BCL assemblies
                .AddReferences(MetadataReference.CreateFromFile(typeof(int).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(Expression).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(Index).Assembly.Location))

                // Our custom assembly
                .AddReferences(new[] { MetadataReference.CreateFromFile(typeof(CSharpExpression).Assembly.Location) })

                // Support for dynamic
                .AddReferences(MetadataReference.CreateFromFile(typeof(CSharpDynamic.Binder).Assembly.Location))

                // Test utilities
                .AddReferences(MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location))

                // Generated test code based on `expr`
                .AddSyntaxTrees(tree);

            var asm = default(Assembly);

            using (var ms = new MemoryStream())
            {
                var res = default(EmitResult);

                try
                {
                    // NB: This can fail because we're testing custom builds of the compiler.
                    res = csc.Emit(ms);
                }
                catch (Exception ex)
                {
                    throw new Exception("Fatal compiler error!", ex); // TODO: add the program text
                }

                if (!res.Success)
                {
                    var diag = string.Join("\r\n", res.Diagnostics.Select(d => d.ToString()));
                    throw new InvalidProgramException(diag);
                }

                ms.Position = 0;

                asm = Assembly.Load(ms.ToArray());
            }

            var typ = asm.GetType(typeName);

            var exp = typ.GetProperty(exprName);
            var del = typ.GetProperty(funcName);
            var log = typ.GetField("s_Log");

            return new FuncEval<TDelegate>
            {
                Function = (TDelegate)del.GetValue(null),
                Expression = (Expression<TDelegate>)exp.GetValue(null),
                Log = (List<string>)log.GetValue(null),
            };
        }

        class Reducer : ExpressionVisitor
        {
            public static readonly Reducer Instance = new Reducer();
        }
    }

    public class FuncEval<TDelegate>
    {
        public TDelegate Function;
        public Expression<TDelegate> Expression;
        public List<string> Log;
    }
}
