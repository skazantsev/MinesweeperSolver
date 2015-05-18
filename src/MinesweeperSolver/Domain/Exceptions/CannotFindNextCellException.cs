namespace MinesweeperSolver.Domain.Exceptions
{
    public class CannotFindNextCellException : GameSolverException
    {
        public CannotFindNextCellException()
            :this("Cannot find the next cell to click. It's probably an error in code.")
        { }

        public CannotFindNextCellException(string message)
            : base(message)
        { }
    }
}
