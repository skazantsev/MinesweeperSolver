using System.Drawing;
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
            var coordX = Coords.ColToCoord(e.Col);
            var coordY = Coords.RowToCoord(e.Row);
            WinApiHelper.MouseClick(_minesweeperWindow, MouseButton.Right, coordX, coordY);
        }

        private void NextCellHasBeenFoundHandler(object sender, GameSolver.CellLocationEventArgs e)
        {
            var coordX = Coords.ColToCoord(e.Col);
            var coordY = Coords.RowToCoord(e.Row);
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

        private CellState GetCellState(int row, int col)
        {
            var cellState =
                CellStateDeterminationList
                    .Where(x => x.Key.Color == WinApiHelper.GetColor(_minesweeperWindow, x.Key.XPointGetter(col), x.Key.YPointGetter(row)))
                    .Select(x => x.Value)
                    .Cast<CellState?>()
                    .FirstOrDefault();

            return cellState != null ? cellState.Value : CellState.Unknown;
        }
    }
}