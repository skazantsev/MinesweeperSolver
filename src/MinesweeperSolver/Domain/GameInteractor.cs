using MinesweeperSolver.Common.BclExtensions;
using MinesweeperSolver.Common.Enums;
using MinesweeperSolver.Common.Helpers;
using MinesweeperSolver.Common.WinApiConstants;
using MinesweeperSolver.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MinesweeperSolver.Domain
{
    public class GameInteractor
    {
        private static readonly KeyValueList<ColorPoint, CellState> CellStateDeterminationList = new KeyValueList<ColorPoint, CellState>
        {
            {ColorPoint.Create(c => Coords.ColToCoord(c), r => Coords.RowToCoord(r), Color.FromArgb(255, 0, 0)), CellState.Boom },

            {ColorPoint.Create(c => Coords.ColToCoord(c, 7), r => Coords.RowToCoord(r, 8), Color.FromArgb(0, 0, 255)), CellState.One },

            {ColorPoint.Create(c => Coords.ColToCoord(c, 7), r => Coords.RowToCoord(r, 8), Color.FromArgb(0, 128, 0)), CellState.Two },

            {ColorPoint.Create(c => Coords.ColToCoord(c, 10), r => Coords.RowToCoord(r, 8), Color.FromArgb(255, 0, 0)), CellState.Three },

            {ColorPoint.Create(c => Coords.ColToCoord(c, 10), r => Coords.RowToCoord(r, 8), Color.FromArgb(0, 0, 128)), CellState.Four },

            {ColorPoint.Create(c => Coords.ColToCoord(c, 10), r => Coords.RowToCoord(r, 8), Color.FromArgb(128, 0, 0)), CellState.Five },

            {ColorPoint.Create(c => Coords.ColToCoord(c, 10), r => Coords.RowToCoord(r, 8), Color.FromArgb(0, 128, 128)), CellState.Six },

            {ColorPoint.Create(c => Coords.ColToCoord(c, 3), r => Coords.RowToCoord(r, 2), Color.FromArgb(0, 0, 0)), CellState.Seven },

            {ColorPoint.Create(c => Coords.ColToCoord(c, 10), r => Coords.RowToCoord(r, 8), Color.FromArgb(128, 128, 128)), CellState.Eight },

            {ColorPoint.Create(c => Coords.ColToCoord(c), r => Coords.RowToCoord(r), Color.FromArgb(192, 192, 192)), CellState.Empty }
        };

        private readonly IntPtr _minesweeperWindow;

        private Dictionary<int, Color> _colorsCache;

        public GameInteractor(IEnumerable<string> gameWindowNames)
        {
            _minesweeperWindow = GetMinesweeperWindowPtr(gameWindowNames);
        }

        public void NewGame()
        {
            WinApiHelper.PostMessage(_minesweeperWindow, WindowMessages.WM_KEYDOWN, (IntPtr)Keys.F2, IntPtr.Zero);
        }

        public void UpdateCellStates(IEnumerable<Cell> cells)
        {
            _colorsCache = new Dictionary<int, Color>();
            var hdc = IntPtr.Zero;
            try
            {
                hdc = WinApiHelper.GetDC(_minesweeperWindow);
                cells.ForEach(x => UpdateCellState(x, hdc));
            }
            finally
            {
                if (hdc != IntPtr.Zero)
                {
                    WinApiHelper.ReleaseDC(_minesweeperWindow, hdc);
                }
            }
        }

        public void TickMine(int row, int col)
        {
            WinApiHelper.MouseClick(_minesweeperWindow, MouseButton.Right, Coords.ColToCoord(col), Coords.RowToCoord(row));
        }

        public void OpenCell(int row, int col)
        {
            WinApiHelper.MouseClick(_minesweeperWindow, MouseButton.Left, Coords.ColToCoord(col), Coords.RowToCoord(row));
        }

        private static IntPtr GetMinesweeperWindowPtr(IEnumerable<string> gameWindowNames)
        {
            foreach (var wndName in gameWindowNames)
            {
                var hWnd = WinApiHelper.FindWindow(null, wndName);
                if (hWnd != IntPtr.Zero)
                    return hWnd;
            }

            throw new WindowCannotBeFoundException();
        }

        private void UpdateCellState(Cell cell, IntPtr hdc)
        {
            foreach (var kvp in CellStateDeterminationList)
            {
                Color color;
                var cacheKey = kvp.Key.XPointGetter(cell.Col) * 100 + kvp.Key.YPointGetter(cell.Row);
                if (!_colorsCache.TryGetValue(cacheKey, out color))
                {
                    color = WinApiHelper.GetColor(hdc, kvp.Key.XPointGetter(cell.Col), kvp.Key.YPointGetter(cell.Row));
                    _colorsCache[cacheKey] = color;
                }

                if (color != kvp.Key.Color)
                    continue;

                cell.UpdateState(kvp.Value);
                break;
            }
        }
    }
}
