// Prototyping extended expression trees for C#.
//
// bartde - December 2015

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.CompilerServices;

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
        #region Named parameters

        // TODO: any issues with interface methods?

        [TestMethod]
        public void CrossCheck_NamedParameters_Call()
        {
            var f = Compile<Func<string, string>>(@"s =>
{
    return s.Substring(length: Return(2), startIndex: Return(1));
}");
            f("foobar");
            AssertEx.Throws<ArgumentOutOfRangeException>(() => f(""));
            AssertEx.Throws<NullReferenceException>(() => f(null));
        }

        [TestMethod]
        public void CrossCheck_NamedParameters_Call_ByRef1()
        {
            var f = Compile<Func<int[], int>>(@"xs =>
{
    return Utils.NamedParamByRef(y: Return(42), x: ref xs[0]);
}");
            f(new[] { 17 });
            AssertEx.Throws<IndexOutOfRangeException>(() => f(new int[0]));
            AssertEx.Throws<NullReferenceException>(() => f(null));
        }

        [TestMethod]
        public void CrossCheck_NamedParameters_Call_ByRef2()
        {
            var f = Compile<Func<StrongBox<int>, int>>(@"b =>
{
    return Utils.NamedParamByRef(y: Return(42), x: ref b.Value);
}");
            f(new StrongBox<int>(17));
            AssertEx.Throws<NullReferenceException>(() => f(null));
        }

        [TestMethod]
        public void CrossCheck_NamedParameters_Call_ByRef3()
        {
            var f = Compile<Func<int>>(@"() =>
{
    var b = new WeakBox<int> { Value = 17 };
    return Utils.NamedParamByRef(y: Return(42), x: ref b.Value);
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_NamedParameters_Call_ByRef4()
        {
            var f = Compile<Func<int>>(@"() =>
{
    var b = new StrongBox<int>(1);
    Log(b.Value);

    var res = System.Threading.Interlocked.Exchange(value: Return(42), location1: ref b.Value);
    Log(b.Value);

    return res;
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_NamedParameters_Call_ByRef5()
        {
            var f = Compile<Func<int>>(@"() =>
{
    var b = new WeakBox<int>(1);
    Log(b.Value);

    var res = System.Threading.Interlocked.Exchange(value: Return(42), location1: ref b.Value);
    Log(b.Value);

    return res;
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_NamedParameters_Call_ByRef6()
        {
            var f = Compile<Func<int>>(@"() =>
{
    var b = new WeakBox<WeakBox<int>>(new WeakBox<int>(1));
    Log(b.Value.Value);

    var res = System.Threading.Interlocked.Exchange(value: Return(42), location1: ref b.Value.Value);
    Log(b.Value.Value);

    return res;
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_NamedParameters_Call_ByRef7()
        {
            var f = Compile<Func<int>>(@"() =>
{
    var b = new[] { new WeakBox<int>(1) };
    Log(b[0].Value);

    var res = System.Threading.Interlocked.Exchange(value: Return(42), location1: ref b[Return(0)].Value);
    Log(b[0].Value);

    return res;
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_NamedParameters_Call_ByRef8()
        {
            var f = Compile<Func<WeakBox<int>[], int>>(@"b =>
{
    var res = System.Threading.Interlocked.Exchange(value: Return(42), location1: ref b[Return(0)].Value);
    return res;
}");
            AssertEx.Throws<NullReferenceException>(() => f(null));
            AssertEx.Throws<IndexOutOfRangeException>(() => f(new WeakBox<int>[0]));
        }

        [TestMethod]
        public void CrossCheck_NamedParameters_Call_ByRef9()
        {
            var f = Compile<Func<int>>(@"() =>
{
    var b = new WeakBox<int>[][] { new WeakBox<int>[] { new WeakBox<int>(1) } };
    Log(b[0][0].Value);

    var res = System.Threading.Interlocked.Exchange(value: Return(42), location1: ref b[Return(1) - 1][Return(2) - 2].Value);
    Log(b[0][0].Value);

    return res;
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_NamedParameters_Call_ByRef10()
        {
            var f = Compile<Func<WeakBox<int>[][], int>>(@"b =>
{
    var res = System.Threading.Interlocked.Exchange(value: Return(42), location1: ref b[Return(1) - 1][Return(2) - 2].Value);
    return res;
}");
            AssertEx.Throws<NullReferenceException>(() => f(null));
            AssertEx.Throws<IndexOutOfRangeException>(() => f(new WeakBox<int>[0][]));
            AssertEx.Throws<IndexOutOfRangeException>(() => f(new WeakBox<int>[1][] { new WeakBox<int>[0] }));
        }

        [TestMethod]
        public void CrossCheck_NamedParameters_Call_ByRef11()
        {
            var f = Compile<Func<int>>(@"() =>
{
    var b = new WeakBox<int>[,] { { new WeakBox<int>(1) } };
    Log(b[0, 0].Value);

    var res = System.Threading.Interlocked.Exchange(value: Return(42), location1: ref b[Return(1) - 1, Return(2) - 2].Value);
    Log(b[0, 0].Value);

    return res;
}");
            f();
        }

        [TestMethod]
        public void CrossCheck_NamedParameters_Call_ByRef12()
        {
            var f = Compile<Func<WeakBox<int>[,], int>>(@"b =>
{
    var res = System.Threading.Interlocked.Exchange(value: Return(42), location1: ref b[Return(1) - 1, Return(2) - 2].Value);
    return res;
}");
            AssertEx.Throws<NullReferenceException>(() => f(null));
            AssertEx.Throws<IndexOutOfRangeException>(() => f(new WeakBox<int>[0, 0]));
            AssertEx.Throws<IndexOutOfRangeException>(() => f(new WeakBox<int>[0, 1]));
            AssertEx.Throws<IndexOutOfRangeException>(() => f(new WeakBox<int>[1, 0]));
        }

        [TestMethod]
        public void CrossCheck_NamedParameters_New()
        {
            var f = Compile<Func<int>>("() => new NamedAndOptionalParameters(z: Return(false), y: Return(\"foobar\"), x: Return(43)).Value");
            f();
        }

        [TestMethod]
        public void CrossCheck_NamedParameters_Index()
        {
            var f = Compile<Func<int>>("() => new IndexerWithNamedAndOptionalParameters()[z: Return(false), y: Return(\"foobar\"), x: Return(43)]");
            f();
        }

        [TestMethod]
        public void CrossCheck_NamedParameters_Invoke()
        {
            var f = Compile<Func<DelegateWithNamedAndOptionalParameters, int>>("f => f(z: Return(false), y: Return(\"foobar\"), x: Return(43))");
            f((x, y, z) => x + y.Length + (z ? 1 : 0));
        }

        #endregion

        #region Optional parameters

        // TODO: more cases
        // TODO: ref/out
        // TODO: generic methods with default(T)

        [TestMethod]
        public void CrossCheck_OptionalParameters_Call1()
        {
            var f1 = Compile<Func<int>>("() => Utils.OptionalParams()");
            f1();

            var f2 = Compile<Func<int>>("() => Utils.OptionalParams(Return(43))");
            f2();

            var f3 = Compile<Func<int>>("() => Utils.OptionalParams(Return(43), Return(\"foobar\"))");
            f3();

            var f4 = Compile<Func<int>>("() => Utils.OptionalParams(Return(43), Return(\"foobar\"), Return(false))");
            f4();
        }

        [TestMethod]
        public void CrossCheck_OptionalParameters_Call2()
        {
            var f1 = Compile<Func<int>>("() => Utils.OptionalParams(Return(43), y: Return(\"foobar\"))");
            f1();

            var f2 = Compile<Func<int>>("() => Utils.OptionalParams(Return(43), z: Return(false))");
            f2();

            var f3 = Compile<Func<int>>("() => Utils.OptionalParams(Return(43), Return(\"foobar\"), z: Return(false))");
            f3();
        }

        [TestMethod]
        public void CrossCheck_OptionalParameters_New1()
        {
            var f1 = Compile<Func<int>>("() => new NamedAndOptionalParameters().Value");
            f1();

            var f2 = Compile<Func<int>>("() => new NamedAndOptionalParameters(Return(43)).Value");
            f2();

            var f3 = Compile<Func<int>>("() => new NamedAndOptionalParameters(Return(43), Return(\"foobar\")).Value");
            f3();

            var f4 = Compile<Func<int>>("() => new NamedAndOptionalParameters(Return(43), Return(\"foobar\"), Return(false)).Value");
            f4();
        }

        [TestMethod]
        public void CrossCheck_OptionalParameters_New2()
        {
            var f1 = Compile<Func<int>>("() => new NamedAndOptionalParameters(Return(43), y: Return(\"foobar\")).Value");
            f1();

            var f2 = Compile<Func<int>>("() => new NamedAndOptionalParameters(Return(43), z: Return(false)).Value");
            f2();

            var f3 = Compile<Func<int>>("() => new NamedAndOptionalParameters(Return(43), Return(\"foobar\"), z: Return(false)).Value");
            f3();
        }

        [TestMethod]
        public void CrossCheck_OptionalParameters_Index1()
        {
            // NB: Need at least one argument for an indexer.

            var f2 = Compile<Func<int>>("() => new IndexerWithNamedAndOptionalParameters()[Return(43)]");
            f2();

            var f3 = Compile<Func<int>>("() => new IndexerWithNamedAndOptionalParameters()[Return(43), Return(\"foobar\")]");
            f3();

            var f4 = Compile<Func<int>>("() => new IndexerWithNamedAndOptionalParameters()[Return(43), Return(\"foobar\"), Return(false)]");
            f4();
        }

        [TestMethod]
        public void CrossCheck_OptionalParameters_Index2()
        {
            var f1 = Compile<Func<int>>("() => new IndexerWithNamedAndOptionalParameters()[Return(43), y: Return(\"foobar\")]");
            f1();

            var f2 = Compile<Func<int>>("() => new IndexerWithNamedAndOptionalParameters()[Return(43), z: Return(false)]");
            f2();

            var f3 = Compile<Func<int>>("() => new IndexerWithNamedAndOptionalParameters()[Return(43), Return(\"foobar\"), z: Return(false)]");
            f3();
        }

        [TestMethod]
        public void CrossCheck_OptionalParameters_Invoke1()
        {
            var d = new DelegateWithNamedAndOptionalParameters((x, y, z) => x + y.Length + (z ? 1 : 0));

            var f1 = Compile<Func<DelegateWithNamedAndOptionalParameters, int>>("f => f()");
            f1(d);

            var f2 = Compile<Func<DelegateWithNamedAndOptionalParameters, int>>("f => f(Return(43))");
            f2(d);

            var f3 = Compile<Func<DelegateWithNamedAndOptionalParameters, int>>("f => f(Return(43), Return(\"foobar\"))");
            f3(d);

            var f4 = Compile<Func<DelegateWithNamedAndOptionalParameters, int>>("f => f(Return(43), Return(\"foobar\"), Return(false))");
            f4(d);
        }

        [TestMethod]
        public void CrossCheck_OptionalParameters_Invoke2()
        {
            var d = new DelegateWithNamedAndOptionalParameters((x, y, z) => x + y.Length + (z ? 1 : 0));

            var f1 = Compile<Func<DelegateWithNamedAndOptionalParameters, int>>("f => f(Return(43), y: Return(\"foobar\"))");
            f1(d);

            var f2 = Compile<Func<DelegateWithNamedAndOptionalParameters, int>>("f => f(Return(43), z: Return(false))");
            f2(d);

            var f3 = Compile<Func<DelegateWithNamedAndOptionalParameters, int>>("f => f(Return(43), Return(\"foobar\"), z: Return(false))");
            f3(d);
        }

        #endregion
    }
}
