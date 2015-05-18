using MinesweeperSolver.Domain.Interfaces;

namespace MinesweeperSolver.Domain
{
    public class MinesweeperBotFactory : IMinesweeperBotFactory
    {
        private MinesweeperBot _minesweeperBot;

        public MinesweeperBot Get()
        {
            return _minesweeperBot ?? (_minesweeperBot = new MinesweeperBot());
        }
    }
}
