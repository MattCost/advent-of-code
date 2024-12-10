namespace AdventOfCode.Base.Grid
{

    public class Grid<T>
    {
        public GridNode<T>[,] Nodes { get; init; }
        public int Rows { get; init; }
        public int Cols { get; init; }

        public Grid(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Nodes = new GridNode<T>[rows, cols];
        }

        public void AddNode(T value, int row, int col)
        {
            Nodes[row, col] = new GridNode<T>(value, row, col);
        }

        public void PopulateEdges()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    if (Nodes[row, col] == null)
                    {
                        continue;
                    }
                    if (row > 0)
                    {
                        Nodes[row, col].Edges.Add(new GridNodeEdge<T>(Nodes[row - 1, col], Direction.Up));
                    }

                    if (row < Rows - 1)
                    {
                        Nodes[row, col].Edges.Add(new GridNodeEdge<T>(Nodes[row + 1, col], Direction.Down));
                    }

                    if (col > 0)
                    {
                        Nodes[row, col].Edges.Add(new GridNodeEdge<T>(Nodes[row, col - 1], Direction.Left));
                    }

                    if (col < Cols - 1)
                    {
                        Nodes[row, col].Edges.Add(new GridNodeEdge<T>(Nodes[row, col + 1], Direction.Right));
                    }
                }
            }

        }
    }


}