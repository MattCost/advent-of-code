

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

        public void PopulateEdges(bool linkAnyValue = true)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    if (Nodes[row, col] == null)
                    {
                        continue;
                    }
                    if (row > 0 && (linkAnyValue || Nodes[row, col].Value!.Equals(Nodes[row - 1, col].Value)))
                    {
                        Nodes[row, col].Edges.Add(new GridNodeEdge<T>(Nodes[row - 1, col], Direction.Up));
                    }

                    if (row < Rows - 1 && (linkAnyValue || Nodes[row, col].Value!.Equals(Nodes[row + 1, col].Value)))
                    {
                        Nodes[row, col].Edges.Add(new GridNodeEdge<T>(Nodes[row + 1, col], Direction.Down));
                    }

                    if (col > 0 && (linkAnyValue || Nodes[row, col].Value!.Equals(Nodes[row, col - 1].Value)))
                    {
                        Nodes[row, col].Edges.Add(new GridNodeEdge<T>(Nodes[row, col - 1], Direction.Left));
                    }

                    if (col < Cols - 1 && (linkAnyValue || Nodes[row, col].Value!.Equals(Nodes[row, col + 1].Value)))
                    {
                        Nodes[row, col].Edges.Add(new GridNodeEdge<T>(Nodes[row, col + 1], Direction.Right));
                    }
                }
            }

        }

        public bool PointInBounds(int row, int col)
        {
            return row > 0 && col > 0 && row < Rows - 1 && col < Cols - 1;
        }

        public void PrintGrid(bool showVisited = false)
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    var node = Nodes[r, c];
                    if (showVisited && node.Visited)
                    {
                        var character = node.ExitDirection switch
                        {
                            Direction.Up => '^',
                            Direction.Down => 'v',
                            Direction.Left => '<',
                            Direction.Right => '>',
                            _ => '!'
                        };
                        Console.Write(character);
                    }
                    else
                    {
                        Console.Write(node.Value);
                    }
                }
                Console.Write('\n');
            }
        }
    }
}