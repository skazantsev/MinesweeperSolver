using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MinesweeperSolver.Domain
{
    public class GameOptions
    {
        private static readonly IEnumerable<GameOptions> PredefinedModes;

        static GameOptions()
        {
            PredefinedModes = new List<GameOptions>
            {
                new GameOptions {Mode = GameMode.Beginner, Height = 9, Width = 9, Mines = 10},
                new GameOptions {Mode = GameMode.Intermediate, Height = 16, Width = 16, Mines = 40},
                new GameOptions {Mode = GameMode.Expert, Height = 16, Width = 30, Mines = 99}
            };
        }

        public GameMode Mode { get; private set; }

        public int Height { get; private set; }

        public int Width { get; private set; }

        public int Mines { get; private set; }

        public static GameOptions BuildCustom(int height, int width, int mines)
        {
            return new GameOptions {Height = height, Width = width, Mines = mines};
        }

        public static GameOptions GetPredefined(GameMode mode)
        {
            var gameOptions = PredefinedModes.FirstOrDefault(x => x.Mode == mode);
            if (gameOptions == null)
                throw new InvalidEnumArgumentException("The mode has not been found among predefined ones.");
            return gameOptions;
        }
    }
}
