using System;
using MinesweeperSolver.Common.Events;

namespace MinesweeperSolver.Domain.Events
{
    public class UnexpectedErrorOccured : IDomainEvent
    {
        public UnexpectedErrorOccured(Exception error)
        {
            Error = error;
        }

        public Exception Error { get; private set; }
    }
}
