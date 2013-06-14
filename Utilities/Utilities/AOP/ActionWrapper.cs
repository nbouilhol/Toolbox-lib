using System;

namespace Utilities.AOP
{
    public class ActionWrapper<T> : BaseWrapper<T> where T : class
    {
        private readonly Action<T> _actionAfter;
        private readonly Action<T> _actionBefore;

        public ActionWrapper(T source, Action<T> actionBefore, Action<T> actionAfter)
            : base(source)
        {
            _actionBefore = actionBefore;
            _actionAfter = actionAfter;
        }

        public override void OnEntry(T source, string methodName, object[] args)
        {
            if (_actionBefore != null)
                _actionBefore(source);
        }

        public override void OnExit(T source, string methodName, object[] args, object result)
        {
            if (_actionAfter != null)
                _actionAfter(source);
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