using AdventOfCode.Base.Grid;

public class Day18 : BaseDay
{
    List<string> _lines = new();
    int _gridSize = 71;

    public Day18()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line = sr.ReadLine();
        while (!string.IsNullOrEmpty(line) )
        {
            _lines.Add(line);
            line = sr.ReadLine();
        }

    }

    public override ValueTask<string> Solve_1()
    {
        var maze = LoadMaze(1024);

        PriorityQueue<(int row, int col, Direction dir), int> _processingQueue = new();

        bool[,] visited = new bool[maze.Rows, maze.Cols];
        int[,] distances = new int[maze.Rows, maze.Cols];

        for (int r = 0; r < maze.Rows; r++) for (int c = 0; c < maze.Cols; c++) distances[r, c] = int.MaxValue;

        distances[0, 0] = 0;

        // Start off going east
        _processingQueue.Enqueue((row: 0, col: 0, dir: Direction.Right), 0);

        while (_processingQueue.Count > 0)
        {
            (int currentRow, int currentCol, Direction currentDir) = _processingQueue.Dequeue();

            // if target node break
            if (currentRow == _gridSize - 1 && currentCol == _gridSize - 1) break;

            if (visited[currentRow, currentCol])
                continue;

            visited[currentRow, currentCol] = true;

            var currentNode = maze.Nodes[currentRow, currentCol];

            foreach (var edge in currentNode.Edges)
            {
                // skip visited nodes
                if (visited[edge.Node.Row, edge.Node.Col]) continue;

                // figure out cost to neighbor
                int newDistance = distances[currentNode.Row, currentNode.Col] + 1;

                // update distance table if its cheaper  and enqueue the new entry
                if (newDistance < distances[edge.Node.Row, edge.Node.Col])
                {
                    distances[edge.Node.Row, edge.Node.Col] = newDistance;
                    _processingQueue.Enqueue((edge.Node.Row, edge.Node.Col, edge.Direction), newDistance);
                }
            }
        }
        var distance = distances[_gridSize - 1, _gridSize - 1];

        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {distance}");
    }

    public override ValueTask<string> Solve_2()
    {
        int startingBytes = 1024;
        int byteDelta = 64;
        int currentBytes = startingBytes;
        bool prevExitReachable = true;
        do
        {
            var maze = LoadMaze(currentBytes);
            var exitReachable = RunMaze(maze);
            if(!exitReachable && prevExitReachable && byteDelta == 1)
                break;

            if( (exitReachable && !prevExitReachable) || (!exitReachable && prevExitReachable))
                byteDelta = -(byteDelta>>1);
           
            currentBytes += byteDelta;
            prevExitReachable = exitReachable;
        }
        while (byteDelta != 0);
        if(byteDelta == 0) Console.WriteLine("exit is never blocked ;)");
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2 {_lines[currentBytes]}");
    }

    private bool RunMaze(Grid<char> maze)
    {
        PriorityQueue<(int row, int col), int> _processingQueue = new();

        bool[,] visited = new bool[maze.Rows, maze.Cols];
        int[,] distances = new int[maze.Rows, maze.Cols];

        for (int r = 0; r < maze.Rows; r++) for (int c = 0; c < maze.Cols; c++) distances[r, c] = int.MaxValue;

        distances[0, 0] = 0;

        _processingQueue.Enqueue((row: 0, col: 0), 0);

        while (_processingQueue.Count > 0)
        {
            (int currentRow, int currentCol) = _processingQueue.Dequeue();

            // if target node break
            if (currentRow == _gridSize - 1 && currentCol == _gridSize - 1) break;

            if (visited[currentRow, currentCol])
                continue;

            visited[currentRow, currentCol] = true;

            var currentNode = maze.Nodes[currentRow, currentCol];

            foreach (var edge in currentNode.Edges)
            {
                // skip visited nodes
                if (visited[edge.Node.Row, edge.Node.Col]) continue;

                // figure out cost to neighbor
                int newDistance = distances[currentNode.Row, currentNode.Col] + 1;

                // update distance table if its cheaper  and enqueue the new entry
                if (newDistance < distances[edge.Node.Row, edge.Node.Col])
                {
                    distances[edge.Node.Row, edge.Node.Col] = newDistance;
                    _processingQueue.Enqueue((edge.Node.Row, edge.Node.Col), newDistance);
                }
            }
        }
        var distance = distances[_gridSize - 1, _gridSize - 1];
        return distance != int.MaxValue;
    }

    private Grid<char> LoadMaze(int loadByteCount)
    {
        var output = new Grid<char>(_gridSize, _gridSize);

        for (int row = 0; row < _gridSize; row++)
        {
            for (int col = 0; col < _gridSize; col++)
            {
                output.AddNode('.', row, col);
            }
        }

        int byteCount = 0;
        while (byteCount++ < loadByteCount)
        {
            var line = _lines[byteCount];
            var c = int.Parse(line.Split(',')[0]);
            var r = int.Parse(line.Split(',')[1].Trim());
            output.AddNode('#', r, c);
        }

        output.PopulateEdges(false);

        return output;
    }


}