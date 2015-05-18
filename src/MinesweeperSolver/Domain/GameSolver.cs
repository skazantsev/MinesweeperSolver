using System;
using System.Linq;
using MinesweeperSolver.Common.BclExtensions;
using MinesweeperSolver.Domain.Exceptions;

namespace MinesweeperSolver.Domain
{
    public class GameSolver
    {
        private readonly Map _map;

        private readonly int _mineCount;

        private readonly Random _random;

        public GameSolver(Map map, int mineCount)
        {
            _map = map;
            _mineCount = mineCount;
            _random = new Random();
        }

        public event EventHandler<CellLocationEventArgs> MineHasBeenFound;

        public event EventHandler<CellLocationEventArgs> NextCellHasBeenFound;

        public event EventHandler<EventArgs> GameSolved;

        public event EventHandler<EventArgs> GameLost;

        public void CalculateNextStep()
        {
            FindMines();

            if (CheckIfGameIsEnded())
                return;

            if (FindSolvedCells() || FindNextCellWithBestProbability() || FindRandomCell())
                return;

            throw new CannotFindNextCellException();
        }

        private bool CheckIfGameIsEnded()
        {
            if (_map.GetCells().Any(x => x.State == CellState.Boom))
            {
                GameLost.Raise(this, EventArgs.Empty);
                return true;
            }

            if (_map.GetCells().All(x => x.State != CellState.Unknown))
            {
                GameSolved.Raise(this, EventArgs.Empty);
                return true;
            }
            return false;
        }

        private void FindMines()
        {
            foreach (var cell in _map.GetCells())
            {
                if (cell.Number.HasValue &&
                    _map.GetAdjacentCellCountOfState(cell, new[] { CellState.Unknown, CellState.Mine }) == cell.Number.Value)
                {
                    foreach (var adjacentCell in _map.GetAdjacentCells(cell, x => x.State == CellState.Unknown))
                    {
                        MineHasBeenFound.Raise(this, new CellLocationEventArgs(adjacentCell.Row, adjacentCell.Col));
                    }
                }
            }
        }

        private bool FindSolvedCells()
        {
            var foundAny = false;
            foreach (var cell in _map.GetCells())
            {
                if (cell.Number.HasValue
                    && _map.GetAdjacentCellCountOfState(cell, CellState.Mine) == cell.Number.Value)
                {
                    var adjacentCells = _map.GetAdjacentCells(cell, x => x.State == CellState.Unknown);
                    foundAny = adjacentCells.Any();
                    foreach (var adjCell in adjacentCells)
                    {
                        NextCellHasBeenFound.Raise(this, new CellLocationEventArgs(adjCell.Row, adjCell.Col));
                    }
                }
            }
            return foundAny;
        }

        private bool FindNextCellWithBestProbability()
        {
            var bestProbability = 0d;
            var randomProbability = CalculateRandomProbability();
            Cell bestCell = null;
            foreach (var cell in _map.GetCells())
            {
                if (cell.Number.HasValue)
                {
                    var adjacentUnknown = (double) _map.GetAdjacentCellCountOfState(cell, CellState.Unknown);
                    var adjacentUnknownMines = (double) (cell.Number.Value - _map.GetAdjacentCellCountOfState(cell, CellState.Mine));
                    if (adjacentUnknown <= 0)
                        continue;

                    var currentCellProbability = 1d - (adjacentUnknownMines / adjacentUnknown);
                    if (currentCellProbability > bestProbability && currentCellProbability > randomProbability)
                    {
                        bestProbability = currentCellProbability;
                        bestCell = _map.GetAdjacentCells(cell, x => x.State == CellState.Unknown).First();
                    }
                }
            }

            if (bestCell != null)
            {
                NextCellHasBeenFound.Raise(this, new CellLocationEventArgs(bestCell.Row, bestCell.Col));
            }
            return bestCell != null;
        }

        private double CalculateRandomProbability()
        {
            var minesLeft = (double)(_mineCount - _map.GetCells().Count(x => x.State == CellState.Mine));
            var unknownCellCount = _map.GetCells().Count(x => x.State == CellState.Unknown);
            return 1d - minesLeft/unknownCellCount;
        }

        private bool FindRandomCell()
        {
            var unknownCells = _map.GetCells().Where(x => x.State == CellState.Unknown).ToList();
            if (!unknownCells.Any())
                return false;

            var randomCell = unknownCells.ElementAt(_random.Next(0, unknownCells.Count - 1));
            NextCellHasBeenFound.Raise(this, new CellLocationEventArgs(randomCell.Row, randomCell.Col));
            return true;
        }

        public class CellLocationEventArgs : EventArgs
        {
            public int Row { get; set; }

            public int Col { get; set; }

            public CellLocationEventArgs(int row, int col)
            {
                Row = row;
                Col = col;
            }
        }
    }
}