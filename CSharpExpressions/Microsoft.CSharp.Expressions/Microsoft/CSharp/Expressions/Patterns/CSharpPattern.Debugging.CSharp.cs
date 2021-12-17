// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System.Collections.Generic;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Security.Cryptography;

namespace Microsoft.CSharp.Expressions
{
    static class CSharpPrintingVisitorExtensionsForPatterns
    {
        public static void Visit(this ICSharpPrintingVisitor visitor, CSharpPattern pattern)
        {
            switch (pattern.PatternType)
            {
                case CSharpPatternType.Constant:
                    visitor.Visit((ConstantCSharpPattern)pattern);
                    break;
                case CSharpPatternType.Discard:
                    visitor.Out("_");
                    break;
                case CSharpPatternType.Type:
                    visitor.Visit((TypeCSharpPattern)pattern);
                    break;
                case CSharpPatternType.And:
                case CSharpPatternType.Or:
                    visitor.Visit((BinaryCSharpPattern)pattern);
                    break;
                case CSharpPatternType.Not:
                    visitor.Visit((NotCSharpPattern)pattern);
                    break;
                case CSharpPatternType.LessThan:
                case CSharpPatternType.LessThanOrEqual:
                case CSharpPatternType.GreaterThan:
                case CSharpPatternType.GreaterThanOrEqual:
                    visitor.Visit((RelationalCSharpPattern)pattern);
                    break;
                case CSharpPatternType.Declaration:
                    visitor.Visit((DeclarationCSharpPattern)pattern);
                    break;
                case CSharpPatternType.Var:
                    visitor.Visit((VarCSharpPattern)pattern);
                    break;
                case CSharpPatternType.Recursive:
                    visitor.Visit((RecursiveCSharpPattern)pattern);
                    break;
                case CSharpPatternType.ITuple:
                    visitor.Visit((ITupleCSharpPattern)pattern);
                    break;
                default:
                    break;
            }
        }

        private static void Visit(this ICSharpPrintingVisitor visitor, ConstantCSharpPattern pattern)
        {
            visitor.Visit(pattern.Value);
        }
        
        private static void Visit(this ICSharpPrintingVisitor visitor, BinaryCSharpPattern pattern)
        {
            var op = pattern.PatternType switch
            {
                CSharpPatternType.And => " and ",
                CSharpPatternType.Or => " or ",
                _ => throw ContractUtils.Unreachable,
            };

            visitor.ParenthesizedVisit(pattern, pattern.Left);
            visitor.Out(op);
            visitor.ParenthesizedVisit(pattern, pattern.Right);
        }

        private static void Visit(this ICSharpPrintingVisitor visitor, NotCSharpPattern pattern)
        {
            visitor.Out("not ");
            visitor.ParenthesizedVisit(pattern, pattern.Negated);
        }

        private static void ParenthesizedVisit(this ICSharpPrintingVisitor visitor, CSharpPattern parent, CSharpPattern nodeToVisit)
        {
            if (nodeToVisit.GetPrecedence() < parent.GetPrecedence())
            {
                visitor.Out("(");
                visitor.Visit(nodeToVisit);
                visitor.Out(")");
            }
            else
            {
                visitor.Visit(nodeToVisit);
            }
        }

        private static int GetPrecedence(this CSharpPattern pattern) => pattern.PatternType switch
        {
            CSharpPatternType.Or => 1,
            CSharpPatternType.And => 2,
            CSharpPatternType.Not => 3,
            _ => 9,
        };

        private static void Visit(this ICSharpPrintingVisitor visitor, RelationalCSharpPattern pattern)
        {
            var op = pattern.PatternType switch
            {
                CSharpPatternType.LessThan => "<",
                CSharpPatternType.LessThanOrEqual => "<=",
                CSharpPatternType.GreaterThan => ">",
                CSharpPatternType.GreaterThanOrEqual => ">=",
                _ => throw ContractUtils.Unreachable,
            };

            visitor.Out(op);
            visitor.Out(" ");
            visitor.Visit(pattern.Value);
        }

        private static void Visit(this ICSharpPrintingVisitor visitor, TypeCSharpPattern pattern)
        {
            visitor.Out(visitor.ToCSharp(pattern.Type));
        }

        private static void Visit(this ICSharpPrintingVisitor visitor, DeclarationCSharpPattern pattern)
        {
            visitor.Out(visitor.ToCSharp(pattern.Type));
            visitor.VisitDesignation(pattern);
        }

        private static void Visit(this ICSharpPrintingVisitor visitor, VarCSharpPattern pattern)
        {
            visitor.Out("var");
            visitor.VisitDesignation(pattern);
        }

        private static void Visit(this ICSharpPrintingVisitor visitor, RecursiveCSharpPattern pattern)
        {
            // positional_pattern: type? '(' subpatterns? ')' property_subpattern? simple_designation?
            // property_pattern: type? property_subpattern simple_designation?

            if (pattern.Type != null)
            {
                visitor.Out(visitor.ToCSharp(pattern.Type));
                visitor.Out(" ");
            }

            if (pattern.Deconstruction.Count > 0)
            {
                visitor.Out("(");
                visitor.ArgsVisit(pattern.Deconstruction);
                visitor.Out(")");
            }

            if (pattern.Properties.Count > 0)
            {
                visitor.Out("{ ");
                visitor.ArgsVisit(pattern.Properties);
                visitor.Out(" }");
            }
            else if (pattern.Deconstruction.Count == 0)
            {
                visitor.Out("{ }");
            }
            
            visitor.VisitDesignation(pattern, noDiscard: true);
        }

        private static void Visit(this ICSharpPrintingVisitor visitor, ITupleCSharpPattern pattern)
        {
            if (pattern.Deconstruction.Count > 0)
            {
                visitor.Out("(");
                visitor.ArgsVisit(pattern.Deconstruction);
                visitor.Out(")");
            }
        }

        private static void VisitDesignation(this ICSharpPrintingVisitor visitor, CSharpObjectPattern pattern, bool noDiscard = false)
        {
            if (pattern.Variable != null)
            {
                visitor.Out(" ");
                visitor.Visit(pattern.Variable);
            }
            else if (!noDiscard)
            {
                visitor.Out(" _");
            }
        }

        private static void ArgsVisit(this ICSharpPrintingVisitor visitor, IList<PositionalCSharpSubpattern> args)
        {
            var n = args.Count;

            for (var i = 0; i < n; i++)
            {
                var arg = args[i];

                if (arg.Field != null)
                {
                    visitor.Out(arg.Field.Name);
                    visitor.Out(": ");
                }
                else if (arg.Parameter != null)
                {
                    visitor.Out(arg.Parameter.Name);
                    visitor.Out(": ");
                }

                visitor.Visit(arg.Pattern);

                if (i != n - 1)
                {
                    visitor.Out(", ");
                }
            }
        }

        private static void ArgsVisit(this ICSharpPrintingVisitor visitor, IList<PropertyCSharpSubpattern> args)
        {
            var n = args.Count;

            for (var i = 0; i < n; i++)
            {
                var arg = args[i];

                visitor.Visit(arg.Member);
                visitor.Out(": ");

                visitor.Visit(arg.Pattern);

                if (i != n - 1)
                {
                    visitor.Out(", ");
                }
            }
        }

        private static void Visit(this ICSharpPrintingVisitor visitor, PropertyCSharpSubpatternMember member)
        {
            if (member.Receiver != null)
            {
                visitor.Visit(member.Receiver);
                visitor.Out(".");
            }

            if (member.Member != null)
            {
                visitor.Out(member.Member.Name);
            }
            else
            {
                if (member.TupleField.Name != null)
                {
                    visitor.Out(member.TupleField.Name);
                }
                else
                {
                    visitor.Out("Item" + (member.TupleField.Index + 1));
                }
            }
        }
    }
}
