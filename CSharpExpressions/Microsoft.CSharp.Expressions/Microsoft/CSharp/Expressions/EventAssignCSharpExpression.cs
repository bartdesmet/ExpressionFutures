// Prototyping extended expression trees for C#.
//
// bartde - January 2022

using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;

using static System.Dynamic.Utils.ContractUtils;
using static System.Dynamic.Utils.ErrorUtils;
using static System.Dynamic.Utils.ExpressionUtils;
using static System.Dynamic.Utils.TypeUtils;

#pragma warning disable CA1720 // Identifier contains type name (use of Object property).

namespace Microsoft.CSharp.Expressions
{
    using static Helpers;

    /// <summary>
    /// Represents an event assignment operation.
    /// </summary>
    public sealed partial class EventAssignCSharpExpression : CSharpExpression
    {
        internal EventAssignCSharpExpression(CSharpExpressionType type, Expression @object, EventInfo @event, Expression handler)
        {
            CSharpNodeType = type;
            Object = @object;
            Event = @event;
            Handler = handler;
        }

        /// <summary>
        /// Returns the node type of this <see cref="CSharpExpression" />.
        /// </summary>
        /// <returns>The <see cref="CSharpExpressionType"/> that represents this expression.</returns>
        public override CSharpExpressionType CSharpNodeType { get; }

        /// <summary>
        /// Gets the static type of the expression.
        /// </summary>
        public override Type Type => typeof(void);

        /// <summary>
        /// The expression representing the object on which to access the event, or <c>null</c> if the event is static.
        /// </summary>
        public Expression Object { get; }

        /// <summary>
        /// Gets the event to add or remove a handler on.
        /// </summary>
        public EventInfo Event { get; }

        /// <summary>
        /// The expression representing the event handler to add or remove.
        /// </summary>
        public Expression Handler { get; }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <param name="visitor">The visitor to visit this node with.</param>
        /// <returns>The result of visiting this node.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal override Expression Accept(CSharpExpressionVisitor visitor) => visitor.VisitEventAssign(this);

        /// <summary>
        /// Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will return this expression.
        /// </summary>
        /// <param name="object">The <see cref="Object" /> property of the result. </param>
        /// <param name="handler">The <see cref="Handler" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public EventAssignCSharpExpression Update(Expression @object, Expression handler)
        {
            if (@object == Object && handler == Handler)
            {
                return this;
            }

            return CSharpExpression.MakeEventAssign(CSharpNodeType, @object, Event, handler);
        }

        /// <summary>
        /// Reduces the expression node to a simpler expression.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            var method = CSharpNodeType == CSharpExpressionType.EventAddAssign ? Event.GetAddMethod(nonPublic: true) : Event.GetRemoveMethod(nonPublic: true);

            return Expression.Call(Object, method, Handler);
        }
    }

    partial class CSharpExpression
    {
        /// <summary>
        /// Creates an expression representing an event assignment operation.
        /// </summary>
        /// <param name="type">The type of assignment represented.</param>
        /// <param name="object">The instance on which to access the event, or <c>null</c> if the event is static.</param>
        /// <param name="event">The event to add or remove a handler on.</param>
        /// <param name="handler">The handler to add to or remove from the event.</param>
        /// <returns>A new <see cref="EventAssignCSharpExpression"/> instance representing the event assignment.</returns>
        public static EventAssignCSharpExpression MakeEventAssign(CSharpExpressionType type, Expression @object, EventInfo @event, Expression handler) =>
            type switch
            {
                CSharpExpressionType.EventAddAssign => EventAddAssign(@object, @event, handler),
                CSharpExpressionType.EventSubtractAssign => EventSubtractAssign(@object, @event, handler),
                _ => throw UnhandledBinary(type),
            };

        /// <summary>
        /// Creates an expression representing an event addition operation.
        /// </summary>
        /// <param name="object">The instance on which to access the event, or <c>null</c> if the event is static.</param>
        /// <param name="event">The event to add a handler on.</param>
        /// <param name="handler">The handler to add to the event.</param>
        /// <returns>A new <see cref="EventAssignCSharpExpression"/> instance representing the event assignment.</returns>
        public static EventAssignCSharpExpression EventAddAssign(Expression @object, EventInfo @event, Expression handler) =>
            EventAssign(@object, @event, handler, isAddition: true);

        /// <summary>
        /// Creates an expression representing an event removal operation.
        /// </summary>
        /// <param name="object">The instance on which to access the event, or <c>null</c> if the event is static.</param>
        /// <param name="event">The event to remove a handler from.</param>
        /// <param name="handler">The handler to remove from the event.</param>
        /// <returns>A new <see cref="EventAssignCSharpExpression"/> instance representing the event assignment.</returns>
        public static EventAssignCSharpExpression EventSubtractAssign(Expression @object, EventInfo @event, Expression handler) =>
            EventAssign(@object, @event, handler, isAddition: false);

        /// <summary>
        /// Creates an expression representing an event addition operation.
        /// </summary>
        /// <param name="object">The instance on which to access the event, or <c>null</c> if the event is static.</param>
        /// <param name="eventAccessor">The accessor method of the event to add a handler on.</param>
        /// <param name="handler">The handler to add to the event.</param>
        /// <returns>A new <see cref="EventAssignCSharpExpression"/> instance representing the event assignment.</returns>
        public static EventAssignCSharpExpression EventAddAssign(Expression @object, MethodInfo eventAccessor, Expression handler) =>
            EventAssign(@object, eventAccessor, handler, isAddition: true);

        /// <summary>
        /// Creates an expression representing an event removal operation.
        /// </summary>
        /// <param name="object">The instance on which to access the event, or <c>null</c> if the event is static.</param>
        /// <param name="eventAccessor">The accessor method of the event to remove a handler from.</param>
        /// <param name="handler">The handler to remove from the event.</param>
        /// <returns>A new <see cref="EventAssignCSharpExpression"/> instance representing the event assignment.</returns>
        public static EventAssignCSharpExpression EventSubtractAssign(Expression @object, MethodInfo eventAccessor, Expression handler) =>
            EventAssign(@object, eventAccessor, handler, isAddition: false);

        private static EventAssignCSharpExpression EventAssign(Expression @object, MethodInfo eventAccessor, Expression handler, bool isAddition)
        {
            RequiresNotNull(eventAccessor, nameof(eventAccessor));
            ValidateMethodInfo(eventAccessor, nameof(eventAccessor));

            return EventAssign(@object, GetEvent(eventAccessor), handler, isAddition);
        }

        private static EventAssignCSharpExpression EventAssign(Expression @object, EventInfo @event, Expression handler, bool isAddition)
        {
            RequiresNotNull(@event, nameof(@event));

            var accessor = isAddition ? @event.GetAddMethod(nonPublic: true) : @event.GetRemoveMethod(nonPublic: true);

            if (accessor == null)
                throw Error.EventDoesNotHaveAccessor(@event);

            if (accessor.IsStatic)
            {
                if (@object != null)
                    throw Error.OnlyStaticEventsHaveNullInstance();
            }
            else
            {
                if (@object == null)
                    throw Error.OnlyStaticEventsHaveNullInstance();

                RequiresCanRead(@object, nameof(@object));

                if (!IsValidInstanceType(@event, @object.Type))
                    throw Error.EventNotDefinedForType(@event, @object.Type);
            }

            RequiresCanRead(handler, nameof(handler));

            // NB: We only support classic .NET events for the time being because WindowsRuntimeMarshal needed for reduction
            //     of event add/remove calls is not available on future .NET platforms.

            if (accessor.ReturnType != typeof(void))
                throw Error.EventAccessorShouldReturnVoid();

            var parameters = accessor.GetParametersCached();

            if (parameters.Length != 1)
                throw Error.EventAccessorShouldHaveOneParameter();

            if (!AreReferenceAssignable(parameters[0].ParameterType, handler.Type))
                throw Error.EventAccessorParameterTypeMismatch(handler.Type, parameters[0].ParameterType);

            return new EventAssignCSharpExpression(isAddition ? CSharpExpressionType.EventAddAssign : CSharpExpressionType.EventSubtractAssign, @object, @event, handler);
        }

        private static EventInfo GetEvent(MethodInfo method)
        {
            var declaringType = method.DeclaringType;

            var flags = BindingFlags.Public | BindingFlags.NonPublic;
            flags |= (method.IsStatic ? BindingFlags.Static : BindingFlags.Instance);

            foreach (var eventInfo in declaringType.GetEvents(flags))
            {
                if (eventInfo.GetAddMethod(nonPublic: true) == method)
                {
                    return eventInfo;
                }

                if (eventInfo.GetRemoveMethod(nonPublic: true) == method)
                {
                    return eventInfo;
                }
            }

            throw Error.MethodNotEventAccessor(declaringType, method.Name);
        }

    }

    partial class CSharpExpressionVisitor
    {
        /// <summary>
        /// Visits the children of the <see cref="AssignBinaryCSharpExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Following the visitor pattern from System.Linq.Expressions.")]
        protected internal virtual Expression VisitEventAssign(EventAssignCSharpExpression node) =>
            node.Update(
                Visit(node.Object),
                Visit(node.Handler)
            );
    }
}
