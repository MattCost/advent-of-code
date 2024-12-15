using AdventOfCode.Base.Grid;

public class Day15 : BaseDay
{
    Grid<char> _grid;
    List<GridNode<char>> _boxes = new();
    GridNode<char> _robot;
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
        for (int row = 0; row < _lines.Count; row++)
        {
            line = _lines[row];
            for (int col = 0; col < _lines[0].Length; col++)
            {
                var nodeValue = line[col];
                _grid.AddNode(nodeValue, row, col);

                if (nodeValue == 'O')
                    _boxes.Add(_grid.Nodes[row, col]);

                if (nodeValue == '@')
                    _robot = _grid.Nodes[row, col];
            }
        }
        if (_robot == null) throw new Exception("404 robot not found");
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
        var colwDelta = direction switch
        {
            Direction.Left => -1,
            Direction.Right => 1,
            _ => 0
        };
        var row = _robot.Row + rowDelta;
        var col = _robot.Col + colwDelta;
        while (_grid.PointInBounds(row, col))
        {
            var node = _grid.Nodes[row, col];
            switch (node.Value)
            {
                case 'O':
                    nodes.Add(node);
                    row += rowDelta;
                    col += colwDelta;
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
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2");
    }

}