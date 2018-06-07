//
// Examples of the expression types feature discussed at
// https://github.com/bartdesmet/roslyn/blob/ExpressionTreeLikeTypes/docs/features/expression-types.md
//
// bartde - June 2018
//

using System.Collections.Generic;
using System.Reflection;

namespace System.Linq.Expressions
{
    /// <summary>
    /// Documents the factory methods the compiler currently binds to.
    /// </summary>
    public static class ExpressionFactory
    {
        public static BinaryExpression Add(Expression left, Expression right) => Expression.Add(left, right);
        public static BinaryExpression Add(Expression left, Expression right, MethodInfo method) => Expression.Add(left, right, method);
        public static BinaryExpression AddChecked(Expression left, Expression right) => Expression.AddChecked(left, right);
        public static BinaryExpression AddChecked(Expression left, Expression right, MethodInfo method) => Expression.AddChecked(left, right, method);

        public static BinaryExpression And(Expression left, Expression right) => Expression.And(left, right);
        public static BinaryExpression And(Expression left, Expression right, MethodInfo method) => Expression.And(left, right, method);

        public static BinaryExpression AndAlso(Expression left, Expression right) => Expression.AndAlso(left, right);
        public static BinaryExpression AndAlso(Expression left, Expression right, MethodInfo method) => Expression.AndAlso(left, right, method);

        public static BinaryExpression ArrayIndex(Expression array, Expression index) => Expression.ArrayIndex(array, index);
        public static MethodCallExpression ArrayIndex(Expression array, Expression[] indexes) => Expression.ArrayIndex(array, indexes);

        public static UnaryExpression ArrayLength(Expression array) => Expression.ArrayLength(array);

        public static MemberAssignment Bind(MemberInfo member, Expression expression) => Expression.Bind(member, expression); // NB: Used for fields.
        public static MemberAssignment Bind(MethodInfo propertyAccessor, Expression expression) => Expression.Bind(propertyAccessor, expression); // NB: Used for properties.

        public static MethodCallExpression Call(Expression instance, MethodInfo method, Expression[] arguments) => Expression.Call(instance, method, arguments);

        public static BinaryExpression Coalesce(Expression left, Expression right) => Expression.Coalesce(left, right);
        public static BinaryExpression Coalesce(Expression left, Expression right, LambdaExpression conversion) => Expression.Coalesce(left, right, conversion);

        public static ConditionalExpression Condition(Expression test, Expression ifTrue, Expression ifFalse) => Expression.Condition(test, ifTrue, ifFalse);

        public static ConstantExpression Constant(object value, Type type) => Expression.Constant(value, type);

        public static UnaryExpression Convert(Expression expression, Type type) => Expression.Convert(expression, type);
        public static UnaryExpression Convert(Expression expression, Type type, MethodInfo method) => Expression.Convert(expression, type, method);
        public static UnaryExpression ConvertChecked(Expression expression, Type type) => Expression.ConvertChecked(expression, type); // NB: Never binds to overload with MethodInfo parameter.

        public static BinaryExpression Divide(Expression left, Expression right) => Expression.Divide(left, right);
        public static BinaryExpression Divide(Expression left, Expression right, MethodInfo method) => Expression.Divide(left, right, method);

        public static ElementInit ElementInit(MethodInfo addMethod, Expression[] arguments) => Expression.ElementInit(addMethod, arguments);

        public static BinaryExpression Equal(Expression left, Expression right) => Expression.Equal(left, right);
        public static BinaryExpression Equal(Expression left, Expression right, bool liftToNull, MethodInfo method) => Expression.Equal(left, right, liftToNull, method);

        public static BinaryExpression ExclusiveOr(Expression left, Expression right) => Expression.ExclusiveOr(left, right);
        public static BinaryExpression ExclusiveOr(Expression left, Expression right, MethodInfo method) => Expression.ExclusiveOr(left, right, method);

        public static MemberExpression Field(Expression expression, FieldInfo field) => Expression.Field(expression, field);

        public static BinaryExpression GreaterThan(Expression left, Expression right) => Expression.GreaterThan(left, right);
        public static BinaryExpression GreaterThan(Expression left, Expression right, bool liftToNull, MethodInfo method) => Expression.GreaterThan(left, right, liftToNull, method);

        public static BinaryExpression GreaterThanOrEqual(Expression left, Expression right) => Expression.GreaterThanOrEqual(left, right);
        public static BinaryExpression GreaterThanOrEqual(Expression left, Expression right, bool liftToNull, MethodInfo method) => Expression.GreaterThanOrEqual(left, right, liftToNull, method);

        public static InvocationExpression Invoke(Expression expression, Expression[] arguments) => Expression.Invoke(expression, arguments);

        public static Expression<T> Lambda<T>(Expression body, ParameterExpression[] parameters) => Expression.Lambda<T>(body, parameters);

        public static BinaryExpression LeftShift(Expression left, Expression right) => Expression.LeftShift(left, right);
        public static BinaryExpression LeftShift(Expression left, Expression right, MethodInfo method) => Expression.LeftShift(left, right, method);

        public static BinaryExpression LessThan(Expression left, Expression right) => Expression.LessThan(left, right);
        public static BinaryExpression LessThan(Expression left, Expression right, bool liftToNull, MethodInfo method) => Expression.LessThan(left, right, liftToNull, method);

        public static BinaryExpression LessThanOrEqual(Expression left, Expression right) => Expression.LessThanOrEqual(left, right);
        public static BinaryExpression LessThanOrEqual(Expression left, Expression right, bool liftToNull, MethodInfo method) => Expression.LessThanOrEqual(left, right, liftToNull, method);

        public static MemberListBinding ListBind(MemberInfo member, ElementInit[] initializers) => Expression.ListBind(member, initializers); // NB: Used for fields.
        public static MemberListBinding ListBind(MethodInfo propertyAccessor, ElementInit[] initializers) => Expression.ListBind(propertyAccessor, initializers); // NB: Used for properties.

        public static ListInitExpression ListInit(NewExpression newExpression, ElementInit[] initializers) => Expression.ListInit(newExpression, initializers);

        public static MemberMemberBinding MemberBind(MemberInfo member, MemberBinding[] bindings) => Expression.MemberBind(member, bindings); // NB: Used for fields.
        public static MemberMemberBinding MemberBind(MethodInfo propertyAccessor, MemberBinding[] bindings) => Expression.MemberBind(propertyAccessor, bindings); // NB: Used for properties.

        public static MemberInitExpression MemberInit(NewExpression newExpression, MemberBinding[] bindings) => Expression.MemberInit(newExpression, bindings);

        public static BinaryExpression Modulo(Expression left, Expression right) => Expression.Modulo(left, right);
        public static BinaryExpression Modulo(Expression left, Expression right, MethodInfo method) => Expression.Modulo(left, right, method);

        public static BinaryExpression Multiply(Expression left, Expression right) => Expression.Multiply(left, right);
        public static BinaryExpression Multiply(Expression left, Expression right, MethodInfo method) => Expression.Multiply(left, right, method);
        public static BinaryExpression MultiplyChecked(Expression left, Expression right) => Expression.MultiplyChecked(left, right);
        public static BinaryExpression MultiplyChecked(Expression left, Expression right, MethodInfo method) => Expression.MultiplyChecked(left, right, method);

        public static UnaryExpression Negate(Expression expression) => Expression.Negate(expression);
        public static UnaryExpression Negate(Expression expression, MethodInfo method) => Expression.Negate(expression, method);
        public static UnaryExpression NegateChecked(Expression expression) => Expression.NegateChecked(expression);
        public static UnaryExpression NegateChecked(Expression expression, MethodInfo method) => Expression.NegateChecked(expression, method);

        public static NewExpression New(Type type) => Expression.New(type);
        public static NewExpression New(ConstructorInfo constructor, IEnumerable<Expression> arguments) => Expression.New(constructor, arguments); // NB: Quirk causing binding to IEnumerable<T> overload rather than T[].
        public static NewExpression New(ConstructorInfo constructor, IEnumerable<Expression> arguments, MemberInfo[] members) => Expression.New(constructor, arguments, members);

        public static NewArrayExpression NewArrayBounds(Type type, Expression[] bounds) => Expression.NewArrayBounds(type, bounds);
        public static NewArrayExpression NewArrayInit(Type type, Expression[] initializers) => Expression.NewArrayInit(type, initializers);

        public static UnaryExpression Not(Expression expression) => Expression.Not(expression);
        public static UnaryExpression Not(Expression expression, MethodInfo method) => Expression.Not(expression, method);

        public static BinaryExpression NotEqual(Expression left, Expression right) => Expression.NotEqual(left, right);
        public static BinaryExpression NotEqual(Expression left, Expression right, bool liftToNull, MethodInfo method) => Expression.NotEqual(left, right, liftToNull, method);

        public static BinaryExpression Or(Expression left, Expression right) => Expression.Or(left, right);
        public static BinaryExpression Or(Expression left, Expression right, MethodInfo method) => Expression.Or(left, right, method);

        public static BinaryExpression OrElse(Expression left, Expression right) => Expression.OrElse(left, right);
        public static BinaryExpression OrElse(Expression left, Expression right, MethodInfo method) => Expression.OrElse(left, right, method);

        public static ParameterExpression Parameter(Type type, string name) => Expression.Parameter(type, name);

        public static MemberExpression Property(Expression expression, MethodInfo propertyAccessor) => Expression.Property(expression, propertyAccessor); // NB: Never binds to overloads with PropertyInfo.

        public static UnaryExpression Quote(Expression expression) => Expression.Quote(expression);

        public static BinaryExpression RightShift(Expression left, Expression right) => Expression.RightShift(left, right);
        public static BinaryExpression RightShift(Expression left, Expression right, MethodInfo method) => Expression.RightShift(left, right, method);

        public static BinaryExpression Subtract(Expression left, Expression right) => Expression.Subtract(left, right);
        public static BinaryExpression Subtract(Expression left, Expression right, MethodInfo method) => Expression.Subtract(left, right, method);
        public static BinaryExpression SubtractChecked(Expression left, Expression right) => Expression.SubtractChecked(left, right);
        public static BinaryExpression SubtractChecked(Expression left, Expression right, MethodInfo method) => Expression.SubtractChecked(left, right, method);

        public static UnaryExpression TypeAs(Expression expression, Type type) => Expression.TypeAs(expression, type);

        public static TypeBinaryExpression TypeIs(Expression expression, Type type) => Expression.TypeIs(expression, type);

        public static UnaryExpression UnaryPlus(Expression expression, MethodInfo method) => Expression.UnaryPlus(expression, method); // NB: Never binds to overload without method.
    }
}
