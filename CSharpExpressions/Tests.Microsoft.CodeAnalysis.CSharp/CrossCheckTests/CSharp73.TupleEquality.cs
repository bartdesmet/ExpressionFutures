// Prototyping extended expression trees for C#.
//
// bartde - May 2020

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
        public void CrossCheck_TupleEquality()
        {
            var eq = Compile<Func<(int, int), (int, int), bool>>("(t1, t2) => Return(t1, \"L\") == Return(t2, \"R\")");
            eq((1, 2), (1, 2));
            eq((1, 2), (-1, 2));
            eq((1, 2), (1, -2));

            var ne = Compile<Func<(int, int), (int, int), bool>>("(t1, t2) => Return(t1, \"L\") != Return(t2, \"R\")");
            ne((1, 2), (1, 2));
            ne((1, 2), (-1, 2));
            ne((1, 2), (1, -2));
        }

        [TestMethod]
        public void CrossCheck_TupleEquality_Lifted()
        {
            var eq = Compile<Func<(int, int)?, (int, int)?, bool>>("(t1, t2) => Return(t1, \"L\") == Return(t2, \"R\")");

            eq(null, null);
            eq((1, 2), null);
            eq(null, (1, 2));

            eq((1, 2), (1, 2));
            eq((1, 2), (-1, 2));
            eq((1, 2), (1, -2));

            var ne = Compile<Func<(int, int)?, (int, int)?, bool>>("(t1, t2) => Return(t1, \"L\") != Return(t2, \"R\")");

            ne(null, null);
            ne((1, 2), null);
            ne(null, (1, 2));

            ne((1, 2), (1, 2));
            ne((1, 2), (-1, 2));
            ne((1, 2), (1, -2));
        }

        [TestMethod]
        public void CrossCheck_TupleEquality_Literal()
        {
            Compile<Func<bool>>("() => (Return(1, \"L1\"), Return(2, \"L2\")) == (Return(1, \"R1\"), Return(2, \"R2\"))")();
            Compile<Func<bool>>("() => (Return(1, \"L1\"), Return(2, \"L2\")) == (Return(-1, \"R1\"), Return(2, \"R2\"))")();
            Compile<Func<bool>>("() => (Return(1, \"L1\"), Return(2, \"L2\")) == (Return(-1, \"R1\"), Return(-2, \"R2\"))")();
            Compile<Func<bool>>("() => (Return(1, \"L1\"), Return(2, \"L2\")) != (Return(1, \"R1\"), Return(2, \"R2\"))")();
            Compile<Func<bool>>("() => (Return(1, \"L1\"), Return(2, \"L2\")) != (Return(-1, \"R1\"), Return(2, \"R2\"))")();
            Compile<Func<bool>>("() => (Return(1, \"L1\"), Return(2, \"L2\")) != (Return(1, \"R1\"), Return(-2, \"R2\"))")();

            var eq1 = Compile<Func<(int, int), bool>>("t1 => Return(t1, \"L\") == (Return(1, \"R1\"), Return(2, \"R2\"))");
            eq1((1, 2));
            eq1((-1, 2));
            eq1((1, -2));

            var eq2 = Compile<Func<(int, int), bool>>("t2 => (Return(1, \"L1\"), Return(2, \"L2\")) == Return(t2, \"R\")");
            eq2((1, 2));
            eq2((-1, 2));
            eq2((1, -2));

            var ne1 = Compile<Func<(int, int), bool>>("t1 => Return(t1, \"L\") != (Return(1, \"R1\"), Return(2, \"R2\"))");
            ne1((1, 2));
            ne1((-1, 2));
            ne1((1, -2));

            var ne2 = Compile<Func<(int, int), bool>>("t2 => (Return(1, \"L1\"), Return(2, \"L2\")) != Return(t2, \"R\")");
            ne2((1, 2));
            ne2((-1, 2));
            ne2((1, -2));
        }

        [TestMethod]
        public void CrossCheck_TupleEquality_Null()
        {
            Compile<Func<bool>>("() => (1, null, 2L) == (1, \"\", 2L)")();
            Compile<Func<bool>>("() => (1, null, 2L) != (1, \"\", 2L)")();
        }

        [TestMethod]
        [Ignore] // TODO: These literals have no type, causing us to raise an (intentional) error. Turn this into a test for the appropriate compiler warning.
        public void CrossCheck_TupleEquality_NullNull()
        {
            Compile<Func<bool>>("() => (1, null, 2L) == (1, null, 2L)")();
            Compile<Func<bool>>("() => (1, null, 2L) != (1, null, 2L)")();
        }

        [TestMethod]
        public void CrossCheck_TupleEquality_Nested()
        {
            var dt = DateTime.Now;

            var eq = Compile<Func<(int, (bool, string), (int, (long, long), DateTime)), (int, (bool, string), (int, (long, long), DateTime)), bool>>("(t1, t2) => Return(t1, \"L\") == Return(t2, \"R\")");
            eq((1, (true, "bar"), (2, (3L, 4L), dt)), (1, (true, "bar"), (2, (3L, 4L), dt)));
            eq((1, (true, "bar"), (2, (3L, 4L), dt)), (-1, (true, "bar"), (2, (3L, 4L), dt)));
            eq((1, (true, "bar"), (2, (3L, 4L), dt)), (1, (false, "bar"), (2, (3L, 4L), dt)));
            eq((1, (true, "bar"), (2, (3L, 4L), dt)), (1, (true, "foo"), (2, (3L, 4L), dt)));
            eq((1, (true, "bar"), (2, (3L, 4L), dt)), (1, (true, "bar"), (-2, (3L, 4L), dt)));
            eq((1, (true, "bar"), (2, (3L, 4L), dt)), (1, (true, "bar"), (2, (-3L, 4L), dt)));
            eq((1, (true, "bar"), (2, (3L, 4L), dt)), (1, (true, "bar"), (2, (3L, -4L), dt)));
            eq((1, (true, "bar"), (2, (3L, 4L), dt)), (1, (true, "bar"), (2, (3L, 4L), dt.AddSeconds(1))));

            var ne = Compile<Func<(int, (bool, string), (int, (long, long), DateTime)), (int, (bool, string), (int, (long, long), DateTime)), bool>>("(t1, t2) => Return(t1, \"L\") != Return(t2, \"R\")");
            ne((1, (true, "bar"), (2, (3L, 4L), dt)), (1, (true, "bar"), (2, (3L, 4L), dt)));
            ne((1, (true, "bar"), (2, (3L, 4L), dt)), (-1, (true, "bar"), (2, (3L, 4L), dt)));
            ne((1, (true, "bar"), (2, (3L, 4L), dt)), (1, (false, "bar"), (2, (3L, 4L), dt)));
            ne((1, (true, "bar"), (2, (3L, 4L), dt)), (1, (true, "foo"), (2, (3L, 4L), dt)));
            ne((1, (true, "bar"), (2, (3L, 4L), dt)), (1, (true, "bar"), (-2, (3L, 4L), dt)));
            ne((1, (true, "bar"), (2, (3L, 4L), dt)), (1, (true, "bar"), (2, (-3L, 4L), dt)));
            ne((1, (true, "bar"), (2, (3L, 4L), dt)), (1, (true, "bar"), (2, (3L, -4L), dt)));
            ne((1, (true, "bar"), (2, (3L, 4L), dt)), (1, (true, "bar"), (2, (3L, 4L), dt.AddSeconds(1))));
        }

        [TestMethod]
        public void CrossCheck_TupleEquality_Nested_Literal()
        {
            var eqs = new[]
            {
                Compile<Func<(int, (bool, string), (int, (long, long), DateTime)), bool>>("t => t == (Return(1), (Return(true), Return(\"bar\")), (Return(2), (Return(3L), Return(4L)), Return(new DateTime(1983, 2, 11))))"),
                Compile<Func<(int, (bool, string), (int, (long, long), DateTime)), bool>>("t => t != (Return(1), (Return(true), Return(\"bar\")), (Return(2), (Return(3L), Return(4L)), Return(new DateTime(1983, 2, 11))))"),
                Compile<Func<(int, (bool, string), (int, (long, long), DateTime)), bool>>("t => (Return(1), (Return(true), Return(\"bar\")), (Return(2), (Return(3L), Return(4L)), Return(new DateTime(1983, 2, 11)))) == t"),
                Compile<Func<(int, (bool, string), (int, (long, long), DateTime)), bool>>("t => (Return(1), (Return(true), Return(\"bar\")), (Return(2), (Return(3L), Return(4L)), Return(new DateTime(1983, 2, 11)))) != t"),
            };

            foreach (var eq in eqs)
            {
                eq((1, (true, "bar"), (2, (3L, 4L), new DateTime(1983, 2, 11))));
                eq((-1, (true, "bar"), (2, (3L, 4L), new DateTime(1983, 2, 11))));
                eq((1, (false, "bar"), (2, (3L, 4L), new DateTime(1983, 2, 11))));
                eq((1, (true, "foo"), (2, (3L, 4L), new DateTime(1983, 2, 11))));
                eq((1, (true, "bar"), (-2, (3L, 4L), new DateTime(1983, 2, 11))));
                eq((1, (true, "bar"), (2, (-3L, 4L), new DateTime(1983, 2, 11))));
                eq((1, (true, "bar"), (2, (3L, -4L), new DateTime(1983, 2, 11))));
                eq((1, (true, "bar"), (2, (3L, 4L), new DateTime(1985, 2, 11))));
            }

            Compile<Func<bool>>("() => (Return(1, \"L1\"), (Return(true, \"L2\"), Return(\"bar\", \"L3\")), (Return(2, \"L4\"), (Return(3L, \"L5\"), Return(4L, \"L6\")), Return(new DateTime(1983, 2, 11), \"L7\"))) == (Return(1, \"R1\"), (Return(true, \"R2\"), Return(\"bar\", \"R3\")), (Return(2, \"R4\"), (Return(3L, \"R5\"), Return(4L, \"R6\")), Return(new DateTime(1983, 2, 11), \"R7\")))")();
            Compile<Func<bool>>("() => (Return(1, \"L1\"), (Return(true, \"L2\"), Return(\"bar\", \"L3\")), (Return(2, \"L4\"), (Return(3L, \"L5\"), Return(4L, \"L6\")), Return(new DateTime(1983, 2, 11), \"L7\"))) == (Return(1, \"R1\"), (Return(true, \"R2\"), Return(\"bar\", \"R3\")), (Return(2, \"R4\"), (Return(-3L, \"R5\"), Return(4L, \"R6\")), Return(new DateTime(1983, 2, 11), \"R7\")))")();
            Compile<Func<bool>>("() => (Return(1, \"L1\"), (Return(true, \"L2\"), Return(\"bar\", \"L3\")), (Return(2, \"L4\"), (Return(3L, \"L5\"), Return(4L, \"L6\")), Return(new DateTime(1983, 2, 11), \"L7\"))) != (Return(1, \"R1\"), (Return(true, \"R2\"), Return(\"bar\", \"R3\")), (Return(2, \"R4\"), (Return(3L, \"R5\"), Return(4L, \"R6\")), Return(new DateTime(1983, 2, 11), \"R7\")))")();
            Compile<Func<bool>>("() => (Return(1, \"L1\"), (Return(true, \"L2\"), Return(\"bar\", \"L3\")), (Return(2, \"L4\"), (Return(3L, \"L5\"), Return(4L, \"L6\")), Return(new DateTime(1983, 2, 11), \"L7\"))) != (Return(1, \"R1\"), (Return(true, \"R2\"), Return(\"bar\", \"R3\")), (Return(2, \"R4\"), (Return(-3L, \"R5\"), Return(4L, \"R6\")), Return(new DateTime(1983, 2, 11), \"R7\")))")();
        }

        [TestMethod]
        public void CrossCheck_TupleEquality_Dynamic()
        {
            var eqs = new[]
            {
                Compile<Func<(int, dynamic), (int, dynamic), bool>>("(t1, t2) => t1 == t2"),
                Compile<Func<(int, dynamic), (int, dynamic), bool>>("(t1, t2) => t1 != t2"),
            };

            foreach (var eq in eqs)
            {
                eq((1, 2), (1, 2));
                eq((1, 2), (3, 4));
                
                eq((1, 2), (1, "bar"));
                
                eq((1, "bar"), (1, "bar"));
                eq((1, "bar"), (3, "bar"));
                eq((1, "bar"), (1, "foo"));
            }
        }
    }
}
