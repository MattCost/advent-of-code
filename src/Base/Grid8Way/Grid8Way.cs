namespace AdventOfCode.Base.Grid8Way
{
    public enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight
    }

    public class Grid8WayNode<T>
    {
        public T Value { get; init; }
        public int Row { get; init; }
        public int Col { get; init; }
        public bool Visited { get; set; }
        public List<Direction> PastVisitDirections { get; set; } = new();

        public Grid8WayNode(T value, int row, int col)
        {
            Value = value;
            Row = row;
            Col = col;
        }

        public List<Grid8WayEdge<T>> Edges { get; init; } = new();
    }

    public class Grid8WayEdge<T>
    {
        public Direction Direction { get; init; }
        public Grid8WayNode<T> Node { get; init; }
        public Grid8WayEdge(Grid8WayNode<T> node, Direction direction)
        {
            Direction = direction;
            Node = node;
        }

    }
    public class Grid8Way<T>
    {
        public Grid8WayNode<T>[,] Nodes { get; init; }
        public int Rows { get; init; }
        public int Cols { get; init; }

        public Grid8Way(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Nodes = new Grid8WayNode<T>[rows, cols];
        }

        public void AddNode(T value, int row, int col)
        {
            Nodes[row, col] = new Grid8WayNode<T>(value, row, col);
        }



        public void PrintGrid(int startR = -1, int startC = -1)
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (r == startR && c == startC)
                    {
                        Console.Write("^"); //assuming up, bah
                    }
                    else
                    {
                        var node = Nodes[r, c];
                        if (!node.Visited)
                        {
                            Console.Write($"{Nodes[r, c].Value}");
                        }
                        else
                        {
                            var upDown = node.PastVisitDirections.Contains(Direction.Up) || node.PastVisitDirections.Contains(Direction.Down);
                            var leftRight = node.PastVisitDirections.Contains(Direction.Left) || node.PastVisitDirections.Contains(Direction.Right);

                            if (upDown && leftRight)
                                Console.Write($"+");
                            else if (upDown)
                                Console.Write($"|");
                            else
                                Console.Write($"-");
                        }
                    }
                }
                Console.Write("\n");
            }
        }
        public void PopulateEdges()
        {
            // Link up the nodes
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    if (Nodes[row, col] == null)
                    {
                        continue;
                    }

                    // Ups
                    if (row > 0)
                    {
                        Nodes[row, col].Edges.Add(new Grid8WayEdge<T>(Nodes[row - 1, col], Direction.Up));

                        if (col > 0)
                        {
                            Nodes[row, col].Edges.Add(new Grid8WayEdge<T>(Nodes[row - 1, col - 1], Direction.UpLeft));
                        }
                        if (col < Cols - 1)
                        {
                            Nodes[row, col].Edges.Add(new Grid8WayEdge<T>(Nodes[row - 1, col + 1], Direction.UpRight));
                        }
                    }

                    // Downs
                    if (row < Rows - 1)
                    {
                        Nodes[row, col].Edges.Add(new Grid8WayEdge<T>(Nodes[row + 1, col], Direction.Down));

                        if (col > 0)
                        {
                            Nodes[row, col].Edges.Add(new Grid8WayEdge<T>(Nodes[row + 1, col - 1], Direction.DownLeft));
                        }
                        if (col < Cols - 1)
                        {
                            Nodes[row, col].Edges.Add(new Grid8WayEdge<T>(Nodes[row + 1, col + 1], Direction.DownRight));
                        }
                    }

                    // Lefts
                    if (col > 0)
                    {
                        Nodes[row, col].Edges.Add(new Grid8WayEdge<T>(Nodes[row, col - 1], Direction.Left));

                        if (row > 0)
                        {
                            Nodes[row, col].Edges.Add(new Grid8WayEdge<T>(Nodes[row - 1, col - 1], Direction.UpLeft));
                        }
                        if (row < Rows - 1)
                        {
                            Nodes[row, col].Edges.Add(new Grid8WayEdge<T>(Nodes[row + 1, col - 1], Direction.DownLeft));
                        }
                    }

                    // Rights
                    if (col < Cols - 1)
                    {
                        Nodes[row, col].Edges.Add(new Grid8WayEdge<T>(Nodes[row, col + 1], Direction.Right));

                        if (row > 0)
                        {
                            Nodes[row, col].Edges.Add(new Grid8WayEdge<T>(Nodes[row - 1, col + 1], Direction.UpRight));
                        }
                        if (row < Rows - 1)
                        {
                            Nodes[row, col].Edges.Add(new Grid8WayEdge<T>(Nodes[row + 1, col + 1], Direction.DownRight));
                        }
                    }
                }
            }

        }

        public void ResetVisited()
        {
            for(int r=0 ; r<Rows ; r++)
                for(int c=0; c<Cols ; c++)
                    Nodes[r,c].Visited = false;
        }
        public List<Grid8WayNode<T>> GetAdjacentNodes(int row, int col) //, T? filter = null)
        {
            var output = new List<Grid8WayNode<T>>();
            if(row < 0 || row > Rows )
                throw new ArgumentOutOfRangeException(nameof(row));

            if( col < 0 || col > Cols) 
                throw new ArgumentOutOfRangeException(nameof(col));

            var node = Nodes[row,col];
            foreach(var adjacentNode in node.Edges)
            {
                output.Add(adjacentNode.Node);
            }

            return output;
        }
    }
}