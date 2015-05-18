namespace MinesweeperSolver.Common.Events
{
    public interface IHandleEvent<in T> where T : IDomainEvent
    {
        void Handle(T @event);
    }
}
