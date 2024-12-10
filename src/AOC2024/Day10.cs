using AdventOfCode.Base.Grid;

public class Day10 : BaseDay
{
    List<string> _lines = new();
    int _rows;
    int _cols;
    Grid<int> _map;
    List<GridNode<int>> _trailHeads = new();
    public Day10()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            _lines.Add(line);
        }
        _rows = _lines.Count();
        _cols = _lines[0].Length;
        _map = new Grid<int>(_rows, _cols);
        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _cols; c++)
            {
                var v = _lines[r][c] == '.' ? -1 : int.Parse($"{_lines[r][c]}");
                _map.AddNode(v, r, c);
                if (_map.Nodes[r, c].Value == 0)
                {
                    _trailHeads.Add(_map.Nodes[r, c]);
                }
            }
        }
        _map.PopulateEdges();

    }

    public override ValueTask<string> Solve_1()
    {
        int output = 0;
        foreach (var trailHead in _trailHeads)
        {
            output += SummitsReachable(trailHead);
        }
        return new($"{output}");
    }

    private int SummitsReachable(GridNode<int> trailHead)
    {
        if (trailHead.Value != 0) return 0;

        List<GridNode<int>> summits = new();
        Queue<GridNode<int>> nodesToCheck = new();
        nodesToCheck.Enqueue(trailHead);
        while (nodesToCheck.Count > 0)
        {
            var current = nodesToCheck.Dequeue();
            if(current.Value == 9)
            {
                summits.Add(current);
            }
            else
            {
                foreach(var nextStep in current.Edges)
                {
                    if(nextStep.Node.Value == current.Value + 1)
                    {
                        nodesToCheck.Enqueue(nextStep.Node);
                    }
                }
            }
        }

        return summits.Distinct().Count();
    }

    public override ValueTask<string> Solve_2()
    {
                int output = 0;
        foreach (var trailHead in _trailHeads)
        {
            output += UniqueTrails(trailHead);
        }
        return new($"{output}");
    }

    private int UniqueTrails(GridNode<int> trailHead)
    {
        if (trailHead.Value != 0) return 0;

        int output = 0;
        Queue<GridNode<int>> nodesToCheck = new();
        nodesToCheck.Enqueue(trailHead);
        while (nodesToCheck.Count > 0)
        {
            var current = nodesToCheck.Dequeue();
            if(current.Value == 9)
            {
                output++;
            }
            else
            {
                foreach(var nextStep in current.Edges)
                {
                    if(nextStep.Node.Value == current.Value + 1)
                    {
                        nodesToCheck.Enqueue(nextStep.Node);
                    }
                }
            }
        }
        
        return output;
    }
}