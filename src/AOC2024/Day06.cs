using AdventOfCode.Base.Grid8Way;

public class Day06 : BaseDay
{
    List<string> _lines = new();
    int _rows;
    int _cols;
    int _startRow = -1;
    int _startCol = -1;
    Direction _startDirection;
    public Day06()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            _lines.Add(line);
        }

        _rows = _lines.Count();
        _cols = _lines[0].Length;
    }
    private Grid8Way<char> GenerateGrid()
    {
        var grid = new Grid8Way<char>(_rows, _cols);
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                var current = _lines[row][col];
                grid.AddNode(current, row, col);
                if (current != '.' && current != '#')
                {
                    _startRow = row;
                    _startCol = col;
                    if (current == '^') _startDirection = Direction.Up;
                    if (current == '<') _startDirection = Direction.Left;
                    if (current == '>') _startDirection = Direction.Right;
                    if (current == 'v') _startDirection = Direction.Down;
                }
            }
        }
        return grid;
    }

    public override ValueTask<string> Solve_1()
    {
        var _grid = GenerateGrid();
        var output = 0;
        TraverseGridChar(_grid, _startRow, _startCol, _startDirection);
        foreach (var node in _grid.Nodes)
        {
            if (node.Visited) output++;
        }
        // _grid.PrintGrid();
        return new(output.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var baseGrid = GenerateGrid();
        // baseGrid.PrintGrid();
        TraverseGridChar(baseGrid, _startRow, _startCol, _startDirection);
        List<(int row, int col)> addBlocks = new();
        foreach (var node in baseGrid.Nodes)
        {
            // BRUTE any blank spot can become block
            if (node.Value == '.')
            {
                addBlocks.Add((node.Row, node.Col));
            }

            // Slightly Less brute optimize the number of addition blocks by only placing a block in the way of the guard
            // if (node.Visited && !(node.Row == _startRow && node.Col == _startCol))
            // {
            //     addBlocks.Add((node.Row, node.Col));
            // }
        }

        int output = 0;

        Console.WriteLine($"We have {addBlocks.Count()} positions to check");
        // Parallel.ForEach(addBlocks, newBlock => //doesn't seem to be any faster than a normal foreach. Did the runtime get smart enough to parallelize things?
        addBlocks.ForEach( newBlock =>  // Don't run in parallel if printing things out
        {
            var grid = GenerateGrid();
            grid.Nodes[newBlock.row, newBlock.col] = new Grid8WayNode<char>('#', newBlock.row, newBlock.col);
            if (TraverseGridChar(grid, _startRow, _startCol, _startDirection))
            {
                Interlocked.Increment(ref output);
                // Console.WriteLine($"Adding a block to {newBlock.row},{newBlock.col} trapped the guard");
                // grid.PrintGrid(_startRow, _startCol);
            }
        });

        return new(output.ToString());
    }
    public bool TraverseGridChar(Grid8Way<char> _grid, int startRow, int startCol, Direction startDirection)
    {
        int row = startRow, col = startCol;
        Direction direction = startDirection;

        var node = _grid.Nodes[row, col];
        node.Visited = true;
        node.PastVisitDirections.Add(direction);

        do
        {
            // If we are on the edge, and moving outwards, not in a loop
            if (row == 0 && direction == Direction.Up) return false;
            if (row == _rows - 1 && direction == Direction.Down) return false;
            if (col == 0 && direction == Direction.Left) return false;
            if (col == _cols - 1 && direction == Direction.Right) return false;

            switch (direction)
            {
                case Direction.Up:
                    if (row > 0 && _grid.Nodes[row - 1, col].Value == '#')
                    {
                        direction = Direction.Right;
                        node.PastVisitDirections.Add(direction);
                    }
                    break;
                case Direction.Down:
                    if (row < _rows - 1 && _grid.Nodes[row + 1, col].Value == '#')
                    {
                        direction = Direction.Left;
                        node.PastVisitDirections.Add(direction);
                    }
                    break;
                case Direction.Left:
                    if (col > 0 && _grid.Nodes[row, col - 1].Value == '#')
                    {
                        direction = Direction.Up;
                        node.PastVisitDirections.Add(direction);
                    }
                    break;
                case Direction.Right:
                    if (col < _cols - 1 && _grid.Nodes[row, col + 1].Value == '#')
                    {
                        direction = Direction.Down;
                        node.PastVisitDirections.Add(direction);
                    }
                    break;
            }

            switch (direction)
            {
                case Direction.Up:
                    row--;
                    break;
                case Direction.Down:
                    row++;
                    break;
                case Direction.Left:
                    col--;
                    break;
                case Direction.Right:
                    col++;
                    break;
            }

            node = _grid.Nodes[row, col];

            // If we have already been here, going the same direction, we are in a loop (right?)
            if (node.PastVisitDirections.Contains(direction))
            {
                return true;
            }

            node.Visited = true;
            node.PastVisitDirections.Add(direction);

        }
        while (InBounds(row, col));

        return false;
    }

    private bool InBounds(int row, int col)
    {
        if (row < 0) return false;
        if (row > _rows - 1) return false;

        if (col < 0) return false;
        if (col > _cols - 1) return false;

        return true;
    }
}
