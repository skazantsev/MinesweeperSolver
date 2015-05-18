using MinesweeperSolver.Common.Commands;
using MinesweeperSolver.Common.Events;
using MinesweeperSolver.Common.Exceptions;
using MinesweeperSolver.Domain.Events;
using Ninject;
using System;
using System.Linq;

namespace MinesweeperSolver
{
    public static class Program
    {
        private static void KernelInit()
        {
            var kernel = new StandardKernel();
            kernel.Load(AppDomain.CurrentDomain.GetAssemblies()); // dynamic ninject module loading
            KernelHolder.Initialize(kernel);
        }

        public static void Main()
        {
            KernelInit();
            RunInputHandlingLoop();
        }

        private static void RunInputHandlingLoop()
        {
            var commands = KernelHolder.Kernel.GetAll<ICommand>().ToList();
            DomainEvents.Raise(new InstructionsAreRequested());
            do
            {
                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    DomainEvents.Raise(new InvalidCommandIsEntered());
                    continue;
                }

                var commandName = input.Split(' ')[0];
                var command = commands.FirstOrDefault(x => x.Aliases.Any(a => a.Equals(commandName, StringComparison.InvariantCultureIgnoreCase)));
                if (command == null)
                {
                    DomainEvents.Raise(new InvalidCommandIsEntered());
                    continue;
                }

                var optCmd = command as CommandWithOptionsBase;
                if (optCmd != null)
                {
                    try
                    {
                        optCmd.ParseOptions(input);
                    }
                    catch (InvalidCommandOptionsException ex)
                    {
                        DomainEvents.Raise(new InvalidOptionsArePassed(ex.Message));
                        continue;
                    }
                }

                command.Run();

            } while (true);
        }
    }
}
