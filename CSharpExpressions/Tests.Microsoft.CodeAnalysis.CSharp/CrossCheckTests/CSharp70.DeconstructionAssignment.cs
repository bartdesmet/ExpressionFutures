// Prototyping extended expression trees for C#.
//
// bartde - December 2021

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
        public void CrossCheck_DeconstructionAssignment_Tuple()
        {
            var f = Compile<Action<(int, int)>>("t => { var (x, y) = t; Log(x); Log(y); }");

            f((1, 2));
        }

        [TestMethod]
        public void CrossCheck_DeconstructionAssignment_Tuple_Large()
        {
            var f = Compile<Action<(int, int, int, int, int, int, int, int, int, int)>>(@"t => {
                var (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10) = t;
                Log(x1); Log(x2); Log(x3); Log(x4); Log(x5); Log(x6); Log(x7); Log(x8); Log(x9); Log(x10);
            }");

            f((1, 2, 3, 4, 5, 6, 7, 8, 9, 10));
        }

        [TestMethod]
        public void CrossCheck_DeconstructionAssignment_Tuple_Nested_NoRecursiveDeconstruction()
        {
            var f = Compile<Action<(int, (bool, char), string)>>(@"t => {
                var (x, bc, s) = t;
                Log(x); Log(bc); Log(s);
            }");

            f((1, (true, 'a'), "bar"));
        }

        [TestMethod]
        public void CrossCheck_DeconstructionAssignment_Tuple_Nested_RecursiveDeconstruction()
        {
            var f = Compile<Action<(int, (bool, char), string)>>(@"t => {
                var (x, (b, c), s) = t;
                Log(x); Log(b); Log(c); Log(s);
            }");

            f((1, (true, 'a'), "bar"));
        }

        [TestMethod]
        public void CrossCheck_DeconstructionAssignment_Tuple_Nested_DeepLeft()
        {
            var f = Compile<Action<(int, (int, (int, int)))>>(@"t => {
                var (a, (b, (c, d))) = t;
                Log(a); Log(b); Log(c); Log(d);
            }");

            f((1, (2, (3, 4))));
        }

        [TestMethod]
        public void CrossCheck_DeconstructionAssignment_Tuple_Nested_DeepRight()
        {
            var f = Compile<Action<(((int, int), int), int)>>(@"t => {
                var (((a, b), c), d) = t;
                Log(a); Log(b); Log(c); Log(d);
            }");

            f((((1, 2), 3), 4));
        }

        [TestMethod]
        public void CrossCheck_DeconstructionAssignment_Point_Deconstruct_InstanceMethod()
        {
            var f = Compile<Action<int, int>>(@"(px, py) => {
                var (x, y) = new Point(Log) { X = px, Y = py };
                Log(x); Log(y);
            }");

            f(1, 2);
        }

        [TestMethod]
        public void CrossCheck_DeconstructionAssignment_Point2D_Deconstruct_ExtensionMethod()
        {
            var f = Compile<Action<int, int>>(@"(px, py) => {
                var (x, y) = new Point2D(Log) { X = px, Y = py };
                Log(x); Log(y);
            }");

            f(1, 2);
        }

        [TestMethod]
        public void CrossCheck_DeconstructionAssignment_TuplePoint_Recursive()
        {
            var f = Compile<Action<int, int, int, int>>(@"(px1, py1, px2, py2) => {
                var p1 = new Point(Log) { X = px1, Y = py1 };
                var p2 = new Point(Log) { X = px2, Y = py2 };
                var rect = (p1, p2);
                var ((x1, y1), (x2, y2)) = rect;
                Log(x1); Log(y1);
                Log(x2); Log(y2);
            }");

            f(1, 2, 3, 4);
        }

        [TestMethod]
        public void CrossCheck_DeconstructionAssignment_Deconstruct_Nested_DeepLeft()
        {
            var f = Compile<Action<int, int, int, int>>(@"(x1, x2, x3, x4) => {
                var t =
                    new MyTuple<int, MyTuple<int, MyTuple<int, int>>>(
                        1,
                        new MyTuple<int, MyTuple<int, int>>(
                            2,
                            new MyTuple<int, int>(
                                3,
                                4,
                                Log
                            ),
                            Log
                        ),
                        Log
                    );
                var (a, (b, (c, d))) = t;
                Log(a); Log(b); Log(c); Log(d);
            }");

            f(1, 2, 3, 4);
        }

        [TestMethod]
        public void CrossCheck_DeconstructionAssignment_Deconstruct_Nested_DeepRight()
        {
            var f = Compile<Action<int, int, int, int>>(@"(x1, x2, x3, x4) => {
                var t =
                    new MyTuple<MyTuple<MyTuple<int, int>, int>, int>(
                        new MyTuple<MyTuple<int, int>, int>(
                            new MyTuple<int, int>(
                                1,
                                2,
                                Log
                            ),
                            3,
                            Log
                        ),
                        4,
                        Log
                    );
                var (((a, b), c), d) = t;
                Log(a); Log(b); Log(c); Log(d);
            }");

            f(1, 2, 3, 4);
        }

        [TestMethod]
        public void CrossCheck_DeconstructionAssignment_Deconstruct_Nested_Left_NullReference()
        {
            var f = Compile<Action<MyTuple<int, MyTuple<int, int>>>>(@"t => {
                var (a, (b, c)) = t;
                Log(a); Log(b); Log(c);
            }");

            AssertEx.Throws<NullReferenceException>(() => f(new MyTuple<int, MyTuple<int, int>>(1, null)));
        }

        [TestMethod]
        public void CrossCheck_DeconstructionAssignment_Deconstruct_Nested_Right_NullReference()
        {
            var f = Compile<Action<MyTuple<MyTuple<int, int>, int>>>(@"t => {
                var ((a, b), c) = t;
                Log(a); Log(b); Log(c);
            }");

            AssertEx.Throws<NullReferenceException>(() => f(new MyTuple<MyTuple<int, int>, int>(null, 1)));
        }

        [TestMethod]
        public void CrossCheck_DeconstructionAssignment_MyTuplePoint_Recursive()
        {
            var f = Compile<Action<int, int, int, int>>(@"(px1, py1, px2, py2) => {
                var p1 = new Point(Log) { X = px1, Y = py1 };
                var p2 = new Point(Log) { X = px2, Y = py2 };
                var rect = new MyTuple<Point, Point>(p1, p2);
                var ((x1, y1), (x2, y2)) = rect;
                Log(x1); Log(y1);
                Log(x2); Log(y2);
            }");

            f(1, 2, 3, 4);
        }
    }
}
