using MinesweeperSolver.Common.Constants;

namespace MinesweeperSolver.Common.Helpers
{
    public static class Coords
    {
        public static int ColToCoord(int col, int cellOffsetX = 0)
        {
            return AppSettings.StartOfFieldX + col * AppSettings.CellStepPxl + cellOffsetX;
        }

        public static int RowToCoord(int row, int cellOffsetY = 0)
        {
            return AppSettings.StartOfFieldY + row * AppSettings.CellStepPxl + cellOffsetY;
        }
    }
}
