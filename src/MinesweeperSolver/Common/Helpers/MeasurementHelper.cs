using MinesweeperSolver.Common.Constants;

namespace MinesweeperSolver.Common.Helpers
{
    public static class MeasurementHelper
    {
        public static int GetCoordByCol(int col, int cellOffsetX = 0)
        {
            return AppSettings.StartOfFieldX + col * AppSettings.CellStepPxl + cellOffsetX;
        }

        public static int GetCoordByRow(int row, int cellOffsetY = 0)
        {
            return AppSettings.StartOfFieldY + row * AppSettings.CellStepPxl + cellOffsetY;
        }
    }
}
