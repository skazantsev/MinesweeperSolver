namespace MinesweeperSolver.Common.Commands
{
    public abstract class CommandWithOptionsBase : CommandBase
    {
        public abstract void ParseOptions(string input);
    }
}
