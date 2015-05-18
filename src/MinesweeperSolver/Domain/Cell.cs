namespace MinesweeperSolver.Domain
{
    public class Cell
    {
        public Cell(int row, int col, CellState state)
        {
            State = state;
            Number = TryGetCellNumber(state);
            Row = row;
            Col = col;
        }

        public CellState State { get; private set; }

        public int? Number { get; private set; }

        public int Row { get; set; }

        public int Col { get; set; }

        public override string ToString()
        {
            return State.ToString();
        }

        public void UpdateState(CellState state)
        {
            State = state;
            Number = TryGetCellNumber(state);
        }

        private int? TryGetCellNumber(CellState state)
        {
            switch (state)
            {
                case CellState.One:
                    return 1;
                case CellState.Two:
                    return 2;
                case CellState.Three:
                    return 3;
                case CellState.Four:
                    return 4;
                case CellState.Five:
                    return 5;
                case CellState.Six:
                    return 6;
                case CellState.Seven:
                    return 7;
                case CellState.Eight:
                    return 8;
                default:
                    return null;
            }
        }
    }
}
