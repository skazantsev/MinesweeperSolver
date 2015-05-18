using System;

namespace MinesweeperSolver.Common.BclExtensions
{
    public static class EventExtensions
    {
        // Thread-safe event raising.
        // http://stackoverflow.com/a/803113/883623
        public static void Raise<T>(this EventHandler<T> theEvent, object source, T args) where T : EventArgs
        {
            if (theEvent != null)
            {
                theEvent(source, args);
            }
        }
    }
}
