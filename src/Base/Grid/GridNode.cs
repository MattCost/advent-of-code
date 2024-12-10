namespace AdventOfCode.Base.Grid
{
    public class GridNode<T>
    {
        public T Value { get; init; }
        public int Row { get; init; }
        public int Col { get; init; }

        public GridNode(T value, int row, int col)
        {
            Value = value;
            Row = row;
            Col = col;
        }

        public List<GridNodeEdge<T>> Edges { get; init; } = new();
    }
    public class GridNodeEdge<T>
    {
        public Direction Direction { get; init; }
        public GridNode<T> Node { get; init; }
        public GridNodeEdge(GridNode<T> node, Direction direction)
        {
            Direction = direction;
            Node = node;
        }
    }    
}