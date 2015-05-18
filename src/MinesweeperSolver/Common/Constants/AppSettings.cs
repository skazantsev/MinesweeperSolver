using System;

namespace MinesweeperSolver.Common.Constants
{
    public static class AppSettings
    {
        public static readonly TimeSpan MapStatePollingInterval = TimeSpan.FromMilliseconds(10);

        public static readonly int CellStepPxl = 16;

        public static readonly int StartOfFieldX = 13;

        public static readonly int StartOfFieldY = 56;
    }
}
