using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace Utilities
{
    public interface IHasDynamicProperties : IDynamicMetaObjectProvider
    {
        object GetProperty(string name);

        void SetProperty(string name, object value);
    }

    public class DynamicPropertiesMetaObject : DynamicMetaObject
    {
        public DynamicPropertiesMetaObject(Expression parameter, IHasDynamicProperties value)
            : base(parameter, BindingRestrictions.Empty, value)
        {
            Contract.Requires(parameter != null);
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            Contract.Requires(binder != null);

            MethodInfo method = LimitType.GetMethod("get_" + binder.Name);
            Expression getterExpression = method != null
                ? Expression.Call(Expression.Convert(Expression, LimitType), method)
                : GetGetterExpression(binder);

            return new DynamicMetaObject(getterExpression, BindingRestrictions.GetTypeRestriction(Expression, LimitType));
        }

        private MethodCallExpression GetGetterExpression(GetMemberBinder binder)
        {
            Contract.Requires(binder != null);

            MethodInfo method = LimitType.GetMethod("GetProperty");
            return method != null
                ? Expression.Call(Expression.Convert(Expression, LimitType), method,
                    new Expression[] {Expression.Constant(binder.Name)})
                : null;
        }

        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            Contract.Requires(binder != null);
            Contract.Requires(value != null);

            MethodInfo method = LimitType.GetMethod("set_" + binder.Name);
            Expression setterExpression = method != null
                ? Expression.Call(Expression.Convert(Expression, LimitType), method,
                    Expression.Convert(value.Expression, value.LimitType))
                : GetSetterExpression(binder, value);

            return new DynamicMetaObject(Expression.Block(setterExpression, Expression.New(typeof (object))),
                BindingRestrictions.GetTypeRestriction(Expression, LimitType));
        }

        private MethodCallExpression GetSetterExpression(SetMemberBinder binder, DynamicMetaObject value)
        {
            Contract.Requires(binder != null);
            Contract.Requires(value != null);

            MethodInfo method = LimitType.GetMethod("SetProperty");
            return method != null
                ? Expression.Call(Expression.Convert(Expression, LimitType), method,
                    new Expression[]
                    {Expression.Constant(binder.Name), Expression.Convert(value.Expression, typeof (object))})
                : null;
        }
    }
}