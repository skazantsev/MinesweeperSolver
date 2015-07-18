using MinesweeperSolver.Common.BclExtensions;
using MinesweeperSolver.Common.Constants;
using MinesweeperSolver.Common.Events;
using MinesweeperSolver.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MinesweeperSolver.Domain
{
    public class MinesweeperBot
    {
        private GameInteractor _gameInteractor;
        private Map _map;
        private GameSolver _solver;

        public bool IsStarted { get; private set; }

        public void Start(GameOptions gameOptions, IEnumerable<string> gameWindowNames)
        {
            _gameInteractor = new GameInteractor(gameWindowNames);
            _map = new Map(gameOptions.Height, gameOptions.Width);
            _solver = new GameSolver(_map, gameOptions.Mines);
            _solver.MineHasBeenFound += MineHasBeenFoundHandler;
            _solver.NextCellHasBeenFound += NextCellHasBeenFoundHandler;
            _solver.GameSolved += GameSolvedHandler;
            _solver.GameLost += GameLostHandler;

            IsStarted = true;
            _gameInteractor.NewGame();
            Task.Factory.StartNew(BeginMapPollingLoop);
        }

        public void Stop()
        {
            _gameInteractor.NewGame();
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
            var cellsToUpdate = _map.GetCells().Where(x => x.State == CellState.Unknown);
            _gameInteractor.UpdateCellStates(cellsToUpdate);
        }

        private void MineHasBeenFoundHandler(object sender, GameSolver.CellLocationEventArgs e)
        {
            _map.GetCellAt(e.Row, e.Col).UpdateState(CellState.Mine);
            _gameInteractor.TickMine(e.Row, e.Col);
        }

        private void NextCellHasBeenFoundHandler(object sender, GameSolver.CellLocationEventArgs e)
        {
            _gameInteractor.OpenCell(e.Row, e.Col);
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
    }
}