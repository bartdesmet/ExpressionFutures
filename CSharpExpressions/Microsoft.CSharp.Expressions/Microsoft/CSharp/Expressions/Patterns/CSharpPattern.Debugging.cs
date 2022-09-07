// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System.Collections.Generic;
using System.Dynamic.Utils;
using System.Xml.Linq;

namespace Microsoft.CSharp.Expressions
{
    partial class CSharpPattern
    {
        internal string DebugView => new CSharpDebugViewExpressionVisitor().GetDebugView(this).ToString();
    }

    partial class CSharpSubpattern
    {
        internal string DebugView => new CSharpDebugViewExpressionVisitor().GetDebugView(this).ToString();
    }

    partial class CSharpDebugViewExpressionVisitor
    {
        public XNode GetDebugView(CSharpPattern node)
        {
            return Visit(node);
        }

        public XNode GetDebugView(CSharpSubpattern node)
        {
            return Visit(node);
        }

        protected XNode Visit(CSharpPattern node)
        {
            VisitPattern(node);
            return _nodes.Pop();
        }

        protected XNode Visit(CSharpSubpattern node)
        {
            return node switch
            {
                PositionalCSharpSubpattern p => Visit(p),
                PropertyCSharpSubpattern p => Visit(p),
                _ => throw ContractUtils.Unreachable,
            };
        }

        protected XNode Visit(PositionalCSharpSubpattern node)
        {
            VisitPositionalSubpattern(node);
            return _nodes.Pop();
        }

        protected XNode Visit(PropertyCSharpSubpattern node)
        {
            VisitPropertySubpattern(node);
            return _nodes.Pop();
        }

        protected CSharpPattern Push(CSharpPattern node, params object[] content)
        {
            return Push($"{node.PatternType}Pattern", node, content);
        }

        protected CSharpPattern Push(string name, CSharpPattern node, params object[] content)
        {
            var nodes = new List<object>
            {
                new XAttribute(nameof(node.InputType), node.InputType),
                new XAttribute(nameof(node.NarrowedType), node.NarrowedType),
            };

            if (node is CSharpObjectPattern obj && obj.Variable != null)
            {
                nodes.Add(new XElement(nameof(obj.Variable), Visit(obj.Variable)));
            }

            nodes.AddRange(content);

            _nodes.Push(new XElement(name, nodes));

            return node;
        }

        protected internal override CSharpPattern VisitConstantPattern(ConstantCSharpPattern node)
        {
            return Push(node, Visit(node.Value));
        }

        protected internal override CSharpPattern VisitDiscardPattern(DiscardCSharpPattern node)
        {
            return Push(node);
        }

        protected internal override CSharpPattern VisitBinaryPattern(BinaryCSharpPattern node)
        {
            return Push(node,
                new XElement(nameof(node.Left), Visit(node.Left)),
                new XElement(nameof(node.Right), Visit(node.Right))
            );
        }

        protected internal override CSharpPattern VisitNotPattern(NotCSharpPattern node)
        {
            return Push(node, new XElement(nameof(node.Negated), Visit(node.Negated)));
        }

        protected internal override CSharpPattern VisitTypePattern(TypeCSharpPattern node)
        {
            return Push(node, new XAttribute(nameof(node.Type), node.Type));
        }

        protected internal override CSharpPattern VisitVarPattern(VarCSharpPattern node)
        {
            return Push(node, new XElement(nameof(node.Variable), Visit(node.Variable)));
        }

        protected internal override CSharpPattern VisitRelationalPattern(RelationalCSharpPattern node)
        {
            return Push(node, Visit(node.Value));
        }

        protected internal override CSharpPattern VisitDeclarationPattern(DeclarationCSharpPattern node)
        {
            return Push(node, new XAttribute(nameof(node.Type), node.Type));
        }

        protected internal override CSharpPattern VisitRecursivePattern(RecursiveCSharpPattern node)
        {
            var nodes = new List<object>();

            if (node.Type != null)
            {
                nodes.Add(new XAttribute(nameof(node.Type), node.Type));
            }

            if (node.DeconstructMethod != null)
            {
                nodes.Add(new XAttribute(nameof(node.DeconstructMethod), node.DeconstructMethod));
            }

            if (node.Deconstruction.Count > 0)
            {
                nodes.Add(Visit(nameof(node.Deconstruction), node.Deconstruction, Visit));
            }

            if (node.Properties.Count > 0)
            {
                nodes.Add(Visit(nameof(node.Properties), node.Properties, Visit));
            }

            return Push(node, nodes);
        }

        protected internal override CSharpPattern VisitITuplePattern(ITupleCSharpPattern node)
        {
            var nodes = new List<object>();

            if (node.GetLengthMethod != null)
            {
                nodes.Add(new XAttribute(nameof(node.GetLengthMethod), node.GetLengthMethod));
            }

            if (node.GetItemMethod != null)
            {
                nodes.Add(new XAttribute(nameof(node.GetItemMethod), node.GetItemMethod));
            }

            if (node.Deconstruction.Count > 0)
            {
                nodes.Add(Visit(nameof(node.Deconstruction), node.Deconstruction, Visit));
            }

            return Push(node, nodes);
        }

        protected internal override PositionalCSharpSubpattern VisitPositionalSubpattern(PositionalCSharpSubpattern node)
        {
            var nodes = new List<object>();

            if (node.Field != null)
            {
                var attrs = new List<object>();

                if (node.Field.Name != null)
                {
                    attrs.Add(new XAttribute(nameof(node.Field.Name), node.Field.Name));
                }

                attrs.Add(new XAttribute(nameof(node.Field.Index), node.Field.Index));

                nodes.Add(new XElement(nameof(node.Field), attrs));
            }
            else if (node.Parameter != null)
            {
                nodes.Add(new XAttribute(nameof(node.Parameter), node.Parameter));
            }

            nodes.Add(new XElement(nameof(node.Pattern), Visit(node.Pattern)));

            _nodes.Push(new XElement(node.SubpatternType.ToString(), nodes));

            return node;
        }

        protected internal override PropertyCSharpSubpattern VisitPropertySubpattern(PropertyCSharpSubpattern node)
        {
            var nodes = new List<object>
            {
                new XElement(nameof(node.Member), Visit(node.Member)),
                new XElement(nameof(node.Pattern), Visit(node.Pattern)),
            };

            _nodes.Push(new XElement(node.SubpatternType.ToString(), nodes));

            return node;
        }

        protected XNode Visit(PropertyCSharpSubpatternMember node)
        {
            var nodes = new List<object>();

            if (node.Member != null)
            {
                nodes.Add(new XAttribute(nameof(node.Member), node.Member));
            }
            else
            {
                nodes.Add(new XAttribute(nameof(node.TupleField), node.TupleField!));
            }

            if (node.Receiver != null)
            {
                nodes.Add(new XElement(nameof(node.Receiver), Visit(node.Receiver)));
            }

            return new XElement(nameof(PropertyCSharpSubpatternMember), nodes);
        }

        protected internal override CSharpPattern VisitListPattern(ListCSharpPattern node)
        {
            var nodes = new List<object>
            {
                new XElement(nameof(node.LengthAccess), Visit(node.LengthAccess)),
                new XElement(nameof(node.IndexerAccess), Visit(node.IndexerAccess)),
                Visit(nameof(node.Patterns), node.Patterns, Visit),
            };

            return Push(node, nodes);
        }

        protected internal override CSharpPattern VisitSlicePattern(SliceCSharpPattern node)
        {
            var nodes = new List<object>();

            if (node.IndexerAccess != null)
            {
                nodes.Add(new XElement(nameof(node.IndexerAccess), Visit(node.IndexerAccess)));
            }

            if (node.Pattern != null)
            {
                nodes.Add(new XElement(nameof(node.Pattern), Visit(node.Pattern)));
            }

            return Push(node, nodes);
        }
    }
}
