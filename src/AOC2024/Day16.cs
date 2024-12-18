using AdventOfCode.Base.Grid;

public class Day16 : BaseDay
{
    List<string> _lines = new();
    Grid<char> _maze;
    GridNode<char> _start;
    GridNode<char> _end;
    int[,] distances;
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
        // _maze.PrintGrid();
        distances = new int[_maze.Rows, _maze.Cols];
    }

    public override ValueTask<string> Solve_1()
    {
        PriorityQueue<(int row, int col, Direction dir), int> _processingQueue = new();

        bool[,] visited = new bool[_maze.Rows, _maze.Cols];

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

        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {distance}");
    }

    // Used for debug
    void PrintGrid(Grid<int> grid)
    {
        for (int r = 0; r < grid.Rows; r++)
        {
            for (int c = 0; c < grid.Cols; c++)
            {
                var d = grid.Nodes[r, c].Value;
                if (d < int.MaxValue)
                {
                    Console.Write(d.ToString().PadLeft(10));
                }
                else
                {
                    Console.Write($"          ");
                }
            }
            Console.WriteLine('\n');
        }
    }

    public override ValueTask<string> Solve_2()
    {

        PriorityQueue<(int row, int col, Direction dir, List<(int row, int col)> path, int distance), int> _processingQueue = new();
        bool[,] visited = new bool[_maze.Rows, _maze.Cols];
        bool[,] validVisited = new bool[_maze.Rows, _maze.Cols];

        //Save the maxDistance from part 1
        var maxDistance = distances[_end.Row, _end.Col];
        // Then reset for the next walk
        for (int r = 0; r < _maze.Rows; r++) for (int c = 0; c < _maze.Cols; c++)
                distances[r, c] = int.MaxValue;
        distances[_start.Row, _start.Col] = 0;

        // Start off going east
        _processingQueue.Enqueue((row: _start.Row, col: _start.Col, dir: Direction.Right, path: [], distance: 0), 0);

        while (_processingQueue.Count > 0)
        {
            var copy = new Grid<char>(_maze.Rows, _maze.Cols);
            for (int r = 0; r < _maze.Rows; r++)
            {
                for (int c = 0; c < _maze.Cols; c++)
                {
                    copy.AddNode(_maze.Nodes[r, c].Value, r, c);
                }
            }
            var x = _processingQueue.UnorderedItems;
            foreach (var y in x)
            {
                copy.AddNode('@', y.Element.row, y.Element.col);
            }
            // Console.Clear();
            // copy.PrintGrid();
            // Console.ReadKey();
            // Task.Delay(25).Wait();

            var currentInfo = _processingQueue.Dequeue();

            // if target node break
            if (currentInfo.row == _end.Row && currentInfo.col == _end.Col)
            {
                if (currentInfo.distance <= maxDistance)
                {
                    //add path to validVisited
                    foreach (var node in currentInfo.path)
                    {
                        validVisited[node.row, node.col] = true;
                    }
                    validVisited[currentInfo.row, currentInfo.col] = true;
                }
                else
                {
                    Console.WriteLine("whomp");
                }
                continue;
            }

            if (currentInfo.distance > maxDistance) continue;
            visited[currentInfo.row, currentInfo.col] = true;

            var currentNode = _maze.Nodes[currentInfo.row, currentInfo.col];
            var currentDir = currentInfo.dir;

            foreach (var edge in currentNode.Edges.Where(edge => edge.Direction != currentDir.Reverse()))
            {
                // skip visited nodes
                if (currentInfo.path.Contains((edge.Node.Row, edge.Node.Col))) continue;

                // figure out cost to neighbor
                int newDistance = currentInfo.distance + (edge.Direction == currentDir ? 1 : 1001);

                // optimal path
                if (newDistance < distances[edge.Node.Row, edge.Node.Col])
                {
                    distances[edge.Node.Row, edge.Node.Col] = newDistance;
                    var newPath = currentInfo.path.ToList();
                    newPath.Add((currentNode.Row, currentNode.Col));
                    _processingQueue.Enqueue((row: edge.Node.Row, col: edge.Node.Col, dir: edge.Direction, path: newPath, distance: newDistance), newDistance);
                }
                else
                {
                    if (newDistance == distances[edge.Node.Row, edge.Node.Col] || newDistance == distances[edge.Node.Row, edge.Node.Col] + 1000)
                    {
                        var newPath = currentInfo.path.ToList();
                        newPath.Add((currentNode.Row, currentNode.Col));
                        _processingQueue.Enqueue((row: edge.Node.Row, col: edge.Node.Col, dir: edge.Direction, path: newPath, distance: newDistance), newDistance);
                    }
                    else
                    {
                        Console.Write("x");
                    }
                }
            }
        }

        for (int r = 0; r < _maze.Rows; r++)
        {
            for (int c = 0; c < _maze.Cols; c++)
            {

                if (validVisited[r, c])
                {
                    Console.Write('1');
                }
                else
                {
                    Console.Write($" ");
                }
            }
            Console.Write('\n');
        }

        int validNodes = 0;
        foreach (var boo in validVisited)
        {
            if (boo) validNodes++;

        }

        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {validNodes}");



        // This was close, then I manually reviewed the output, and confused myself and guess the correct answer
        // var maxDistance = distances[_end.Row, _end.Col];

        // var grid2 = new Grid<int>(_maze.Rows, _maze.Cols);
        // for (int r = 0; r < _maze.Rows; r++)
        // {
        //     for (int c = 0; c < _maze.Cols; c++)
        //     {
        //         grid2.AddNode(distances[r, c] <= maxDistance ? distances[r, c] : -1, r, c);
        //     }
        // }

        // // PrintGrid(grid2);
        // grid2.PopulateEdges();
        // bool[,] visited = new bool[_maze.Rows, _maze.Cols];
        // Queue<GridNode<int>> nodesToProcess = new();
        // nodesToProcess.Enqueue(grid2.Nodes[_end.Row, _end.Col]);
        // while (nodesToProcess.TryDequeue(out var currentNode))
        // {
        //     // var currentNode = nodesToProcess.Dequeue();
        //     visited[currentNode.Row, currentNode.Col] = true;

        //     var possibleNext = currentNode.Edges.Where(edge => edge.Node.Value != -1);
        //     foreach (var next in possibleNext)
        //     {
        //         if ((next.Node.Value == currentNode.Value - 1) || (next.Node.Value == currentNode.Value - 1001) || (next.Node.Value == currentNode.Value + 999))
        //         {
        //             nodesToProcess.Enqueue(next.Node);
        //         }
        //     }
        // }

        // // for (int r = 0; r < grid2.Rows; r++)
        // // {
        // //     for (int c = 0; c < grid2.Cols; c++)
        // //     {
        // //         var d = grid2.Nodes[r, c].Value;
        // //         if (visited[r, c])
        // //         {
        // //             Console.Write(d.ToString().PadLeft(10));
        // //         }
        // //         else
        // //         {
        // //             Console.Write($"          ");
        // //         }
        // //     }
        // //     Console.Write('\n');
        // // }

        // int visitedCount = 0;
        // foreach (var visit in visited) if (visit) visitedCount++;
        // return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {visitedCount}");

    }


}