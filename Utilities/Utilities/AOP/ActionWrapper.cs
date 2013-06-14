using System;

namespace Utilities.AOP
{
    public class ActionWrapper<T> : BaseWrapper<T> where T : class
    {
        private readonly Action<T> actionBefore;
        private readonly Action<T> actionAfter;

        public ActionWrapper(T source, Action<T> actionBefore, Action<T> actionAfter)
            : base(source)
        {
            this.actionBefore = actionBefore;
            this.actionAfter = actionAfter;
        }

        public override void OnEntry(T source, string methodName, object[] args)
        {
            if (actionBefore != null)
                actionBefore(source);
        }

        public override void OnExit(T source, string methodName, object[] args, object result)
        {
            if (actionAfter != null)
                actionAfter(source);
        }
    }

    public static class ActionWrapperExtension
    {
        public static dynamic Arround<T>(this T source, Action<T> actionBefore, Action<T> actionAfter) where T : class
        {
            return new ActionWrapper<T>(source, actionBefore, actionAfter);
        }
    }
}