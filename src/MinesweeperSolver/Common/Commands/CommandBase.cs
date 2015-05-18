using System.Collections.Generic;
using System.Linq;

namespace MinesweeperSolver.Common.Commands
{
    public abstract class CommandBase : ICommand
    {
        public abstract IEnumerable<string> Aliases { get; }

        public abstract string Description { get; }

        public abstract void Run();

        public override string ToString()
        {
            return string.Format("{0}: {1}", Aliases.First(), Description);
        }
    }
}
