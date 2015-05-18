using System.Collections.Generic;
using MinesweeperSolver.Common.BclExtensions;
using MinesweeperSolver.Common.Constants;
using MinesweeperSolver.Common.Enums;
using MinesweeperSolver.Common.Events;
using MinesweeperSolver.Common.Helpers;
using MinesweeperSolver.Common.WinApiConstants;
using MinesweeperSolver.Domain.Events;
using MinesweeperSolver.Domain.Exceptions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinesweeperSolver.Domain
{
    public class MinesweeperBot
    {
        private string[] _gameWindowNames;

        private IntPtr _minesweeperWindow;
        private Map _map;
        private GameSolver _solver;

        public bool IsStarted { get; private set; }

        public void Start(GameOptions gameOptions, string[] gameWindowNames)
        {
            _gameWindowNames = gameWindowNames;
            _minesweeperWindow = GetMinesweeperWindowPtr();
            _map = new Map(gameOptions.Height, gameOptions.Width);
            _solver = new GameSolver(_map, gameOptions.Mines);
            _solver.MineHasBeenFound += MineHasBeenFoundHandler;
            _solver.NextCellHasBeenFound += NextCellHasBeenFoundHandler;
            _solver.GameSolved += GameSolvedHandler;
            _solver.GameLost += GameLostHandler;

            IsStarted = true;
            WinApiHelper.PostMessage(_minesweeperWindow, WindowMessages.WM_KEYDOWN, (IntPtr)Keys.F2, IntPtr.Zero);
            Task.Factory.StartNew(BeginMapPollingLoop);
        }

        public void Stop()
        {
            WinApiHelper.PostMessage(_minesweeperWindow, WindowMessages.WM_KEYDOWN, (IntPtr)Keys.F2, IntPtr.Zero);
            Complete();
        }

        private void Complete()
        {
            IsStarted = false;
            CleanUp();
        }

        private void CleanUp()
        {
            _solver.MineHasBeenFound -= MineHasBeenFoundHandler;
            _solver.NextCellHasBeenFound -= NextCellHasBeenFoundHandler;
            _solver.GameSolved -= GameSolvedHandler;
            _solver.GameLost -= GameLostHandler;
            _solver = null;
            _map = null;
            _minesweeperWindow = IntPtr.Zero;
        }

        private IntPtr GetMinesweeperWindowPtr()
        {
            foreach (var wndName in _gameWindowNames)
            {
                var hWnd = WinApiHelper.FindWindow(null, wndName);
                if (hWnd != IntPtr.Zero)
                    return hWnd;
            }

            throw new WindowCannotBeFoundException();
        }

        private void BeginMapPollingLoop()
        {
            try
            {
                Thread.Sleep(AppSettings.MapStatePollingInterval);
                while (true)
                {
                    if (!IsStarted)
                        break;

                    UpdateMapState();
                    _solver.CalculateNextStep();
                }
            }
            catch (Exception ex)
            {
                Complete();
                DomainEvents.Raise(new UnexpectedErrorOccured(ex));
            }
        }

        private void UpdateMapState()
        {
            _map.GetCells().Where(x => x.State == CellState.Unknown).ForEach(x => x.UpdateState(GetCellState(x.Row, x.Col)));
        }

        private void MineHasBeenFoundHandler(object sender, GameSolver.CellLocationEventArgs e)
        {
            _map.GetCellAt(e.Row, e.Col).UpdateState(CellState.Mine);
            var coordX = MeasurementHelper.GetCoordByCol(e.Col);
            var coordY = MeasurementHelper.GetCoordByRow(e.Row);
            WinApiHelper.MouseClick(_minesweeperWindow, MouseButton.Right, coordX, coordY);
        }

        private void NextCellHasBeenFoundHandler(object sender, GameSolver.CellLocationEventArgs e)
        {
            var coordX = MeasurementHelper.GetCoordByCol(e.Col);
            var coordY = MeasurementHelper.GetCoordByRow(e.Row);
            WinApiHelper.MouseClick(_minesweeperWindow, MouseButton.Left, coordX, coordY);
        }

        private void GameSolvedHandler(object sender, EventArgs e)
        {
            Complete();
            DomainEvents.Raise(new GameIsSolved());
        }

        private void GameLostHandler(object sender, EventArgs e)
        {
            Complete();
            DomainEvents.Raise(new GameIsLost());
        }

        public CellState GetCellState(int row, int col)
        {
            var color = WinApiHelper.GetColor(_minesweeperWindow, MeasurementHelper.GetCoordByCol(col), MeasurementHelper.GetCoordByRow(row));
            if (color.R == 255 && color.G == 0 && color.B == 0)
            {
                return CellState.Boom;
            }

            color = WinApiHelper.GetColor(_minesweeperWindow, MeasurementHelper.GetCoordByCol(col, 7), MeasurementHelper.GetCoordByRow(row, 8));
            if (color.R == 0 && color.G == 0 && color.B == 255)
            {
                return CellState.One;
            }
            if (color.R == 0 && color.G == 128 && color.B == 0)
            {
                return CellState.Two;
            }

            color = WinApiHelper.GetColor(_minesweeperWindow, MeasurementHelper.GetCoordByCol(col, 10), MeasurementHelper.GetCoordByRow(row, 8));
            if (color.R == 255 && color.G == 0 && color.B == 0)
            {
                return CellState.Three;
            }
            if (color.R == 0 && color.G == 0 && color.B == 128)
            {
                return CellState.Four;
            }
            if (color.R == 128 && color.B == 0 && color.G == 0)
            {
                return CellState.Five;
            }
            if (color.R == 0 && color.B == 128 && color.G == 128)
            {
                return CellState.Six;
            }
            if (color.R == 128 && color.B == 128 && color.G == 128)
            {
                return CellState.Eight;
            }

            color = WinApiHelper.GetColor(_minesweeperWindow, MeasurementHelper.GetCoordByCol(col, 3), MeasurementHelper.GetCoordByRow(row, 2));
            if (color.R == 0 && color.B == 128 && color.G == 128)
            {
                return CellState.Seven;
            }

            color = WinApiHelper.GetColor(_minesweeperWindow, MeasurementHelper.GetCoordByCol(col), MeasurementHelper.GetCoordByRow(row));
            if (color.R == 192 && color.G == 192 && color.B == 192)
            {
                return CellState.Empty;
            }

            return CellState.Unknown;
        }
    }
}