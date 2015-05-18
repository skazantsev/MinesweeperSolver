using MinesweeperSolver.Common.Events;

namespace MinesweeperSolver.Domain.Events
{
    public class InvalidOptionsArePassed : IDomainEvent
    {
        public InvalidOptionsArePassed(string details)
        {
            Details = details;
        }

        public string Details { get; private set; }
    }
}
