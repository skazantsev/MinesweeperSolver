using System;
using System.Collections.Generic;
using MinesweeperSolver.Common.Commands;
using MinesweeperSolver.Common.Events;
using MinesweeperSolver.Domain.Events;

namespace MinesweeperSolver.Commands
{
    public class ExitProgramCommand : CommandBase
    {
        public override IEnumerable<string> Aliases
        {
            get { return new[] {"exit", "quit", "q"}; }
        }

        public override string Description
        {
            get { return "exits the program"; }
        }

        public override void Run()
        {
            DomainEvents.Raise(new ExitIsRequested());
            Environment.Exit(0);
        }
    }
}
