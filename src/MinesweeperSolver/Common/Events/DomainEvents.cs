using System;
using System.Collections.Generic;
using Ninject;

namespace MinesweeperSolver.Common.Events
{
    public static class DomainEvents
    {
        [ThreadStatic]
        private static List<Delegate> _actions;

        public static void Register<T>(Action<T> callback) where T : IDomainEvent
        {
            if (_actions == null)
                _actions = new List<Delegate>();
            _actions.Add(callback);
        }

        public static void ClearCallbacks()
        {
            _actions = null;
        }

        public static void Raise<T>(T @event) where T : IDomainEvent
        {
            foreach(var handler in KernelHolder.Kernel.GetAll<IHandleEvent<T>>())
                handler.Handle(@event);
        }
    }
}
