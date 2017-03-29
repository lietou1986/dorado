using System;

namespace Dorado.Core
{
    public class TaskFuncAdapter : ITask
    {
        public TaskFuncAdapter(Action action)
        {
            _action = action ?? _EmptyAction;
        }

        private readonly Action _action;

        private static void _EmptyAction()
        {
        }

        #region ITask Members

        public void Execute()
        {
            _action();
        }

        #endregion ITask Members
    }
}