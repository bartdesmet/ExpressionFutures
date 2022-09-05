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
using System.Threading.Tasks;
using CSharpDynamic = Microsoft.CSharp.RuntimeBinder;

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    public static class TestUtilities
    {
#if FALSE
        // NB: This domain is used to load custom-built Roslyn assemblies when invoking TestUtilities from the T4
        //     text template when generating tests. The problem is that the T4 engine is loaded in VS, with the Roslyn
        //     binaries on the AppDomain's probing path. If we don't tweak this, it will pick up the v1 RTM binaries
        //     from "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\PrivateAssemblies".
        private static AppDomain s_roslyn;
#endif

        // TODO: Move the code below to RuntimeTestUtils.cs instead once this solution builds using the latest C#
        //       language version.
        private static string testCode = @"
using System;

public record class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}
";

#if FALSE
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
#endif

        public static string GetDebugView(string expr, bool reduce = false)
        {
#if FALSE
            if (s_roslyn != null)
            {
                return GetDebugViewMarshal(expr, reduce);
            }
#endif
            return GetDebugViewCore(expr, reduce);
        }

#if FALSE
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
#endif

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
#if FALSE
            if (s_roslyn != null)
            {
                return ToCSharpMarshal(expr, reduce);
            }
#endif

            return ToCSharpCore(expr, reduce);
        }

#if FALSE
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
#endif

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

            var asm = Compile(src, out sem, includingExpressions, trimCR);

            var typ = asm.GetType(typeName);
            var prp = typ.GetProperty(propName);

            //if (true)
            //{
            //    throw new InvalidProgramException(string.Join("\r\n", AppDomain.CurrentDomain.GetAssemblies().Select(a =>
            //    {
            //        string loc = "???";
            //        try
            //        {
            //            loc = a.Location;
            //        }
            //        catch { }
            //        return a.FullName + " at " + loc;
            //    })));
            //}

            return prp.GetValue(null);
        }

        public static Assembly Compile(string code, out SemanticModel sem, bool includingExpressions = true, bool trimCR = false, params Assembly[] references)
        {
            if (trimCR)
            {
                code = code.Replace("\r\n", "\n");
            }

            var tree = CSharpSyntaxTree.ParseText(code, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview));

            tree = Format(tree, trimCR);

            var csc = GetCSharpCompilation(includingExpressions, references).AddSyntaxTrees(tree);

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

                //ms.Position = 0;

                //using (var fs = File.Create(@"Expressions.dll"))
                //{
                //    ms.CopyTo(fs);
                //}

                ms.Position = 0;

                return Assembly.Load(ms.ToArray());
            }
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
                    newTree = CSharpSyntaxTree.ParseText(src, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview));
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

#if FALSE
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
#endif
        }

        private static void AppendMethod(MethodBase method, StringBuilder sb)
        {
            var res = MethodBodyInfo.Create(method);

            sb.AppendLine($"{res.MethodToString} // 0X{res.Identity:X2}");
            sb.AppendLine("{");
            foreach (var instr in res.Instructions)
            {
                sb.AppendLine("   " + instr);
            }
            sb.AppendLine("}");
        }

        public static FuncEval<TDelegate> FuncEval<TDelegate>(string expr, params Assembly[] references)
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

            var tree = CSharpSyntaxTree.ParseText(src, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview));

            var csc = GetCSharpCompilation(includingExpressions: true, references)
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

        private static CSharpCompilation GetCSharpCompilation(bool includingExpressions, params Assembly[] references)
        {
            var self = typeof(TestUtilities).Assembly;

            var refs = self.GetReferencedAssemblies().Select(asm => Assembly.Load(asm.FullName)).ToArray();
            references = references.Concat(refs).ToArray();

            return CSharpCompilation
                // A class library `Expressions` which will be emitted in memory
                .Create("Expressions")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, warningLevel: 0))

                // BCL assemblies
                .AddReferences(MetadataReference.CreateFromFile(typeof(int).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(Expression).Assembly.Location))

                // BCL extensions for C# 7
                .AddReferences(MetadataReference.CreateFromFile(typeof(ValueTuple).Assembly.Location))

                // BCL extensions for C# 8
                .AddReferences(MetadataReference.CreateFromFile(typeof(Index).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(IAsyncDisposable).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(ValueTask).Assembly.Location))

                // Support for dynamic
                .AddReferences(MetadataReference.CreateFromFile(typeof(CSharpDynamic.Binder).Assembly.Location))

                // Test utilities
                .AddReferences(MetadataReference.CreateFromFile(self.Location))

                // Our custom assembly
                .AddReferences(includingExpressions ? new[] { MetadataReference.CreateFromFile(typeof(CSharpExpression).Assembly.Location) } : Array.Empty<MetadataReference>())

                // Extra references
                .AddReferences(references.Select(r => MetadataReference.CreateFromFile(r.Location)))

                // Helper types
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(testCode, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview)));
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
