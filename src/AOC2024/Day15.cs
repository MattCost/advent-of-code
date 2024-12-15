using AdventOfCode.Base.Grid;

public class Day15 : BaseDay
{
    Grid<char> _grid;
    Grid<char> _wideGrid;
    List<GridNode<char>> _boxes = new();
    List<GridNode<char>> _wideBoxes = new();
    GridNode<char> _robot;
    GridNode<char> _wideRobot;
    List<string> _lines = new();
    List<Direction> _moves = new();
    public Day15()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line = sr.ReadLine();
        while (!string.IsNullOrEmpty(line))
        {
            _lines.Add(line);
            line = sr.ReadLine();
        }
        _grid = new Grid<char>(_lines.Count, _lines[0].Length);
        _wideGrid = new Grid<char>(_lines.Count, _lines[0].Length * 2);

        for (int row = 0; row < _lines.Count; row++)
        {
            line = _lines[row];
            for (int col = 0; col < _lines[0].Length; col++)
            {
                var nodeValue = line[col];

                // Normal Grid
                _grid.AddNode(nodeValue, row, col);
                if (nodeValue == 'O')
                    _boxes.Add(_grid.Nodes[row, col]);
                if (nodeValue == '@')
                    _robot = _grid.Nodes[row, col];

                // Wide Grid
                switch (nodeValue)
                {
                    case '#':
                    case '.':
                        _wideGrid.AddNode(nodeValue, row, col * 2);
                        _wideGrid.AddNode(nodeValue, row, col * 2 + 1);
                        break;
                    case 'O':
                        _wideGrid.AddNode('[', row, col * 2);
                        _wideGrid.AddNode(']', row, col * 2 + 1);
                        _wideBoxes.Add(_wideGrid.Nodes[row, col * 2]);
                        _wideBoxes.Add(_wideGrid.Nodes[row, col * 2 + 1]);
                        break;
                    case '@':
                        _wideGrid.AddNode('@', row, col * 2);
                        _wideGrid.AddNode('.', row, col * 2+1);
                        _wideRobot = _wideGrid.Nodes[row, col * 2];

                        break;
                }
            }
        }

        if (_robot == null || _wideRobot == null) throw new Exception("404 robot not found");

        _grid.PopulateEdges();


        while ((line = sr.ReadLine()) != null)
        {
            for (int i = 0; i < line.Length; i++)
            {
                _moves.Add(line[i] switch
                {
                    '<' => Direction.Left,
                    '>' => Direction.Right,
                    '^' => Direction.Up,
                    'v' => Direction.Down,
                    _ => throw new Exception("Lost?")
                });
            }
        }
    }

    public override ValueTask<string> Solve_1()
    {
        foreach (var move in _moves)
        {
            //If robot can do move
            if (CanMove(move, out var nodes))
            {
                MoveBoxes(move, nodes);
                _grid.AddNode('.', _robot.Row, _robot.Col);
                var rowDelta = move switch
                {
                    Direction.Up => -1,
                    Direction.Down => 1,
                    _ => 0
                };
                var colDelta = move switch
                {
                    Direction.Left => -1,
                    Direction.Right => 1,
                    _ => 0
                };
                _grid.AddNode('@', _robot.Row + rowDelta, _robot.Col + colDelta);
                _robot = _grid.Nodes[_robot.Row + rowDelta, _robot.Col + colDelta];
            }
        }

        long output = 0;
        for (int row = 0; row < _grid.Rows; row++)
        {
            for (int col = 0; col < _grid.Cols; col++)
            {
                if (_grid.Nodes[row, col].Value == 'O')
                {
                    output += 100 * row + col;
                }
            }
        }

        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {output}");
    }

    bool CanMove(Direction direction, out List<GridNode<char>> nodes)
    {
        nodes = new();
        var rowDelta = direction switch
        {
            Direction.Up => -1,
            Direction.Down => 1,
            _ => 0
        };
        var colDelta = direction switch
        {
            Direction.Left => -1,
            Direction.Right => 1,
            _ => 0
        };
        var row = _robot.Row + rowDelta;
        var col = _robot.Col + colDelta;
        while (_grid.PointInBounds(row, col))
        {
            var node = _grid.Nodes[row, col];
            switch (node.Value)
            {
                case 'O':
                    nodes.Add(node);
                    row += rowDelta;
                    col += colDelta;
                    break;
                case '.':
                    return true;
                case '#':
                    return false;
            }
        }
        return false;
    }

    void MoveBoxes(Direction direction, List<GridNode<char>> nodes)
    {
        var rowDelta = direction switch
        {
            Direction.Up => -1,
            Direction.Down => 1,
            _ => 0
        };
        var colDelta = direction switch
        {
            Direction.Left => -1,
            Direction.Right => 1,
            _ => 0
        };

        if (nodes.Where(node => node.Value != 'O').Any())
        {
            throw new Exception("Cant move non-boxes");
        }

        foreach (var node in nodes)
        {
            _grid.AddNode('.', node.Row, node.Col);
        }

        foreach (var node in nodes)
        {
            _grid.AddNode('O', node.Row + rowDelta, node.Col + colDelta);
        }
    }

    public override ValueTask<string> Solve_2()
    {
        foreach (var move in _moves)
        {
            //If robot can do move
            if (CanMove2(move, out var nodes))
            {
                MoveBoxes2(move, nodes);
                _wideGrid.AddNode('.', _wideRobot.Row, _wideRobot.Col);
                var rowDelta = move switch
                {
                    Direction.Up => -1,
                    Direction.Down => 1,
                    _ => 0
                };
                var colDelta = move switch
                {
                    Direction.Left => -1,
                    Direction.Right => 1,
                    _ => 0
                };
                _wideGrid.AddNode('@', _wideRobot.Row + rowDelta, _wideRobot.Col + colDelta);
                _wideRobot = _wideGrid.Nodes[_wideRobot.Row + rowDelta, _wideRobot.Col + colDelta];
            }
        }

        long output = 0;
        for (int row = 0; row < _wideGrid.Rows; row++)
        {
            for (int col = 0; col < _wideGrid.Cols; col++)
            {
                if (_wideGrid.Nodes[row, col].Value == '[')
                {
                    output += 100 * row + col;
                }
            }
        }

        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {output}");
    }
    bool CanMove2(Direction direction, out List<GridNode<char>> nodes)
    {
        nodes = new();
        var rowDelta = direction switch
        {
            Direction.Up => -1,
            Direction.Down => 1,
            _ => 0
        };
        var colDelta = direction switch
        {
            Direction.Left => -1,
            Direction.Right => 1,
            _ => 0
        };
        var row = _wideRobot.Row + rowDelta;
        var col = _wideRobot.Col + colDelta;

        if (rowDelta == 0)
        {
            while (_wideGrid.PointInBounds(row, col))
            {
                var node = _wideGrid.Nodes[row, col];
                switch (node.Value)
                {
                    case '[':
                    case ']':
                        nodes.Add(node);
                        row += rowDelta;
                        col += colDelta;
                        break;
                    case '.':
                        return true;
                    case '#':
                        return false;
                }
            }
            return false;
        }
        else
        {
            List<int> searchCols = [col];

            while (_wideGrid.PointInBounds(row, col))
            {
                // If any of the searchCols contain a wall, return false
                if (searchCols.Where(searchCol => _wideGrid.Nodes[row, searchCol].Value == '#').Any())
                    return false;

                // If all of the searchCols contain empty space, return true
                if (searchCols.Where(searchCol => _wideGrid.Nodes[row, searchCol].Value == '.').Count() == searchCols.Count)
                    return true;

                // Otherwise, if there are boxes in searchCols, the next row's searchCols will fan in/out based on box position
                var nextSearchCols = new List<int>();
                foreach (var searchCol in searchCols)
                {
                    if (_wideGrid.Nodes[row, searchCol].Value == '[')
                    {
                        nextSearchCols.Add(searchCol);
                        nextSearchCols.Add(searchCol + 1);
                    }

                    if (_wideGrid.Nodes[row, searchCol].Value == ']')
                    {
                        nextSearchCols.Add(searchCol);
                        nextSearchCols.Add(searchCol - 1);
                    }
                }
                searchCols = nextSearchCols.Distinct().ToList();
                foreach (var searchCol in searchCols)
                {
                    nodes.Add(_wideGrid.Nodes[row, searchCol]);
                }
                row += rowDelta;
                col += colDelta;
            }
            return false;
        }
    }

    void MoveBoxes2(Direction direction, List<GridNode<char>> nodes)
    {
        var rowDelta = direction switch
        {
            Direction.Up => -1,
            Direction.Down => 1,
            _ => 0
        };
        var colDelta = direction switch
        {
            Direction.Left => -1,
            Direction.Right => 1,
            _ => 0
        };

        if (nodes.Any(node => node.Value == '.' || node.Value == '#' || node.Value == '@'))
        {
            throw new Exception("Cant move non-boxes");
        }
        List<(char value, int row, int col)> oldNodes = new();
        foreach (var node in nodes)
        {
            oldNodes.Add(new (node.Value, node.Row, node.Col));
            _wideGrid.AddNode('.', node.Row, node.Col);
        }

        foreach (var node in oldNodes)
        {
            _wideGrid.AddNode(node.value, node.row + rowDelta, node.col + colDelta);
            // _wideGrid.AddNode('O', node.Row + rowDelta, node.Col + colDelta);
        }
    }


}