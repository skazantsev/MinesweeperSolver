using System.Collections.Generic;
using MinesweeperSolver.Common.Commands;
using MinesweeperSolver.Common.Events;
using MinesweeperSolver.Domain.Events;
using MinesweeperSolver.Domain.Interfaces;

namespace MinesweeperSolver.Commands
{
    public class StopBotCommand : CommandBase
    {
        private readonly IMinesweeperBotFactory _botFactory;

        public StopBotCommand(IMinesweeperBotFactory botFactory)
        {
            _botFactory = botFactory;
        }

        public override IEnumerable<string> Aliases
        {
            get { return new[] {"stop"}; }
        }

        public override string Description
        {
            get { return "stops minesweeper solving"; }
        }

        public override void Run()
        {
            var minesweeperBot = _botFactory.Get();
            if (!minesweeperBot.IsStarted)
            {
                DomainEvents.Raise(new BotIsNotStarted());
                return;
            }

            minesweeperBot.Stop();
            DomainEvents.Raise(new BotStopped());
        }
    }
}
