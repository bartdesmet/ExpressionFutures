// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using Microsoft.CSharp.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Tests
{
    [TestClass]
    public class WithTests
    {
        [TestMethod]
        public void With_Factory_ArgumentChecking()
        {
            // null checks - object
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.With(@object: null, default(MemberInitializer[])));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.With(@object: null, default(IEnumerable<MemberInitializer>)));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.With(@object: null, clone: null, default(MemberInitializer[])));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.With(@object: null, clone: null, default(IEnumerable<MemberInitializer>)));

            var person = Expression.Parameter(typeof(Person), "p");

            // null checks - initializers
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.With(person, default(MemberInitializer[])));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.With(person, default(IEnumerable<MemberInitializer>)));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.With(person, clone: null, default(MemberInitializer[])));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.With(person, clone: null, default(IEnumerable<MemberInitializer>)));

            var name = CSharpExpression.MemberInitializer(typeof(PersonNoClone).GetProperty(nameof(PersonNoClone.Name)), Expression.Constant("Bart"));
            var age = CSharpExpression.MemberInitializer(typeof(PersonNoClone).GetProperty(nameof(PersonNoClone.Age)), Expression.Constant(21));
            var personInits = new[] { name, age };

            var personNoClone = Expression.Parameter(typeof(PersonNoClone), "q");

            // no clone method
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.With(personNoClone, clone: null, personInits));

            // invalid clone method
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.With(person, typeof(Person).GetMethod(nameof(Person.StaticClone)), personInits));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.With(person, typeof(Person).GetMethod(nameof(Person.GenericClone)), personInits));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.With(person, typeof(Person).GetMethod(nameof(Person.CloneOneArg)), personInits));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.With(person, typeof(Person).GetMethod(nameof(Person.CloneInvalidReturn)), personInits));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.With(person, typeof(Point).GetMethod(nameof(Point.Clone)), personInits));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.With(person, typeof(object).GetMethod(nameof(object.ToString)), personInits));

            var x = CSharpExpression.MemberInitializer(typeof(Point).GetProperty(nameof(Point.X)), Expression.Constant(1));
            var y = CSharpExpression.MemberInitializer(typeof(Point).GetProperty(nameof(Point.Y)), Expression.Constant(2));
            var pointInits = new[] { x, y };

            var point = Expression.Parameter(typeof(Point), "p");

            // no clone for structs
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.With(point, typeof(Point).GetMethod(nameof(Point.Clone)), pointInits));

            // invalid member
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.With(person, typeof(Person).GetMethod(nameof(Person.Clone)), pointInits));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.With(point, clone: null, personInits));

            var foo = Expression.Parameter(typeof(Foo), "f");
            var bar = CSharpExpression.MemberInitializer(typeof(Foo).GetField(nameof(Foo.Bar)), Expression.Constant(1));
            var qux = CSharpExpression.MemberInitializer(typeof(Foo).GetProperty(nameof(Foo.Qux)), Expression.Constant(1));

            // non-writeable member
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.With(foo, clone: null, bar));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.With(foo, clone: null, qux));

            // valid cases for sanity
            _ = CSharpExpression.With(person, typeof(Person).GetMethod(nameof(Person.Clone)), personInits);
            _ = CSharpExpression.With(person, typeof(Person).GetMethod(nameof(Person.Clone)), name);
            _ = CSharpExpression.With(person, typeof(Person).GetMethod(nameof(Person.Clone)), age);
            _ = CSharpExpression.With(point, clone: null, pointInits);
            _ = CSharpExpression.With(point, clone: null, x);
            _ = CSharpExpression.With(point, clone: null, y);
        }

        [TestMethod]
        public void With_Factory_ArgumentChecking_AnonymousType()
        {
            var obj = new { Name = "Bart", Age = 21 };

            var nameProperty = obj.GetType().GetProperty(nameof(obj.Name));
            var ageProperty = obj.GetType().GetProperty(nameof(obj.Age));

            var p = Expression.Parameter(obj.GetType(), "p");

            var name = CSharpExpression.MemberInitializer(nameProperty, Expression.Constant("Bart"));
            var age = CSharpExpression.MemberInitializer(ageProperty, Expression.Constant(21));
            var personInits = new[] { name, age };

            var members = new MemberInfo[] { nameProperty, ageProperty };

            // null checks - object
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.With(@object: null, members: null, default(MemberInitializer[])));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.With(@object: null, members: null, default(IEnumerable<MemberInitializer>)));

            // null checks - members
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.With(p, members: null, default(MemberInitializer[])));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.With(p, members: null, default(IEnumerable<MemberInitializer>)));

            // null checks - initializers
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.With(p, members, default(MemberInitializer[])));
            AssertEx.Throws<ArgumentNullException>(() => CSharpExpression.With(p, members, default(IEnumerable<MemberInitializer>)));

            // incorrect members
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.With(p, new MemberInfo[] { nameProperty }, personInits));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.With(p, new MemberInfo[] { ageProperty }, personInits));
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.With(p, new MemberInfo[] { ageProperty, nameProperty }, personInits));

            // invalid member
            AssertEx.Throws<ArgumentException>(() => CSharpExpression.With(p, new MemberInfo[] { typeof(Person).GetProperty(nameof(Person.Name)) }, personInits));

            // valid cases for sanity
            _ = CSharpExpression.With(p, members, name);
            _ = CSharpExpression.With(p, members, age);
            _ = CSharpExpression.With(p, members, personInits);
        }

        class PersonNoClone
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        class Person : PersonNoClone
        {
            public Person Clone() => new Person { Name = Name, Age = Age };

            public static Person StaticClone() => throw new Exception();
            public Person GenericClone<T>() => throw new Exception();
            public Person CloneOneArg(string name) => throw new Exception();
            public int CloneInvalidReturn() => throw new Exception();
        }

        struct Point
        {
            public int X { get; set; }
            public int Y { get; set; }

            public Point Clone() => this;
        }

        class Foo
        {
            public readonly int Bar;
            public int Qux { get; }

            public Foo Clone() => throw new Exception();
        }
    }
}
