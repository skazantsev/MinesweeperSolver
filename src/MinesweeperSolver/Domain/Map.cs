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
            return GetAdjacentCellCountOfState(cell.Row, cell.Col, states);
        }

        private int GetAdjacentCellCountOfState(int row, int col, params CellState[] states)
        {
            return GetAdjacentCells(row, col).Count(x => states.Any(s => s == x.State));
        }

        public List<Cell> GetAdjacentCells(Cell cell, Func<Cell, bool> predicate)
        {
            return GetAdjacentCells(cell.Row, cell.Col).Where(predicate).ToList();
        }

        public List<Cell> GetAdjacentCells(Cell cell)
        {
            return GetAdjacentCells(cell.Row, cell.Col);
        }

        // TODO: Simplify this method
        // http://stackoverflow.com/a/2373480/883623
        private List<Cell> GetAdjacentCells(int row, int col)
        {
            var adjacentCells = new List<Cell>();

            if (row != 0)
                adjacentCells.Add(_gameField[row - 1, col]);
            if (row != 0 && col != _colCount - 1)
                adjacentCells.Add(_gameField[row - 1, col + 1]);
            if (col != _colCount - 1)
                adjacentCells.Add(_gameField[row, col + 1]);
            if (row != _rowCount - 1 && col != _colCount - 1)
                adjacentCells.Add(_gameField[row + 1, col + 1]);
            if (row != _rowCount - 1)
                adjacentCells.Add(_gameField[row + 1, col]);
            if (row != _rowCount - 1 && col != 0)
                adjacentCells.Add(_gameField[row + 1, col - 1]);
            if (col != 0)
                adjacentCells.Add(_gameField[row, col - 1]);
            if (row != 0 && col != 0)
                adjacentCells.Add(_gameField[row - 1, col - 1]);

            return adjacentCells;
        }
    }
}