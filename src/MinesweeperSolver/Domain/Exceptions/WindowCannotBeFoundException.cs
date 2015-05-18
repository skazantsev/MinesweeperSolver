using System;

namespace MinesweeperSolver.Domain.Exceptions
{
    public class WindowCannotBeFoundException : Exception
    {
        public WindowCannotBeFoundException()
            :this("Window cannot be found.")
        { }

        public WindowCannotBeFoundException(string message)
            :base(message)
        { }
    }
}
