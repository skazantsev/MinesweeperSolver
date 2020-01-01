using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinesweeperSolver.Common.Commands;
using MinesweeperSolver.Common.Events;
using MinesweeperSolver.Domain.Events;

namespace MinesweeperSolver.Presentation
{
    public class UserNotifier :
        IHandleEvent<BotStarted>,
        IHandleEvent<BotIsAlreadyStarted>,
        IHandleEvent<GameCannotBeFound>,
        IHandleEvent<BotStopped>,
        IHandleEvent<BotIsNotStarted>,
        IHandleEvent<GameIsLost>,
        IHandleEvent<GameIsSolved>,
        IHandleEvent<ExitIsRequested>,
        IHandleEvent<InstructionsAreRequested>,
        IHandleEvent<InvalidCommandIsEntered>,
        IHandleEvent<InvalidOptionsArePassed>,
        IHandleEvent<UnexpectedErrorOccured>
    {
        private readonly List<ICommand> _commands; 
        public UserNotifier(List<ICommand> commands)
        {
            _commands = commands;
        }

        public void Handle(BotStarted @event)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("The bot started.");
            Console.ResetColor();
        }

        public void Handle(BotIsAlreadyStarted @event)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The bot already started. You can stop it using 'stop' command.");
            Console.ResetColor();
        }

        public void Handle(GameCannotBeFound @event)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Cannot find a minesweeper window.");
            Console.ResetColor();
        }

        public void Handle(BotIsNotStarted @event)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The bot is not started. You can start it using 'start' command.");
            Console.ResetColor();
        }

        public void Handle(BotStopped @event)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("The bot stopped.");
            Console.ResetColor();
        }

        public void Handle(GameIsLost @event)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The game is lost :(");
            Console.ResetColor();
        }

        public void Handle(GameIsSolved @event)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("The game is solved.");
            Console.ResetColor();
        }

        public void Handle(ExitIsRequested @event)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Bye-bye.");
            Console.ResetColor();
        }

        public void Handle(InstructionsAreRequested @event)
        {
            PrintInstructions();
        }

        public void Handle(InvalidCommandIsEntered @event)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid command!");
            Console.ResetColor();
            PrintInstructions();
        }

        public void Handle(InvalidOptionsArePassed @event)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(@event.Details);
            Console.ResetColor();
            PrintInstructions();
        }

        public void Handle(UnexpectedErrorOccured @event)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Oops! An error has occured.");
            Console.WriteLine("Message: {0}", @event.Error.Message);
            Console.WriteLine("Stack trace: {0}", @event.Error.StackTrace);
            Console.ResetColor();
        }

        private void PrintInstructions()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            var commandDescription = _commands.Aggregate(
                new StringBuilder(),
                (sb, cmd) => sb.AppendFormat("  - {0}\r\n", cmd));
            Console.WriteLine("-=COMMANDS=-\r\n{0}", commandDescription);
            Console.ResetColor();
        }
    }
}