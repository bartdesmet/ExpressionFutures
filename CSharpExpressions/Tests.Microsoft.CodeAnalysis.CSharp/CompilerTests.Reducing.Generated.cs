﻿// Prototyping extended expression trees for C#.
//
// bartde - December 2015

// NB: Running these tests can take a *VERY LONG* time because it invokes the C# compiler for every test
//     case in order to obtain an expression tree object. Be patient when running these tests.

// NB: These tests are generated from a list of expressions in the .tt file by invoking the C# compiler at
//     text template processing time by the T4 engine. See TestUtilities for the helper functions that call
//     into the compiler, load the generated assembly, extract the Expression objects through reflection on
//     the generated type, and call DebugView() on those. The resulting DebugView string is emitted in this
//     file as `expected` variables. The original expression is escaped and gets passed ot the GetDebugView
//     helper method to obtain `actual`, which causes the C# compiler to run at test execution time, using
//     the same helper library, thus obtaining the DebugView string again. This serves a number of goals:
//
//       1. At test generation time, a custom Roslyn build can be invoked to test the implicit conversion
//          of a lambda expression to an expression tree, which involves the changes made to support the
//          C# expression library in this solution. Any fatal compiler errors will come out at that time.
//
//       2. Reflection on the properties in the emitted class causes a deferred execution of the factory
//          method calls generated by the Roslyn compiler for the implicit conversion of the lambda to an
//          expression tree. Any exceptions thrown by the factory methods will show up as well during test
//          generation time, allowing issues to be uncovered.
//
//       3. The string literals in the `expected` variables are inspectable by a human to assert that the
//          compiler has indeed generated an expression representation that's homo-iconic to the original
//          expression that was provided in the test.
//
//       4. Any changes to the compiler or the runtime library could cause regressions. Because template
//          processing of the T4 only takes place upon editing the .tt file, the generated test code won't
//          change. As such, any regression can cause test failures which allows to detect any changes to
//          compiler or runtime library behavior.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Microsoft.CodeAnalysis.CSharp.TestUtilities;

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests
    {
        [TestMethod]
        public void CompilerTest_6A97_3CC7()
        {
            // (Expression<Func<int, int>>)(x => System.Threading.Interlocked.Exchange(value: int.Parse("1"), location1: ref x))
            var actual = ToCSharp(@"(Expression<Func<int, int>>)(x => System.Threading.Interlocked.Exchange(value: int.Parse(""1""), location1: ref x))", reduce: true);
            var expected = @"
(int x) =>
{
    int value;
    value = int.Parse(""1"");
    return System.Threading.Interlocked.Exchange(ref x, value);
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_6A97_3CC7();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_6A97_3CC7() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_BFB2_9E66()
        {
            // (Expression<Func<StrongBox<int>, int>>)(b => System.Threading.Interlocked.Exchange(value: int.Parse("1"), location1: ref b.Value))
            var actual = ToCSharp(@"(Expression<Func<StrongBox<int>, int>>)(b => System.Threading.Interlocked.Exchange(value: int.Parse(""1""), location1: ref b.Value))", reduce: true);
            var expected = @"
(System.Runtime.CompilerServices.StrongBox<int> b) =>
{
    int value;
    System.Runtime.CompilerServices.StrongBox<int> __object;
    value = int.Parse(""1"");
    __object = b;
    __object.Value;
    return System.Threading.Interlocked.Exchange(ref __object.Value, value);
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_BFB2_9E66();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_BFB2_9E66() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_4609_D626()
        {
            // (Expression<Func<int[], int>>)(xs => System.Threading.Interlocked.Exchange(value: int.Parse("1"), location1: ref xs[int.Parse("0")]))
            var actual = ToCSharp(@"(Expression<Func<int[], int>>)(xs => System.Threading.Interlocked.Exchange(value: int.Parse(""1""), location1: ref xs[int.Parse(""0"")]))", reduce: true);
            var expected = @"
(int[] xs) =>
{
    int value, __index;
    int[] __array;
    value = int.Parse(""1"");
    __array = xs;
    __index = int.Parse(""0"");
    __array[__index];
    return System.Threading.Interlocked.Exchange(ref __array[__index], value);
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_4609_D626();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_4609_D626() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_579D_8D67()
        {
            // (Expression<Action<int>>)(x => { x = 1; })
            var actual = ToCSharp(@"(Expression<Action<int>>)(x => { x = 1; })", reduce: true);
            var expected = @"
(int x) =>
{
    x = 1;
    L0 /*(null)*/:
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_579D_8D67();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_579D_8D67() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_93F1_077C()
        {
            // (Expression<Action<int[]>>)(xs => { xs[0] = 1; })
            var actual = ToCSharp(@"(Expression<Action<int[]>>)(xs => { xs[0] = 1; })", reduce: true);
            var expected = @"
(int[] xs) =>
{
    xs[0] = 1;
    L0 /*(null)*/:
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_93F1_077C();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_93F1_077C() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_2F3A_1F59()
        {
            // (Expression<Action<int[,]>>)(xs => { xs[0, 0] = 1; })
            var actual = ToCSharp(@"(Expression<Action<int[,]>>)(xs => { xs[0, 0] = 1; })", reduce: true);
            var expected = @"
(int[,] xs) =>
{
    xs[0, 0] = 1;
    L0 /*(null)*/:
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_2F3A_1F59();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_2F3A_1F59() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_430E_BB89()
        {
            // (Expression<Action<List<int>>>)(xs => { xs[0] = 1; })
            var actual = ToCSharp(@"(Expression<Action<List<int>>>)(xs => { xs[0] = 1; })", reduce: true);
            var expected = @"
(List<int> xs) =>
{
    {
        List<int> __obj;
        __obj = xs;
        /*return*/ __obj[0] = 1/*;*/
    }
    L0 /*(null)*/:
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_430E_BB89();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_430E_BB89() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_AEF8_094C()
        {
            // (Expression<Action<StrongBox<int>>>)(b => { b.Value = 1; })
            var actual = ToCSharp(@"(Expression<Action<StrongBox<int>>>)(b => { b.Value = 1; })", reduce: true);
            var expected = @"
(System.Runtime.CompilerServices.StrongBox<int> b) =>
{
    b.Value = 1;
    L0 /*(null)*/:
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_AEF8_094C();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_AEF8_094C() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_044F_5A03()
        {
            // (Expression<Action<int>>)(x => { x += 1; })
            var actual = ToCSharp(@"(Expression<Action<int>>)(x => { x += 1; })", reduce: true);
            var expected = @"
(int x) =>
{
    x = x + 1;
    L0 /*(null)*/:
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_044F_5A03();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_044F_5A03() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_E0C9_816B()
        {
            // (Expression<Action<int[]>>)(xs => { xs[0] += 1; })
            var actual = ToCSharp(@"(Expression<Action<int[]>>)(xs => { xs[0] += 1; })", reduce: true);
            var expected = @"
(int[] xs) =>
{
    {
        int[] __object;
        __object = xs;
        /*return*/ __object[0] = __object[0] + 1/*;*/
    }
    L0 /*(null)*/:
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_E0C9_816B();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_E0C9_816B() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_7C94_EEF3()
        {
            // (Expression<Action<int[,]>>)(xs => { xs[0, 0] += 1; })
            var actual = ToCSharp(@"(Expression<Action<int[,]>>)(xs => { xs[0, 0] += 1; })", reduce: true);
            var expected = @"
(int[,] xs) =>
{
    {
        int[,] __object;
        __object = xs;
        /*return*/ __object[0, 0] = __object[0, 0] + 1/*;*/
    }
    L0 /*(null)*/:
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_7C94_EEF3();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_7C94_EEF3() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_79AE_F9F6()
        {
            // (Expression<Action<List<int>>>)(xs => { xs[0] += 1; })
            var actual = ToCSharp(@"(Expression<Action<List<int>>>)(xs => { xs[0] += 1; })", reduce: true);
            var expected = @"
(List<int> xs) =>
{
    {
        List<int> __obj;
        __obj = xs;
        /*return*/ __obj[0] = __obj[0] + 1/*;*/
    }
    L0 /*(null)*/:
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_79AE_F9F6();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_79AE_F9F6() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_08DA_705B()
        {
            // (Expression<Action<StrongBox<int>>>)(b => { b.Value += 1; })
            var actual = ToCSharp(@"(Expression<Action<StrongBox<int>>>)(b => { b.Value += 1; })", reduce: true);
            var expected = @"
(System.Runtime.CompilerServices.StrongBox<int> b) =>
{
    {
        System.Runtime.CompilerServices.StrongBox<int> __lhs;
        __lhs = b;
        /*return*/ __lhs.Value = __lhs.Value + 1/*;*/
    }
    L0 /*(null)*/:
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_08DA_705B();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_08DA_705B() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_2115_5A03()
        {
            // (Expression<Action<int>>)(x => { ++x; })
            var actual = ToCSharp(@"(Expression<Action<int>>)(x => { ++x; })", reduce: true);
            var expected = @"
(int x) =>
{
    x = x + 1;
    L0 /*(null)*/:
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_2115_5A03();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_2115_5A03() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_89F1_816B()
        {
            // (Expression<Action<int[]>>)(xs => { ++xs[0]; })
            var actual = ToCSharp(@"(Expression<Action<int[]>>)(xs => { ++xs[0]; })", reduce: true);
            var expected = @"
(int[] xs) =>
{
    {
        int[] __object;
        __object = xs;
        /*return*/ __object[0] = __object[0] + 1/*;*/
    }
    L0 /*(null)*/:
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_89F1_816B();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_89F1_816B() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_205F_EEF3()
        {
            // (Expression<Action<int[,]>>)(xs => { ++xs[0, 0]; })
            var actual = ToCSharp(@"(Expression<Action<int[,]>>)(xs => { ++xs[0, 0]; })", reduce: true);
            var expected = @"
(int[,] xs) =>
{
    {
        int[,] __object;
        __object = xs;
        /*return*/ __object[0, 0] = __object[0, 0] + 1/*;*/
    }
    L0 /*(null)*/:
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_205F_EEF3();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_205F_EEF3() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_FB63_F9F6()
        {
            // (Expression<Action<List<int>>>)(xs => { ++xs[0]; })
            var actual = ToCSharp(@"(Expression<Action<List<int>>>)(xs => { ++xs[0]; })", reduce: true);
            var expected = @"
(List<int> xs) =>
{
    {
        List<int> __obj;
        __obj = xs;
        /*return*/ __obj[0] = __obj[0] + 1/*;*/
    }
    L0 /*(null)*/:
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_FB63_F9F6();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_FB63_F9F6() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_30FE_705B()
        {
            // (Expression<Action<StrongBox<int>>>)(b => { ++b.Value; })
            var actual = ToCSharp(@"(Expression<Action<StrongBox<int>>>)(b => { ++b.Value; })", reduce: true);
            var expected = @"
(System.Runtime.CompilerServices.StrongBox<int> b) =>
{
    {
        System.Runtime.CompilerServices.StrongBox<int> __lhs;
        __lhs = b;
        /*return*/ __lhs.Value = __lhs.Value + 1/*;*/
    }
    L0 /*(null)*/:
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_30FE_705B();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_30FE_705B() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_00CF_4CB4()
        {
            // (Expression<Action<int>>)(x => { x++; })
            var actual = ToCSharp(@"(Expression<Action<int>>)(x => { x++; })", reduce: true);
            var expected = @"
(int x) =>
{
    {
        int __temp;
        __temp = x;
        x = __temp + 1;
        /*return*/ __temp/*;*/
    }
    L0 /*(null)*/:
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_00CF_4CB4();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_00CF_4CB4() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_FE9D_1C3B()
        {
            // (Expression<Action<int[]>>)(xs => { xs[0]++; })
            var actual = ToCSharp(@"(Expression<Action<int[]>>)(xs => { xs[0]++; })", reduce: true);
            var expected = @"
(int[] xs) =>
{
    {
        int[] __object;
        int __index;
        __object = xs;
        __index = __object[0];
        __object[0] = __index + 1;
        /*return*/ __index/*;*/
    }
    L0 /*(null)*/:
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_FE9D_1C3B();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_FE9D_1C3B() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_BB88_FBEA()
        {
            // (Expression<Action<int[,]>>)(xs => { xs[0, 0]++; })
            var actual = ToCSharp(@"(Expression<Action<int[,]>>)(xs => { xs[0, 0]++; })", reduce: true);
            var expected = @"
(int[,] xs) =>
{
    {
        int[,] __object;
        int __index;
        __object = xs;
        __index = __object[0, 0];
        __object[0, 0] = __index + 1;
        /*return*/ __index/*;*/
    }
    L0 /*(null)*/:
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_BB88_FBEA();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_BB88_FBEA() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_55F5_73B2()
        {
            // (Expression<Action<List<int>>>)(xs => { xs[0]++; })
            var actual = ToCSharp(@"(Expression<Action<List<int>>>)(xs => { xs[0]++; })", reduce: true);
            var expected = @"
(List<int> xs) =>
{
    {
        List<int> __obj;
        __obj = xs;
        /*return*/ {
            int __temp;
            __temp = __obj[0];
            __obj[0] = __temp + 1;
            /*return*/ __temp/*;*/
        }/*;*/
    }
    L0 /*(null)*/:
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_55F5_73B2();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_55F5_73B2() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_0564_D471()
        {
            // (Expression<Action<StrongBox<int>>>)(b => { b.Value++; })
            var actual = ToCSharp(@"(Expression<Action<StrongBox<int>>>)(b => { b.Value++; })", reduce: true);
            var expected = @"
(System.Runtime.CompilerServices.StrongBox<int> b) =>
{
    {
        System.Runtime.CompilerServices.StrongBox<int> __lhs;
        int __temp;
        __lhs = b;
        __temp = __lhs.Value;
        __lhs.Value = __temp + 1;
        /*return*/ __temp/*;*/
    }
    L0 /*(null)*/:
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_0564_D471();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_0564_D471() => INCONCLUSIVE(); }

        [TestMethod]
        public void CompilerTest_9551_2A52()
        {
            // (Expression<Action<object>>)(l => { lock(l) { Console.WriteLine("In lock"); } })
            var actual = ToCSharp(@"(Expression<Action<object>>)(l => { lock(l) { Console.WriteLine(""In lock""); } })", reduce: true);
            var expected = @"
(object l) =>
{
    {
        bool __lockWasTaken;
        object __lock;
        __lockWasTaken = false;
        __lock = l;
        try
        {
            System.Threading.Monitor.Enter(__lock, ref __lockWasTaken);
            {
                Console.WriteLine(""In lock"");
            }
        }
        finally
        {
            if (__lockWasTaken)
                System.Threading.Monitor.Exit(__lock);
        }
    }
    L0 /*(null)*/:
}";
            Assert.AreEqual(expected.TrimStart('\r', '\n'), actual);
            Verify.CompilerTest_9551_2A52();
        }

        partial class Review { /* override in .Verify.cs */ public virtual void CompilerTest_9551_2A52() => INCONCLUSIVE(); }

    }

/*
// NB: The code generated below accepts all tests. *DON'T* just copy/paste this to the .Verify.cs file
//     but review the tests one by one. This output is included in case a minor change is made to debug
//     output produced by DebugView() and all hashes are invalidated. In that case, this output can be
//     copied and pasted into .Verify.cs.

namespace Tests.Microsoft.CodeAnalysis.CSharp
{
    partial class CompilerTests
    {
        partial class Reviewed
        {
            public override void CompilerTest_6A97_3CC7() => OK();
            public override void CompilerTest_BFB2_9E66() => OK();
            public override void CompilerTest_4609_D626() => OK();
            public override void CompilerTest_579D_8D67() => OK();
            public override void CompilerTest_93F1_077C() => OK();
            public override void CompilerTest_2F3A_1F59() => OK();
            public override void CompilerTest_430E_BB89() => OK();
            public override void CompilerTest_AEF8_094C() => OK();
            public override void CompilerTest_044F_5A03() => OK();
            public override void CompilerTest_E0C9_816B() => OK();
            public override void CompilerTest_7C94_EEF3() => OK();
            public override void CompilerTest_79AE_F9F6() => OK();
            public override void CompilerTest_08DA_705B() => OK();
            public override void CompilerTest_2115_5A03() => OK();
            public override void CompilerTest_89F1_816B() => OK();
            public override void CompilerTest_205F_EEF3() => OK();
            public override void CompilerTest_FB63_F9F6() => OK();
            public override void CompilerTest_30FE_705B() => OK();
            public override void CompilerTest_00CF_4CB4() => OK();
            public override void CompilerTest_FE9D_1C3B() => OK();
            public override void CompilerTest_BB88_FBEA() => OK();
            public override void CompilerTest_55F5_73B2() => OK();
            public override void CompilerTest_0564_D471() => OK();
            public override void CompilerTest_9551_2A52() => OK();
        }
    }
}
*/
}
