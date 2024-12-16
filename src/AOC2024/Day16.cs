using AdventOfCode.Base.Grid;

public class Day16 : BaseDay
{
    List<string> _lines = new();

    Grid<char> _maze;
    GridNode<char> _start;
    GridNode<char> _end;

    public Day16()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line = sr.ReadLine();
        while (!string.IsNullOrEmpty(line))
        {
            _lines.Add(line);
            line = sr.ReadLine();
        }
        _maze = new Grid<char>(_lines.Count, _lines[0].Length);

        for (int row = 0; row < _lines.Count; row++)
        {
            line = _lines[row];
            for (int col = 0; col < _lines[0].Length; col++)
            {
                var nodeValue = line[col];

                if (nodeValue == 'S')
                {
                    _maze.AddNode('.', row, col);
                    _start = _maze.Nodes[row, col];
                    
                }
                else if (nodeValue == 'E')
                {
                    _maze.AddNode('.', row, col);
                    _end = _maze.Nodes[row, col];
                }
                else
                {
                    _maze.AddNode(nodeValue, row, col);
                }
            }
        }
        if (_start == null || _end == null) throw new Exception("bad input");

        _maze.PopulateEdges(false);
        _maze.PrintGrid();
    }

    public override ValueTask<string> Solve_1()
    {

        PriorityQueue<(int row, int col, Direction dir), int> _processingQueue = new();

        bool[,] visited = new bool[_maze.Rows, _maze.Cols];

        int[,] distances = new int[_maze.Rows, _maze.Cols];
        for (int r = 0; r < _maze.Rows; r++) for (int c = 0; c < _maze.Cols; c++) distances[r, c] = int.MaxValue;
        
        distances[_start.Row, _start.Col] = 0;
        
        // Start off going east
        _processingQueue.Enqueue((row: _start.Row, col: _start.Col, dir: Direction.Right), 0);

        while (_processingQueue.Count > 0)
        {
            var currentInfo = _processingQueue.Dequeue();

            // if target node break?
            if (_maze.Nodes[currentInfo.row, currentInfo.col].Value == 'E') break;

            if (visited[currentInfo.row, currentInfo.col])
                continue;

            visited[currentInfo.row, currentInfo.col] = true;

            var currentNode = _maze.Nodes[currentInfo.row, currentInfo.col];
            var currentDir = currentInfo.dir;

            foreach (var edge in currentNode.Edges)
            {
                // skip visited nodes
                if (visited[edge.Node.Row, edge.Node.Col]) continue;

                // figure out cost to neighbor
                int newDistance;
                if (edge.Direction == currentDir)
                {
                    newDistance = distances[currentNode.Row, currentNode.Col] + 1;
                }
                else if (edge.Direction == currentDir.Reverse())
                {
                    newDistance = distances[currentNode.Row, currentNode.Col] + 1000 + 1000 + 1;
                }
                else
                {
                    newDistance = distances[currentNode.Row, currentNode.Col] + 1000 + 1;
                }

                // update distance table if its cheaper  and enqueue the new entry
                if (newDistance < distances[edge.Node.Row, edge.Node.Col])
                {
                    distances[edge.Node.Row, edge.Node.Col] = newDistance;
                    _processingQueue.Enqueue((edge.Node.Row, edge.Node.Col, edge.Direction), newDistance);
                }
            }
        }
        var distance = distances[_end.Row, _end.Col];

        // debug for samples
        // for(int r = 0 ; r< _maze.Rows ; r++)
        // {
        //     for(int c=0 ; c<_maze.Cols ; c++)
        //     {
        //         var d = distances[r,c];
        //         if(d < int.MaxValue)
        //         {
        //             Console.Write(d.ToString().PadLeft(5));
        //         }
        //         else
        //         {
        //             Console.Write($"     ");
        //         }
        //     }
        //     Console.WriteLine('\n');
        // }


        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {distance}");
    }

    public override ValueTask<string> Solve_2()
    {
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2");
    }

}