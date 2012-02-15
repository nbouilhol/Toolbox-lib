using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Utilities.Helpers
{
    public class UndoManager
    {
        private readonly ConcurrentStack<Tuple<Action, Action>> doStack = new ConcurrentStack<Tuple<Action, Action>>();
        private readonly ConcurrentStack<Tuple<Action, Action>> undoStack = new ConcurrentStack<Tuple<Action, Action>>();

        public bool CanUndo { get { return undoStack.Any(); } }
        public bool CanRedo { get { return doStack.Any(); } }

        public void DoAndAdd(Action doAction, Action undoAction)
        {
            doAction();
            undoStack.Push(Tuple.Create(doAction, undoAction));
            doStack.Clear();
        }

        public void Undo()
        {
            Tuple<Action, Action> action;
            if (undoStack.TryPop(out action))
            {
                action.Item2();
                doStack.Push(action);
            }
        }

        public void Redo()
        {
            Tuple<Action, Action> action;
            if (doStack.TryPop(out action))
            {
                action.Item1();
                undoStack.Push(action);
            }
        }
    }
}
