// Prototyping extended expression trees for C#.
//
// bartde - December 2021

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;

using static System.Dynamic.Utils.ContractUtils;
using static System.Linq.Expressions.ExpressionStubs;

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    // REVIEW: Consider reflecting the C# grammar instead of "recursive" (despite the C# 8.0 feature being titled "Recurive pattern matching").

    /// <summary>
    /// Represents a pattern that applies positional and/or property subpatterns.
    /// </summary>
    public sealed partial class RecursiveCSharpPattern : CSharpObjectPattern, IPositionalCSharpPattern
    {
        internal RecursiveCSharpPattern(CSharpObjectPatternInfo info, Type type, MethodInfo deconstructMethod, ReadOnlyCollection<PositionalCSharpSubpattern> deconstruction, ReadOnlyCollection<PropertyCSharpSubpattern> properties)
            : base(info)
        {
            Type = type;
            DeconstructMethod = deconstructMethod;
            Deconstruction = deconstruction;
            Properties = properties;
        }

        /// <summary>
        /// Gets the type of the pattern.
        /// </summary>
        public override CSharpPatternType PatternType => CSharpPatternType.Recursive;

        /// <summary>
        /// Gets the type to check for.
        /// </summary>
        public new Type Type { get; }

        /// <summary>
        /// Gets the deconstruct method to use for positional subpatterns.
        /// </summary>
        public MethodInfo DeconstructMethod { get; }

        /// <summary>
        /// Gets the positional subpatterns to apply.
        /// </summary>
        public ReadOnlyCollection<PositionalCSharpSubpattern> Deconstruction { get; }

        /// <summary>
        /// Gets the property subpatterns to apply.
        /// </summary>
        public ReadOnlyCollection<PropertyCSharpSubpattern> Properties { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        protected internal override CSharpPattern Accept(CSharpExpressionVisitor visitor) => visitor.VisitRecursivePattern(this);

        /// <summary>
        /// Changes the input type to the specified type.
        /// </summary>
        /// <remarks>
        /// This functionality can be used when a pattern is pass to an expression or statement that applies the pattern.
        /// </remarks>
        /// <param name="inputType">The new input type.</param>
        /// <returns>The original pattern rewritten to use the specified input type.</returns>
        public override CSharpPattern ChangeType(Type inputType)
        {
            if (inputType == InputType)
            {
                return this;
            }

            return CSharpPattern.Recursive(ObjectPatternInfo(PatternInfo(inputType, NarrowedType), Variable), Type, DeconstructMethod, Deconstruction, Properties);
        }

        internal override Expression Reduce(Expression @object)
        {
            Expression createTest(Expression obj)
            {
                var exit = Expression.Label(typeof(bool), "__return");

                var vars = new List<ParameterExpression>();
                var stmts = new List<Expression>();

                // TODO: Remove duplication of the code below between Recursive and ITuple.
                void addFailIfNot(Expression test)
                {
                    // NB: Peephole optimization for _ pattern.
                    if (test is ConstantExpression { Value: true })
                    {
                        return;
                    }

                    // NB: Peephole optimization for var pattern.
                    if (test is BlockExpression b && b.Variables.Count == 0 && b.Expressions.Count == 2 && b.Result is ConstantExpression { Value: true })
                    {
                        stmts.Add(b.Expressions[0]);
                        return;
                    }

                    var expr =
                        Expression.IfThen(
                            Expression.Not(test),
                            Expression.Goto(exit, Expression.Constant(false))
                        );

                    stmts.Add(expr);
                }

                void emitTypeCheck(Type type)
                {
                    // NB: Implies null check.
                    addFailIfNot(Expression.TypeIs(obj, type));

                    var temp = Expression.Parameter(type, "__objT");

                    vars.Add(temp);
                    stmts.Add(Expression.Assign(temp, Expression.Convert(obj, type)));

                    obj = temp;
                }

                if (Type != null)
                {
                    emitTypeCheck(Type);
                }
                else if (!obj.Type.IsValueType)
                {
                    addFailIfNot(Expression.ReferenceNotEqual(obj, Expression.Constant(null, obj.Type)));
                }
                else if (obj.Type.IsNullableType())
                {
                    emitTypeCheck(obj.Type.GetNonNullableType());
                }

                if (Deconstruction.Count > 0)
                {
                    if (DeconstructMethod != null)
                    {
                        var isExtensionMethod = DeconstructMethod.IsStatic;

                        var parameters = DeconstructMethod.GetParametersCached();
                        var parameterCount = parameters.Length;

                        if (isExtensionMethod)
                        {
                            parameterCount--;
                        }

                        var parameterToVariable = new Dictionary<ParameterInfo, ParameterExpression>(parameterCount);
                        var parameterIndexToVariable = new ParameterExpression[parameterCount];
                        var deconstructArgs = new Expression[parameterCount];

                        for (var i = 0; i < parameterCount; i++)
                        {
                            var parameter = parameters[isExtensionMethod ? i + 1 : i];

                            Debug.Assert(parameter.IsOut && parameter.IsByRefParameter());

                            var type = parameter.ParameterType.GetElementType();
                            var temp = Expression.Parameter(type, "__obj_d" + i);

                            vars.Add(temp);

                            parameterToVariable.Add(parameter, temp);
                            parameterIndexToVariable[i] = temp;
                            deconstructArgs[i] = temp;
                        }

                        var deconstruct = isExtensionMethod
                            ? Expression.Call(DeconstructMethod, deconstructArgs.AddFirst(obj))
                            : Expression.Call(obj, DeconstructMethod, deconstructArgs);

                        stmts.Add(deconstruct);

                        var deconstructionCount = Deconstruction.Count;

                        for (var i = 0; i < deconstructionCount; i++)
                        {
                            var deconstruction = Deconstruction[i];

                            var var = deconstruction.Parameter != null ? parameterToVariable[deconstruction.Parameter] : parameterIndexToVariable[i];
                            var test = deconstruction.Pattern.Reduce(var);

                            addFailIfNot(test);
                        }
                    }
                    else
                    {
                        var deconstructionCount = Deconstruction.Count;

                        for (var i = 0; i < deconstructionCount; i++)
                        {
                            var deconstruction = Deconstruction[i];

                            var index = deconstruction.Field?.Index ?? i;
                            var item = Helpers.GetTupleItemAccess(obj, index);
                            var test = deconstruction.Pattern.Reduce(item);

                            addFailIfNot(test);
                        }
                    }
                }

                if (Properties.Count > 0)
                {
                    foreach (var prop in Properties)
                    {
                        addFailIfNot(prop.Reduce(obj, vars, stmts, addFailIfNot));
                    }
                }

                if (Variable != null)
                {
                    stmts.Add(Expression.Assign(Variable, obj));
                }

                stmts.Add(Expression.Label(exit, Expression.Constant(true)));

                return Expression.Block(vars, stmts);
            }

            return PatternHelpers.Reduce(@object, createTest);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="variable">The <see cref="Variable" /> property of the result.</param>
        /// <param name="deconstruction">The <see cref="Deconstruction" /> property of the result.</param>
        /// <param name="properties">The <see cref="Properties" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public RecursiveCSharpPattern Update(ParameterExpression variable, IEnumerable<PositionalCSharpSubpattern> deconstruction, IEnumerable<PropertyCSharpSubpattern> properties)
        {
            if (variable == Variable && SameElements(ref deconstruction, Deconstruction) && SameElements(ref properties, Properties))
            {
                return this;
            }

            return CSharpPattern.Recursive(ObjectPatternInfo(_info, variable), Type, DeconstructMethod, deconstruction, properties);
        }
    }

    partial class CSharpPattern
    {
        //
        // TODO: Add more convenience overloads for positional recursive patterns.
        //
        //       - Should it find Deconstruct instance methods if unspecified?
        //         - This will require to find types from PositionalCSharpSubpattern.Pattern.InputType to do overload resolution for the out parameters.
        //

        /// <summary>
        /// Creates a positional pattern that matches on components of an object.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="type">The type to check for.</param>
        /// <param name="deconstructMethod">The method used to deconstruct an object into its components for use with positional subpatterns.</param>
        /// <param name="deconstruction">The property subpatterns to apply.</param>
        /// <returns>A <see cref="IPositionalCSharpPattern" /> representing a positional pattern.</returns>
        public static IPositionalCSharpPattern Positional(CSharpObjectPatternInfo info, Type type, MethodInfo deconstructMethod, params PositionalCSharpSubpattern[] deconstruction) => Positional(info, type, deconstructMethod, (IEnumerable<PositionalCSharpSubpattern>)deconstruction);

        /// <summary>
        /// Creates a positional pattern that matches on components of an object.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="type">The type to check for.</param>
        /// <param name="deconstructMethod">The method used to deconstruct an object into its components for use with positional subpatterns.</param>
        /// <param name="deconstruction">The property subpatterns to apply.</param>
        /// <returns>A <see cref="IPositionalCSharpPattern" /> representing a positional pattern.</returns>
        public static IPositionalCSharpPattern Positional(CSharpObjectPatternInfo info, Type type, MethodInfo deconstructMethod, IEnumerable<PositionalCSharpSubpattern> deconstruction)
        {
            RequiresNotNull(info, nameof(info));

            if (deconstructMethod == null && info.Variable == null && type == null)
            {
                var narrowedType = info.Info.NarrowedType;

                if (!Helpers.IsTupleType(narrowedType))
                {
                    return ITuple(deconstruction);
                }
            }

            return Recursive(info, type, deconstructMethod, deconstruction, properties: null);
        }

        /// <summary>
        /// Creates a property pattern that matches on properties.
        /// </summary>
        /// <param name="type">The type to check for.</param>
        /// <param name="variable">The variable to assign to.</param>
        /// <param name="properties">The property subpatterns to apply.</param>
        /// <returns>A <see cref="RecursiveCSharpPattern" /> representing a property pattern.</returns>
        public static RecursiveCSharpPattern Property(Type type, ParameterExpression variable, params PropertyCSharpSubpattern[] properties) =>
            Property(type, variable, (IEnumerable<PropertyCSharpSubpattern>)properties);

        /// <summary>
        /// Creates a property pattern that matches on properties.
        /// </summary>
        /// <param name="type">The type to check for.</param>
        /// <param name="variable">The variable to assign to.</param>
        /// <param name="properties">The property subpatterns to apply.</param>
        /// <returns>A <see cref="RecursiveCSharpPattern" /> representing a property pattern.</returns>
        public static RecursiveCSharpPattern Property(Type type, ParameterExpression variable, IEnumerable<PropertyCSharpSubpattern> properties)
        {
            var narrowedType = variable?.Type ?? type;
            var info = ObjectPatternInfo(PatternInfo(typeof(object), narrowedType), variable);

            return Recursive(info, type, deconstructMethod: null, deconstruction: null, properties);
        }

        /// <summary>
        /// Creates a property pattern that matches on properties.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="type">The type to check for.</param>
        /// <param name="variable">The variable to assign to.</param>
        /// <param name="properties">The property subpatterns to apply.</param>
        /// <returns>A <see cref="RecursiveCSharpPattern" /> representing a property pattern.</returns>
        public static RecursiveCSharpPattern Property(CSharpPatternInfo info, Type type, ParameterExpression variable, params PropertyCSharpSubpattern[] properties) =>
            Property(info, type, variable, (IEnumerable<PropertyCSharpSubpattern>)properties);

        /// <summary>
        /// Creates a property pattern that matches on properties.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="type">The type to check for.</param>
        /// <param name="variable">The variable to assign to.</param>
        /// <param name="properties">The property subpatterns to apply.</param>
        /// <returns>A <see cref="RecursiveCSharpPattern" /> representing a property pattern.</returns>
        public static RecursiveCSharpPattern Property(CSharpPatternInfo info, Type type, ParameterExpression variable, IEnumerable<PropertyCSharpSubpattern> properties) =>
            Recursive(ObjectPatternInfo(info, variable), type, deconstructMethod: null, deconstruction: null, properties);

        /// <summary>
        /// Creates a property pattern that matches on properties.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="type">The type to check for.</param>
        /// <param name="properties">The property subpatterns to apply.</param>
        /// <returns>A <see cref="RecursiveCSharpPattern" /> representing a property pattern.</returns>
        public static RecursiveCSharpPattern Property(CSharpObjectPatternInfo info, Type type, params PropertyCSharpSubpattern[] properties) =>
            Property(info, type, (IEnumerable<PropertyCSharpSubpattern>)properties);

        /// <summary>
        /// Creates a property pattern that matches on properties.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="type">The type to check for.</param>
        /// <param name="properties">The property subpatterns to apply.</param>
        /// <returns>A <see cref="RecursiveCSharpPattern" /> representing a property pattern.</returns>
        public static RecursiveCSharpPattern Property(CSharpObjectPatternInfo info, Type type, IEnumerable<PropertyCSharpSubpattern> properties) =>
            Recursive(info, type, deconstructMethod: null, deconstruction: null, properties);

        /// <summary>
        /// Creates a recursive pattern that can perform positional matching and/or matching on properties.
        /// </summary>
        /// <param name="info">Type information about the pattern.</param>
        /// <param name="type">The type to check for.</param>
        /// <param name="deconstructMethod">The method used to deconstruct an object into its components for use with positional subpatterns.</param>
        /// <param name="deconstruction">The positional subpatterns to apply to the components of the object.</param>
        /// <param name="properties">The property subpatterns to apply.</param>
        /// <returns>A <see cref="RecursiveCSharpPattern" /> representing a recursive pattern.</returns>
        public static RecursiveCSharpPattern Recursive(CSharpObjectPatternInfo info, Type type, MethodInfo deconstructMethod, IEnumerable<PositionalCSharpSubpattern> deconstruction, IEnumerable<PropertyCSharpSubpattern> properties)
        {
            if (info == null)
            {
                // NB: We could arguably attempt to infer the type from the other parameters but that's a bit too implicit.

                RequiresNotNull(type, nameof(type));

                info = ObjectPatternInfo(PatternInfo(typeof(object), type), variable: null);
            }
            else
            {
                if (info.Variable != null)
                {
                    RequiresCompatiblePatternTypes(info.Info.NarrowedType, info.Variable.Type);
                }
            }

            var narrowedType = info.Info.NarrowedType;
            var deconstructionCollection = deconstruction.ToReadOnly();
            var propertiesCollection = properties.ToReadOnly();

            var objParam = Expression.Parameter(narrowedType); // NB: Utility to utilize some expression factories to expedite some checks.

            // REVIEW: Reordering of positional matches with names (for tuple elements or deconstruction out parameters) is not supported.

            validateType();
            validatePositional();
            validateProperties();

            return new RecursiveCSharpPattern(info, type, deconstructMethod, deconstructionCollection, propertiesCollection);

            void validateType()
            {
                if (type != null)
                {
                    ValidatePatternType(type);

                    var variableType = info.Variable?.Type;

                    if (variableType != null)
                    {
                        RequiresCompatiblePatternTypes(type, variableType);

                        if (variableType != narrowedType)
                            throw Error.CannotAssignPatternResultToVariable(variableType, narrowedType);
                    }
                }
            }

            void validatePositional()
            {
                if (deconstructionCollection.Count > 0)
                {
                    foreach (var positionalPattern in deconstructionCollection)
                    {
                        RequiresNotNull(positionalPattern, nameof(deconstruction));
                    }

                    if (deconstructMethod != null)
                    {
                        validateWithDeconstructMethod();
                    }
                    else if (Helpers.IsTupleType(narrowedType))
                    {
                        validateWithTupleType();
                    }
                    else
                    {
                        throw Error.InvalidPositionalPattern();
                    }

                    void validateWithDeconstructMethod()
                    {
                        ValidateMethodInfo(deconstructMethod);

                        if (deconstructMethod.ReturnType != typeof(void))
                            throw Error.DeconstructShouldReturnVoid(deconstructMethod);

                        var parameters = deconstructMethod.GetParametersCached();

                        if (deconstructMethod.IsStatic)
                        {
                            if (parameters.Length == 0)
                                throw Error.DeconstructExtensionMethodMissingThis(deconstructMethod);

                            ValidateOneArgument(deconstructMethod, ExpressionType.Call, objParam, parameters[0]);

                            parameters = parameters.RemoveFirst();
                        }
                        else
                        {
                            ValidateCallInstanceType(narrowedType, deconstructMethod);
                        }

                        var arity = parameters.Length;

                        checkArity(arity);

                        var parameterToPattern = new Dictionary<ParameterInfo, PositionalCSharpSubpattern>();

                        foreach (var positionalPattern in deconstructionCollection)
                        {
                            if (positionalPattern.Field != null)
                                throw Error.PositionalPatternWithDeconstructMethodCannotSpecifyField();

                            var parameter = positionalPattern.Parameter;

                            if (parameter != null)
                            {
                                if (parameter.Member != deconstructMethod)
                                    throw Error.PositionalPatternParameterIsNotDeclaredOnDeconstructMethod(parameter, deconstructMethod);

                                if (parameterToPattern.ContainsKey(parameter))
                                    throw Error.PositionalPatternParameterShouldOnlyBeUsedOnce(parameter);

                                parameterToPattern.Add(parameter, positionalPattern);
                            }
                        }

                        var bindByParameter = parameterToPattern.Count > 0;

                        if (bindByParameter && parameterToPattern.Count != arity)
                            throw Error.PositionalPatternWithDeconstructMethodShouldSpecifyAllParameters();

                        PositionalCSharpSubpattern getPositionalPattern(ParameterInfo parameter, int index) => bindByParameter
                            ? parameterToPattern[parameter]
                            : deconstructionCollection[index];

                        for (var i = 0; i < arity; i++)
                        {
                            var parameter = parameters[i];

                            if (!parameter.IsOut)
                                throw Error.DeconstructParameterShouldBeOut(parameter, deconstructMethod);

                            var pattern = getPositionalPattern(parameter, i).Pattern;
                            var parameterType = parameter.ParameterType.GetElementType();

                            // REVIEW: Can we loosen the type checking here (assignment compatibility) or trigger ChangeType?
                            RequiresCompatiblePatternTypes(pattern.InputType, parameterType);
                        }
                    }

                    void validateWithTupleType()
                    {
                        var arity = Helpers.GetTupleArity(narrowedType);

                        checkArity(arity);

                        var byIndexCount = 0;
                        var indexToPattern = new PositionalCSharpSubpattern[arity];

                        foreach (var positionalPattern in deconstructionCollection)
                        {
                            if (positionalPattern.Parameter != null)
                                throw Error.PositionalPatternWithTupleCannotSpecifyParameter();

                            if (positionalPattern.Field != null)
                            {
                                var index = positionalPattern.Field.Index;

                                if (index < 0 || index >= arity)
                                    throw Error.PositionalPatternTupleIndexOutOfRange(index, arity);

                                if (indexToPattern[index] != null)
                                    throw Error.PositionalPatternTupleIndexShouldOnlyBeUsedOnce(index);

                                indexToPattern[index] = positionalPattern;
                                byIndexCount++;
                            }
                        }

                        var bindByIndex = byIndexCount > 0;

                        if (bindByIndex && byIndexCount != arity)
                            throw Error.PositionalPatternWithTupleShouldSpecifyAllIndices();

                        PositionalCSharpSubpattern getPositionalPattern(int index) => bindByIndex
                            ? indexToPattern[index]
                            : deconstructionCollection[index];

                        var elementTypes = Helpers.GetTupleComponentTypes(narrowedType).ToReadOnly();

                        for (var i = 0; i < arity; i++)
                        {
                            var elementType = elementTypes[i];

                            var pattern = getPositionalPattern(i).Pattern;

                            // REVIEW: Can we loosen the type checking here (assignment compatibility) or trigger ChangeType?
                            RequiresCompatiblePatternTypes(pattern.InputType, elementType);
                        }
                    }

                    void checkArity(int arity)
                    {
                        if (arity != deconstructionCollection.Count)
                            throw Error.InvalidPositionalPatternCount(narrowedType);
                    }
                }
            }

            void validateProperties()
            {
                foreach (var propertyPattern in propertiesCollection)
                {
                    RequiresNotNull(propertyPattern, nameof(properties));

                    var member = findTopMostMember(propertyPattern.Member);

                    if (member.Member != null)
                    {
                        // TODO: Inline the check.
                        _ = Expression.MakeMemberAccess(objParam, member.Member);
                    }
                    else
                    {
                        // TODO: Inline the check.
                        _ = Helpers.GetTupleItemAccess(objParam, member.TupleField.Index);
                    }
                }

                static PropertyCSharpSubpatternMember findTopMostMember(PropertyCSharpSubpatternMember member)
                {
                    if (member.Receiver == null)
                    {
                        return member;
                    }

                    return findTopMostMember(member.Receiver);
                }
            }
        }
    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="RecursiveCSharpPattern" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual CSharpPattern VisitRecursivePattern(RecursiveCSharpPattern node) =>
            node.Update(
                VisitAndConvert(node.Variable, nameof(VisitRecursivePattern)),
                Visit(node.Deconstruction, VisitPositionalSubpattern),
                Visit(node.Properties, VisitPropertySubpattern)
            );
    }
}
