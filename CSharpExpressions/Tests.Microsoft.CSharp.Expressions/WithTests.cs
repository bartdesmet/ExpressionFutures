// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using Microsoft.CSharp.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace Tests
{
    public class WithTests
    {
        [Fact]
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

        [Fact]
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

        [Fact]
        public void With_Update()
        {
            var point1 = Expression.Parameter(typeof(Point), "p");

            var x1 = CSharpExpression.MemberInitializer(typeof(Point).GetProperty(nameof(Point.X)), Expression.Constant(1));
            var y1 = CSharpExpression.MemberInitializer(typeof(Point).GetProperty(nameof(Point.Y)), Expression.Constant(2));
            var pointInits1 = new[] { x1, y1 };

            var e1 = CSharpExpression.With(point1, clone: null, pointInits1);
            Assert.Null(e1.Members);

            var e2 = e1.Update(point1, new[] { x1, y1 });
            Assert.Same(e1, e2);
            Assert.Null(e2.Members);

            var point2 = Expression.Parameter(typeof(Point), "p");

            var x2 = CSharpExpression.MemberInitializer(typeof(Point).GetProperty(nameof(Point.X)), Expression.Constant(3));
            var y2 = CSharpExpression.MemberInitializer(typeof(Point).GetProperty(nameof(Point.Y)), Expression.Constant(4));
            var pointInits2 = new[] { x2, y2 };

            var e3 = e1.Update(point2, e1.Initializers);
            Assert.NotSame(e1, e3);
            Assert.Same(point2, e3.Object);
            Assert.Same(e1.Initializers, e3.Initializers);
            Assert.Null(e3.Members);

            var e4 = e1.Update(e1.Object, pointInits2);
            Assert.NotSame(e1, e4);
            Assert.Same(e1.Object, e4.Object);
            Assert.True(pointInits2.SequenceEqual(e4.Initializers));
            Assert.Null(e4.Members);
        }

        [Fact]
        public void With_Update_AnonymousType()
        {
            var obj = new { Name = "Bart", Age = 21 };

            var nameProperty = obj.GetType().GetProperty(nameof(obj.Name));
            var ageProperty = obj.GetType().GetProperty(nameof(obj.Age));

            var p1 = Expression.Parameter(obj.GetType(), "p");

            var name1 = CSharpExpression.MemberInitializer(nameProperty, Expression.Constant("Bart"));
            var age1 = CSharpExpression.MemberInitializer(ageProperty, Expression.Constant(21));
            var personInits1 = new[] { name1, age1 };

            var members = new MemberInfo[] { nameProperty, ageProperty };

            var e1 = CSharpExpression.With(p1, members, personInits1);
            Assert.True(members.SequenceEqual(e1.Members));

            var e2 = e1.Update(p1, new[] { name1, age1 });
            Assert.Same(e1, e2);
            Assert.True(members.SequenceEqual(e2.Members));

            var p2 = Expression.Parameter(obj.GetType(), "p");

            var name2 = CSharpExpression.MemberInitializer(nameProperty, Expression.Constant("Homer"));
            var age2 = CSharpExpression.MemberInitializer(ageProperty, Expression.Constant(42));
            var personInits2 = new[] { name2, age2 };

            var e3 = e1.Update(p2, e1.Initializers);
            Assert.NotSame(e1, e3);
            Assert.Same(p2, e3.Object);
            Assert.Same(e1.Initializers, e3.Initializers);
            Assert.True(members.SequenceEqual(e3.Members));

            var e4 = e1.Update(e1.Object, personInits2);
            Assert.NotSame(e1, e4);
            Assert.Same(e1.Object, e4.Object);
            Assert.True(personInits2.SequenceEqual(e4.Initializers));
            Assert.True(members.SequenceEqual(e4.Members));
        }

        [Fact]
        public void With_Visitor()
        {
            var x = CSharpExpression.MemberInitializer(typeof(Point).GetProperty(nameof(Point.X)), Expression.Constant(1));
            var y = CSharpExpression.MemberInitializer(typeof(Point).GetProperty(nameof(Point.Y)), Expression.Constant(2));
            var pointInits = new[] { x, y };

            var point = Expression.Parameter(typeof(Point), "p");

            var res = CSharpExpression.With(point, clone: null, pointInits);

            var v = new V();
            Assert.Same(res, v.Visit(res));
            Assert.True(v.Visited);
        }

        class V : CSharpExpressionVisitor
        {
            public bool Visited = false;

            protected internal override Expression VisitWith(WithCSharpExpression node)
            {
                Visited = true;

                return base.VisitWith(node);
            }
        }

        [Fact]
        public void With_Compile_ClassWithClone()
        {
            var person = Expression.Parameter(typeof(Person), "p");

            var name = CSharpExpression.MemberInitializer(typeof(Person).GetProperty(nameof(PersonNoClone.Name)), Expression.Constant("Bart"));
            var with_name = CSharpExpression.With(person, name);
            var f_name = Expression.Lambda<Func<Person, Person>>(with_name, person).Compile();
            var res_name = f_name(new Person { Name = "Abraham", Age = 84 });
            Assert.Equal("Bart", res_name.Name);
            Assert.Equal(84, res_name.Age);

            var age = CSharpExpression.MemberInitializer(typeof(Person).GetProperty(nameof(PersonNoClone.Age)), Expression.Constant(21));
            var with_age = CSharpExpression.With(person, age);
            var f_age = Expression.Lambda<Func<Person, Person>>(with_age, person).Compile();
            var res_age = f_age(res_name);
            Assert.Equal("Bart", res_age.Name);
            Assert.Equal(21, res_age.Age);

            var with_both = CSharpExpression.With(person, name, age);
            var f_both = Expression.Lambda<Func<Person, Person>>(with_both, person).Compile();
            var res_both = f_both(new Person { Name = "Abraham", Age = 84 });
            Assert.Equal("Bart", res_both.Name);
            Assert.Equal(21, res_both.Age);
        }

        [Fact]
        public void With_Compile_Struct()
        {
            var point = Expression.Parameter(typeof(Point), "p");

            var x = CSharpExpression.MemberInitializer(typeof(Point).GetProperty(nameof(Point.X)), Expression.Constant(1));
            var with_x = CSharpExpression.With(point, x);
            var f_x = Expression.Lambda<Func<Point, Point>>(with_x, point).Compile();
            var res_x = f_x(new Point { X = -1, Y = -2 });
            Assert.Equal(1, res_x.X);
            Assert.Equal(-2, res_x.Y);

            var y = CSharpExpression.MemberInitializer(typeof(Point).GetProperty(nameof(Point.Y)), Expression.Constant(2));
            var with_y = CSharpExpression.With(point, y);
            var f_y = Expression.Lambda<Func<Point, Point>>(with_y, point).Compile();
            var res_y = f_y(res_x);
            Assert.Equal(1, res_y.X);
            Assert.Equal(2, res_y.Y);

            var with_both = CSharpExpression.With(point, x, y);
            var f_both = Expression.Lambda<Func<Point, Point>>(with_both, point).Compile();
            var res_both = f_both(new Point { X = -1, Y = -2 });
            Assert.Equal(1, res_both.X);
            Assert.Equal(2, res_both.Y);
        }

        [Fact]
        public void With_Compile_AnonymousType()
        {
            var obj = new { Name = "Abraham", Age = 84 };

            var nameProperty = obj.GetType().GetProperty(nameof(obj.Name));
            var ageProperty = obj.GetType().GetProperty(nameof(obj.Age));

            var members = new MemberInfo[] { nameProperty, ageProperty };

            var person = Expression.Parameter(obj.GetType(), "p");

            var name = CSharpExpression.MemberInitializer(nameProperty, Expression.Constant("Bart"));
            var with_name = CSharpExpression.With(person, members, name);
            var f_name = Expression.Lambda(with_name, person).Compile();
            var res_name = (dynamic)f_name.DynamicInvoke(obj);
            Assert.Equal("Bart", (string)res_name.Name);
            Assert.Equal(84, (int)res_name.Age);

            var age = CSharpExpression.MemberInitializer(ageProperty, Expression.Constant(21));
            var with_age = CSharpExpression.With(person, members, age);
            var f_age = Expression.Lambda(with_age, person).Compile();
            var res_age = (dynamic)f_age.DynamicInvoke(res_name);
            Assert.Equal("Bart", (string)res_age.Name);
            Assert.Equal(21, (int)res_age.Age);

            var with_both = CSharpExpression.With(person, members, name, age);
            var f_both = Expression.Lambda(with_both, person).Compile();
            var res_both = (dynamic)f_both.DynamicInvoke(obj);
            Assert.Equal("Bart", (string)res_both.Name);
            Assert.Equal(21, (int)res_both.Age);
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

#pragma warning disable CS0649
        class Foo
        {
            public readonly int Bar;
            public int Qux { get; }

            public Foo Clone() => throw new Exception();
        }
#pragma warning restore CS0649
    }
}
