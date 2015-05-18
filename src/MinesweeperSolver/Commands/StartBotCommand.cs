using System;
using System.Collections.Generic;
using System.Linq;
using MinesweeperSolver.Common.Commands;
using MinesweeperSolver.Common.Events;
using MinesweeperSolver.Common.Exceptions;
using MinesweeperSolver.Domain;
using MinesweeperSolver.Domain.Events;
using MinesweeperSolver.Domain.Exceptions;
using MinesweeperSolver.Domain.Interfaces;

namespace MinesweeperSolver.Commands
{
    public class StartBotCommand : CommandWithOptionsBase
    {
        private readonly IMinesweeperBotFactory _botFactory;

        private GameOptions _gameOptions;

        public StartBotCommand(IMinesweeperBotFactory botFactory)
        {
            _botFactory = botFactory;
        }

        public override IEnumerable<string> Aliases
        {
            get { return new[] {"start"}; }
        }

        public override string Description
        {
            get
            {
                return "starts minesweeper solving\r\n" +
                       "    usage:\r\n" +
                       "\t* start beginner\r\n" +
                       "\t* start intermediate\r\n" +
                       "\t* start expert\r\n" +
                       "\t* start custom <height> <width> <mines>";
            }
        }

        public override void ParseOptions(string input)
        {
            var inputOptions = input.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();

            GameMode gameMode;
            if (!inputOptions.Any() || !Enum.TryParse(inputOptions[0], true, out gameMode))
                throw new InvalidCommandOptionsException("Unknown game mode.");

            if (gameMode == GameMode.Custom)
            {
                int height, width, mines;
                if (inputOptions.Count < 4)
                    throw new InvalidCommandOptionsException("Invalid number of parameters for custom mode.");

                if (!int.TryParse(inputOptions[1], out height) ||
                    !int.TryParse(inputOptions[2], out width) ||
                    !int.TryParse(inputOptions[3], out mines))
                    throw new InvalidCommandOptionsException("Cannot parse parameters for custom mode. Integers are required.");

                _gameOptions = GameOptions.BuildCustom(height, width, mines);
            }
            else
            {
                _gameOptions = GameOptions.GetPredefined(gameMode);
            }
        }

        public override void Run()
        {
            var minesweeperBot = _botFactory.Get();
            if (minesweeperBot.IsStarted)
            {
                DomainEvents.Raise(new BotAlreadyStarted());
                return;
            }

            try
            {
                minesweeperBot.Start(_gameOptions);
                DomainEvents.Raise(new BotStarted());
            }
            catch (WindowCannotBeFoundException)
            {
                DomainEvents.Raise(new GameCannotBeFound());
            }
        }
    }
}
