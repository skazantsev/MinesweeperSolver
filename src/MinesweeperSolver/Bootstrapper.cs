using System;
using MinesweeperSolver.Common.Commands;
using MinesweeperSolver.Common.Events;
using MinesweeperSolver.Domain;
using MinesweeperSolver.Domain.Interfaces;
using Ninject.Extensions.Conventions;
using Ninject.Modules;

namespace MinesweeperSolver
{
    public class Bootstrapper : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IMinesweeperBotFactory>().To<MinesweeperBotFactory>().InSingletonScope();

            Kernel.Bind(x => x
                .From(AppDomain.CurrentDomain.GetAssemblies())
                .SelectAllClasses()
                .InheritedFrom<ICommand>()
                .BindSingleInterface()
                .Configure(c => c.InTransientScope()));

            Kernel.Bind(x => x
                .From(AppDomain.CurrentDomain.GetAssemblies())
                .SelectAllClasses()
                .InheritedFrom(typeof(IHandleEvent<>))
                .BindAllInterfaces()
                .Configure(c => c.InSingletonScope()));
        }
    }
}
