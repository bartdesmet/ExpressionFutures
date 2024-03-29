﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;

namespace System.Dynamic.Utils
{
    internal static class ErrorUtils
    {
        public static ArgumentException AllCaseBodiesMustHaveSameType(string? paramName) => new("All case bodies and the default body must have the same type.", paramName);
        public static ArgumentException ArgumentCannotBeOfTypeVoid(string? paramName) => new("Argument type cannot be System.Void.", paramName);
        public static ArgumentException ArgumentMemberNotDeclOnType(object? p0, object? p1, string? paramName, int index) => new($"The member '{p0}' is not declared on type '{p1}' being created.", GetParamName(paramName, index));
        public static ArgumentException ArgumentMustBeArray(string? paramName) => new("Argument must be array.", paramName);
        public static ArgumentException ArgumentMustBeArrayIndexType(string? paramName) => new("Argument for array index must be of type Int32.", paramName);
        public static ArgumentException ArgumentMustBeBoolean(string? paramName) => new("Argument must be Boolean.", paramName);
        public static ArgumentException ArgumentMustBeFieldInfoOrPropertyInfo(string? paramName) => new("Argument must be either a FieldInfo or PropertyInfo.", paramName);
        public static ArgumentException ArgumentMustBeFieldInfoOrPropertyInfoOrMethod(string? paramName, int index) => new("Argument must be either a FieldInfo, PropertyInfo or MethodInfo", GetParamName(paramName, index));
        public static ArgumentException ArgumentMustBeInstanceMember(string? paramName, int index) => new("Argument must be an instance member.", GetParamName(paramName, index));
        public static ArgumentException ArgumentTypesMustMatch(string? paramName = null) => new("Argument types do not match.", paramName);
        public static ArgumentException BoundsCannotBeLessThanOne(string? paramName) => new("Bounds count cannot be less than 1.", paramName);
        public static ArgumentException CannotAutoInitializeValueTypeElementThroughProperty(object? p0) => new($"Cannot auto initialize elements of value type through property '{p0}', use assignment instead.");
        public static ArgumentException CannotAutoInitializeValueTypeMemberThroughProperty(object? p0) => new($"Cannot auto initialize members of value type through property '{p0}', use assignment instead.");
        public static ArgumentException CoalesceUsedOnNonNullType() => new("Coalesce used with type that cannot be null");
        public static ArgumentException DuplicateVariable(object? p0, string? paramName, int index) => new($"Found duplicate parameter '{p0}'. Each ParameterExpression in the list must be a unique object.", GetParamName(paramName, index));
        public static ArgumentException ExpressionTypeCannotInitializeArrayType(object? p0, object? p1) => new($"An expression of type '{p0}' cannot be used to initialize an array of type '{p1}'.");
        public static ArgumentException ExpressionTypeDoesNotMatchAssignment(object? p0, object? p1) => new($"Expression of type '{p0}' cannot be used for assignment to type '{p1}'.");
        public static ArgumentException ExpressionTypeDoesNotMatchConstructorParameter(object? p0, object? p1, string? paramName, int index) => new($"Expression of type '{p0}' cannot be used for constructor parameter of type '{p1}'.", GetParamName(paramName, index));
        public static ArgumentException ExpressionTypeDoesNotMatchMethodParameter(object? p0, object? p1, object p2, string? paramName, int index) => new($"Expression of type '{p0}' cannot be used for parameter of type '{p1}' of method '{p2}'.", GetParamName(paramName, index));
        public static ArgumentException ExpressionTypeDoesNotMatchParameter(object? p0, object? p1, string? paramName, int index) => new($"Expression of type '{p0}' cannot be used for parameter of type '{p1}'.", GetParamName(paramName, index));
        public static ArgumentException ExpressionTypeDoesNotMatchReturn(object? p0, object? p1) => new($"Expression of type '{p0}' cannot be used for return type '{p1}'.");
        public static ArgumentException ExpressionTypeNotInvocable(object? p0, string? paramName) => new($"Expression of type '{p0}' cannot be invoked.", paramName);
        public static ArgumentException FieldInfoNotDefinedForType(object? p0, object? p1, object p2) => new($"Field '{p0}.{p1}' is not defined for type '{p2}'.");
        public static ArgumentException IncorrectNumberOfConstructorArguments() => new("Incorrect number of arguments for constructor.");
        public static ArgumentException IncorrectNumberOfIndexes() => new("Incorrect number of indexes.");
        public static ArgumentException IncorrectNumberOfLambdaArguments() => new("Incorrect number of arguments supplied for lambda invocation.");
        public static ArgumentException IncorrectNumberOfLambdaDeclarationParameters() => new("Incorrect number of parameters supplied for lambda declaration.");
        public static ArgumentException IncorrectNumberOfMethodCallArguments(object? p0, string? paramName, int index = -1) => new($"Incorrect number of arguments supplied for call to method '{p0}'.", GetParamName(paramName, index));
        public static ArgumentException InstanceAndMethodTypeMismatch(object? p0, object? p1, object p2) => new($"Method '{p0}' declared on type '{p1}' cannot be called with instance of type '{p2}'.");
        public static ArgumentException InstanceFieldNotDefinedForType(object? p0, object? p1) => new($"Instance field '{p0}' is not defined for type '{p1}'.");
        public static ArgumentException InstancePropertyNotDefinedForType(object? p0, object? p1, string? paramName) => new($"Instance property '{p0}' is not defined for type '{p1}'.", paramName);
        public static ArgumentException InvalidLvalue(ExpressionType p0) => new($"Invalid lvalue for assignment: {p0}.");
        public static ArgumentException LabelTypeMustBeVoid(string? paramName) => new("Type must be System.Void for this label argument.", paramName);
        public static ArgumentException LambdaTypeMustBeDerivedFromSystemDelegate(string? paramName) => new("Lambda type parameter must be derived from System.MulticastDelegate.", paramName);
        public static ArgumentException MemberNotFieldOrProperty(object? p0, string? paramName) => new($"Member '{p0}' not field or property.", paramName);
        public static ArgumentException NotAMemberOfType(object? p0, object? p1, string? paramName, int index = -1) => new($"'{p0}' is not a member of member of type '{p1}'.", GetParamName(paramName, index));
        public static NotSupportedException NotSupported() => new();
        public static ArgumentException OnlyStaticMethodsHaveNullInstance() => new("Static method requires null instance, non-static method requires non-null instance.");
        public static ArgumentException OperandTypesDoNotMatchParameters(object? p0, object? p1, string? paramName, int index = -1) => new($"The operands for operator '{p0}' do not match the parameters of method '{p1}'.", GetParamName(paramName, index));
        public static ArgumentException ParameterExpressionNotValidAsDelegate(object? p0, object? p1, string? paramName, int index = -1) => new($"ParameterExpression of type '{p0}' cannot be used for delegate parameter of type '{p1}'.", GetParamName(paramName, index));
        public static ArgumentException PropertyDoesNotHaveAccessor(object? p0, string? paramName) => new($"The property '{p0}' has no 'get' or 'set' accessors.", paramName);
        public static ArgumentException PropertyDoesNotHaveGetter(object? p0, string? paramName, int index) => new($"The property '{p0}' has no 'get' accessor.", GetParamName(paramName, index));
        public static ArgumentException PropertyNotDefinedForType(object? p0, object? p1, string? paramName) => new($"Property '{p0}' is not defined for type '{p1}'.", paramName);
        public static ArgumentException TryMustHaveCatchFinallyOrFault() => new("Try must have at least one catch, finally, or fault clause.");
        public static ArgumentException UnhandledBinary(object? p0) => new($"Unhandled binary: {p0}.");
        public static ArgumentException UnhandledBinding() => new("Unhandled binding.");
        public static ArgumentException UnhandledUnary(object? p0) => new($"Unhandled unary: {p0}.");
        public static ArgumentException VariableMustNotBeByRef(object? p0, object? p1, string? paramName) => new($"Variable '{p0}' uses unsupported type '{p1}'. Reference types are not supported for variables.", paramName);

        private static string? GetParamName(string? paramName, int index) => index >= 0 ? $"{paramName}[{index}]" : paramName;
    }
}
