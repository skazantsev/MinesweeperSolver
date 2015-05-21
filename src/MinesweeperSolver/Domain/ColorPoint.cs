using System;
using System.Drawing;

namespace MinesweeperSolver.Domain
{
    public class ColorPoint
    {
        private ColorPoint(Func<int, int> xPointGetter, Func<int, int> yPointGetter, Color color)
        {
            XPointGetter = xPointGetter;
            YPointGetter = yPointGetter;
            Color = color;
        }

        public Func<int, int> XPointGetter { get; private set; }

        public Func<int, int> YPointGetter { get; private set; }

        public Color Color { get; private set; }

        public static ColorPoint Create(Func<int, int> xPointGetter, Func<int, int> yPointGetter, Color color)
        {
            return new ColorPoint(xPointGetter, yPointGetter, color);
        }
    }
}
