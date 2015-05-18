using System;

namespace MinesweeperSolver.Common.Exceptions
{
    public class InvalidCommandOptionsException : Exception
    {
        public InvalidCommandOptionsException()
            :this("Invalid command options.")
        { }

        public InvalidCommandOptionsException(string message)
            :base(message)
        { }
    }
}
