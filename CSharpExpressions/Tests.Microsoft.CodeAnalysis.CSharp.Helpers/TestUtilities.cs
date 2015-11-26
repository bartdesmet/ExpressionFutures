// Prototyping extended expression trees for C#.
//
// bartde - November 2015

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CSharp.Expressions;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    public static class TestUtilities
    {
        public static string GetDebugView(string expr)
        {
            // TODO: Investigate using the scripting APIs here instead.
            
            var typeName = "Expressions";
            var propName = "Expression";

            var exprProp = $"    public static Expression {propName} => {expr};";

            var tree = CSharpSyntaxTree.ParseText($@"
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

public static class {typeName}
{{
{exprProp}
}}
");
            var csc = CSharpCompilation
                // A class library `Expressions` which will be emitted in memory
                .Create("Expressions")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, warningLevel: 0))
                
                // BCL assemblies
                .AddReferences(MetadataReference.CreateFromFile(typeof(int).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(Expression).Assembly.Location))
                
                // Our custom assembly
                .AddReferences(MetadataReference.CreateFromFile(typeof(CSharpExpression).Assembly.Location))

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
            var prp = typ.GetProperty(propName);
            var exp = (Expression)prp.GetValue(null);
            return exp.DebugView().ToString();
        }
    }
}
