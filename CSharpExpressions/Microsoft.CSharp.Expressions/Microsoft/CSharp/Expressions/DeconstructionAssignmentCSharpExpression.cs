// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;

namespace Microsoft.CSharp.Expressions
{
    /*
     * TODO: This file contains a placeholder for the APIs called for deconstructing assignment.
     * 
        // Case 1
        var t = (1, 2);
        (long x, int y) = t;

        // Case 2
        var t = (1, (2, 3));
        (long x, (int y, double z)) = t;

        // Case 3
        var t = (1, (2, 3));
        var (x, yz) = t;
        var (y, z) = yz;

        // Case 4
        var p = new Point { X = 1, Y = 2 };
        (int x, int y) = p;

        // Case 5
        var t = (1, new Point { X = 2, Y = 3 });
        (long a, (int x, int y)) = t;
     */

    public class Conversion
    {
    }

    public class SimpleConversion : Conversion
    {
        public LambdaExpression Conversion { get; set; }
    }

    public class DeconstructionConversion : Conversion
    {
        public LambdaExpression Deconstruct { get; set; }
        public ReadOnlyCollection<Conversion> Conversions { get; set; }
    }

    partial class CSharpExpression
    {
        public static Expression DeconstructionAssignment(Type type, Expression left, Expression right, DeconstructionConversion conversion)
        {
            return Expression.Default(type);
        }

        public static SimpleConversion Convert(LambdaExpression conversion)
        {
            return new SimpleConversion { Conversion = conversion };
        }

        public static DeconstructionConversion Deconstruct(params Conversion[] conversions)
        {
            return new DeconstructionConversion { Conversions = conversions.ToReadOnly() };
        }

        public static DeconstructionConversion Deconstruct(LambdaExpression deconstruct, params Conversion[] conversions)
        {
            return new DeconstructionConversion { Deconstruct = deconstruct, Conversions = conversions.ToReadOnly() };
        }

        public static LambdaExpression DeconstructLambda(Expression body, params ParameterExpression[] parameters)
        {
            return Lambda(body, parameters);
        }
    }
}
