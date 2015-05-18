using System.Collections.Generic;

namespace MinesweeperSolver.Common.Commands
{
    // Usually it's better to separate command definitiion from it's execution.
    // Too many abstractions for small apps though.
    public interface ICommand
    {
        IEnumerable<string> Aliases { get; }
        string Description { get; }
        void Run();
    }
}