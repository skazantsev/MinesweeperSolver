using System;

namespace MinesweeperSolver.Domain.Exceptions
{
    public abstract class GameSolverException : Exception
    {
        protected GameSolverException()
        { }

        protected GameSolverException(string message)
            : base(message)
        { }
    }
}
