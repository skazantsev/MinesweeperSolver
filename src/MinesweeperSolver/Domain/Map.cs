using System;
using System.Collections.Generic;
using System.Linq;

namespace MinesweeperSolver.Domain
{
    public class Map
    {
        private readonly int _rowCount;
        private readonly int _colCount;

        private readonly Cell[,] _gameField;

        private readonly IReadOnlyCollection<Cell> _cells;

        public Map(int fieldHeight, int fieldWidth)
        {
            _rowCount = fieldHeight;
            _colCount = fieldWidth;
            _gameField = new Cell[_rowCount, _colCount];
            for (var row = 0; row < _rowCount; ++row)
            {
                for (var col = 0; col < _colCount; ++col)
                {
                    _gameField[row, col] = new Cell(row, col, CellState.Unknown);
                }
            }
            _cells = _gameField.Cast<Cell>().ToList().AsReadOnly();
        }

        public IReadOnlyCollection<Cell> GetCells()
        {
            return _cells;
        }

        public Cell GetCellAt(int row, int col)
        {
            return _gameField[row, col];
        }

        public int GetAdjacentCellCountOfState(Cell cell, params CellState[] states)
        {
            return GetAdjacentCells(cell).Count(x => states.Any(s => s == x.State));
        }

        public List<Cell> GetAdjacentCells(Cell cell, Func<Cell, bool> predicate)
        {
            return GetAdjacentCells(cell).Where(predicate).ToList();
        }

        private IEnumerable<Cell> GetAdjacentCells(Cell cell)
        {
            var row = cell.Row;
            var col = cell.Col;
            var adjacency = Enumerable.Range(-1, 3)
                .SelectMany(x => Enumerable.Range(-1, 3), (x, y) => new {dx = x, dy = y})
                .Where(p => p.dx != 0 || p.dy != 0);

            foreach (var adj in adjacency)
            {
                if (0 <= row + adj.dx && row + adj.dx < _rowCount &&
                    0 <= col + adj.dy && col + adj.dy < _colCount)
                    yield return _gameField[row + adj.dx, col + adj.dy];
            }

        }
    }
}