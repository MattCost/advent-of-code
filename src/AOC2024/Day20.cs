using System.Collections.Concurrent;
using System.Text;
using AdventOfCode.Base.Grid;

public class Day20 : BaseDay
{
    List<string> _lines = new();
    int _startRow = 0, _startCol = 0;
    int _endRow = 0, _endCol = 0;
    int targetSaving = 10;

    public Day20()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            _lines.Add(line);
        }
    }

    private Grid<char> LoadMaze()
    {
        var output = new Grid<char>(_lines.Count, _lines.Count);

        for (int row = 0; row < output.Rows; row++)
        {
            for (int col = 0; col < output.Cols; col++)
            {
                var nodeValue = _lines[row][col];
                if (nodeValue == 'S')
                {
                    _startRow = row;
                    _startCol = col;
                    nodeValue = '.';
                }
                if (nodeValue == 'E')
                {
                    _endRow = row;
                    _endCol = col;
                    nodeValue = '.';
                }
                output.AddNode(nodeValue, row, col);
            }
        }

        output.PopulateEdges(true);

        return output;
    }

    int[,] defaultDistances;

    private List<(int row, int col)> RunMaze(Grid<char> maze, int cheatRow = -1, int cheatCol = -1)
    {
        bool cheatActive = (cheatRow > -1) && (cheatCol > -1);

        if (cheatActive)
        {
            maze.AddNode('.', cheatRow, cheatCol);
            maze.PopulateEdges();
        }

        PriorityQueue<(int row, int col, List<(int row, int col)> path), int> _processingQueue = new();

        bool[,] visited = new bool[maze.Rows, maze.Cols];
        int[,] distances = new int[maze.Rows, maze.Cols];

        for (int r = 0; r < maze.Rows; r++) for (int c = 0; c < maze.Cols; c++) distances[r, c] = int.MaxValue;

        distances[_startRow, _startCol] = 0;

        _processingQueue.Enqueue((row: _startRow, col: _startCol, path: []), 0);

        while (_processingQueue.Count > 0)
        {
            (int currentRow, int currentCol, List<(int row, int col)> currentPath) = _processingQueue.Dequeue();

            // if target node break
            if (currentRow == _endRow && currentCol == _endCol)
            {
                if (!cheatActive)
                    defaultDistances = distances; //save for later

                currentPath.Add((currentRow, currentCol));
                return currentPath;
            }

            if (visited[currentRow, currentCol])
                continue;

            visited[currentRow, currentCol] = true;

            var currentNode = maze.Nodes[currentRow, currentCol];

            bool cheatPassed = cheatActive && currentPath.Contains((cheatRow, cheatCol));

            foreach (var edge in currentNode.Edges.Where(edge => edge.Node.Value == '.'))
            {
                // skip visited nodes
                if (visited[edge.Node.Row, edge.Node.Col]) continue;

                // figure out cost to neighbor
                int newDistance = distances[currentNode.Row, currentNode.Col] + 1;

                if (cheatPassed)
                {
                    var savings = defaultDistances[currentNode.Row, currentNode.Col] - newDistance;
                    if (savings < 100)
                        continue; //we didn't save enough
                }
                // update distance table if its cheaper  and enqueue the new entry
                else if (newDistance < distances[edge.Node.Row, edge.Node.Col])
                {
                    distances[edge.Node.Row, edge.Node.Col] = newDistance;
                    var newPath = currentPath.ToList();
                    newPath.Add((currentRow, currentCol));
                    _processingQueue.Enqueue((edge.Node.Row, edge.Node.Col, newPath), newDistance);
                }
            }
        }
        return new();
    }


    private bool RunMazeWithCheat(Grid<char> maze, int cheatRow, int cheatCol, int targetSavings = 100)
    {
        maze.AddNode('.', cheatRow, cheatCol);
        maze.PopulateEdges();
        PriorityQueue<(int row, int col, List<(int row, int col)> path), int> _processingQueue = new();

        bool[,] visited = new bool[maze.Rows, maze.Cols];
        int[,] distances = new int[maze.Rows, maze.Cols];

        for (int r = 0; r < maze.Rows; r++) for (int c = 0; c < maze.Cols; c++) distances[r, c] = int.MaxValue;

        distances[_startRow, _startCol] = 0;

        _processingQueue.Enqueue((row: _startRow, col: _startCol, path: []), 0);

        while (_processingQueue.Count > 0)
        {
            (int currentRow, int currentCol, List<(int row, int col)> currentPath) = _processingQueue.Dequeue();

            // if target node break
            if (currentRow == _endRow && currentCol == _endCol)
            {
                currentPath.Add((currentRow, currentCol));
                var savings = defaultDistances[currentRow, currentCol] - distances[currentRow, currentCol];
                return savings >= targetSavings;
            }

            if (visited[currentRow, currentCol])
                continue;

            visited[currentRow, currentCol] = true;

            bool cheatPassed = currentPath.Contains((cheatRow, cheatCol));

            if (cheatPassed)
            {
                var savings = defaultDistances[currentRow, currentCol] - distances[currentRow, currentCol];
                return savings >= targetSavings;
            }

            var currentNode = maze.Nodes[currentRow, currentCol];

            foreach (var edge in currentNode.Edges.Where(edge => edge.Node.Value == '.'))
            {
                // skip visited nodes
                if (visited[edge.Node.Row, edge.Node.Col]) continue;

                // figure out cost to neighbor
                int newDistance = distances[currentNode.Row, currentNode.Col] + 1;

                // update distance table if its cheaper  and enqueue the new entry
                if (newDistance < distances[edge.Node.Row, edge.Node.Col])
                {
                    distances[edge.Node.Row, edge.Node.Col] = newDistance;
                    var newPath = currentPath.ToList();
                    newPath.Add((currentRow, currentCol));
                    _processingQueue.Enqueue((edge.Node.Row, edge.Node.Col, newPath), newDistance);
                }
            }
        }
        return new();
    }



    private void DrawMap(Grid<char> maze, List<(int row, int col)> defaultPath, int cheatRow = -1, int cheatCol = -1, List<(int row, int col)>? pathTaken = null)
    {
        bool cheatActive = (cheatRow > -1) && (cheatCol > -1) && pathTaken != null;
        List<List<char>> output = new();

        for (int r = 0; r < maze.Rows; r++)
        {
            output.Add(new());
            for (int c = 0; c < maze.Cols; c++)
            {
                output[r].Add(maze.Nodes[r, c].Value);
            }
        }

        foreach (var step in defaultPath)
        {
            output[step.row][step.col] = 'o';
        }

        output[_startRow][_startCol] = 'S';
        output[_endRow][_endCol] = 'E';

        if (cheatActive)
        {
            output[cheatRow][cheatCol] = 'X';
            foreach (var step in pathTaken!)
            {
                output[step.row][step.col] = 'O';
            }

        }

        foreach (var line in output)
        {
            foreach (var step in line)
            {
                switch (step)
                {
                    case '#':
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write('#');
                        break;
                    case '.':
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write('.');
                        break;

                    case 'X':
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write('X');
                        break;

                    case 'o':
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write('o');
                        break;

                    case 'O':
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write('O');
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write(step);
                        break;
                }

            }
            Console.Write('\n');
        }

    }

    public override ValueTask<string> Solve_1()
    {
        var initialMaze = LoadMaze();
        var defaultPath = RunMaze(initialMaze);
        var defaultScore = defaultPath.Count - 1;
        Console.WriteLine($"The default path with no cheats takes {defaultScore} picoseconds");
        // DrawMap(initialMaze, defaultPath);

        int shorterPaths = 0;
        foreach (var point in defaultPath)
        {
            var node = initialMaze.Nodes[point.row, point.col];
            foreach (var edge in node.Edges)
            {
                // skip all border nodes
                if (edge.Node.Row == 0) continue;
                if (edge.Node.Col == 0) continue;
                if (edge.Node.Row == initialMaze.Rows - 1) continue;
                if (edge.Node.Col == initialMaze.Cols - 1) continue;
                if (edge.Node.Value == '#')
                {
                    var quantumLeap = edge.Node.Edges.Where(secondEdge => secondEdge.Direction == edge.Direction && secondEdge.Node.Value == '.');
                    if (quantumLeap.Any())
                    {
                        var secondNode = quantumLeap.First();
                        var savings = defaultDistances[secondNode.Node.Row, secondNode.Node.Col] - (defaultDistances[node.Row, node.Col] + 2);
                        if (savings >= targetSaving)
                            shorterPaths++;
                    }
                }
            }
        }
        Console.WriteLine($"There are {shorterPaths} that will save at least {targetSaving} picoseconds");

        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {shorterPaths}");
    }

    public override ValueTask<string> Solve_2()
    {
        int maxCheat = 20;
        int targetSaving = 100;
        int shorterPathCount = 0;
        Dictionary<(int r1, int c1, int r2, int c2), int> shorterPaths = new();
        
        var initialMaze = LoadMaze();
        var defaultPath = RunMaze(initialMaze);
        var defaultScore = defaultPath.Count - 1;
        Console.WriteLine($"The default path with no cheats takes {defaultScore} picoseconds");

        foreach (var (row, col) in defaultPath)
        {
            PriorityQueue<GridNode<char>, int> nodesToCheck = new();

            var shortcutStartNode = initialMaze.Nodes[row, col];

            bool[,] visited = new bool[initialMaze.Rows, initialMaze.Cols];
            int[,] distances = new int[initialMaze.Rows, initialMaze.Cols];
            for (int r = 0; r < initialMaze.Rows; r++) for (int c = 0; c < initialMaze.Cols; c++) distances[r, c] = int.MaxValue;

            foreach (var edge in shortcutStartNode.Edges)
            {
                nodesToCheck.Enqueue(edge.Node, 1);
                distances[edge.Node.Row, edge.Node.Col] = 1;
            }

            while (nodesToCheck.TryDequeue(out var nodeToCheck, out var cheatCount))
            {
                if (cheatCount >= maxCheat) // or >= ?
                    continue;

                visited[nodeToCheck.Row, nodeToCheck.Col] = true;

                foreach (var edge in nodeToCheck.Edges)
                {
                    // A shortcut can't go backwards
                    if (edge.Node.Row == row && edge.Node.Col == col) 
                        continue;

                    // skip visited nodes
                    if (visited[edge.Node.Row, edge.Node.Col])
                        continue;
                        
                    // any node that is a . could be the end of a shortcut
                    if (edge.Node.Value == '.')
                    {
                        var shortcut = (row, col, edge.Node.Row, edge.Node.Col);
                        if (!shorterPaths.ContainsKey(shortcut))
                        {
                            shorterPaths[shortcut] = cheatCount + 1;
                        }
                        else if (cheatCount + 1 < shorterPaths[shortcut])
                        {
                            shorterPaths[shortcut] = cheatCount + 1;
                        }
                    }
                    
                    // add the next wave of # to the queue
                    int newCheat = cheatCount + 1;
                    if (newCheat >= maxCheat) continue;
                    if (newCheat < distances[edge.Node.Row, edge.Node.Col])
                    {
                        distances[edge.Node.Row, edge.Node.Col] = newCheat;
                        nodesToCheck.Enqueue(edge.Node, newCheat);
                    }
                }
            }
        }

        foreach (var shortcut in shorterPaths.Keys)
        {
            var startRow = shortcut.r1;
            var startCol = shortcut.c1;
            var endRow = shortcut.r2;
            var endCol = shortcut.c2;

            var savings = defaultDistances[endRow, endCol] - (defaultDistances[startRow, startCol] + shorterPaths[shortcut]);
            if (savings >= targetSaving)
            {
                shorterPathCount++;
            }
        }

        Console.WriteLine($"There are {shorterPathCount} that will save at least {targetSaving} picoseconds");

        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2 {shorterPathCount}");
    }

}