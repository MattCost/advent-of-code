using AdventOfCode.Base.Grid;

public class Day12 : BaseDay
{
    List<string> _lines = new();
    int _rows;
    int _cols;
    Grid<char> _map;
    Dictionary<char, List<(long area, long perimeter)>> _plotInfo = new();

    public Day12()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            _lines.Add(line);
        }
        _rows = _lines.Count();
        _cols = _lines[0].Length;
        _map = new Grid<char>(_rows, _cols);
        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _cols; c++)
            {
                var plot = _lines[r][c];
                _map.AddNode(plot, r, c);
                if (!_plotInfo.ContainsKey(plot)) _plotInfo[plot] = new();
            }
        }

        _map.PopulateEdges(false);


    }

    public override ValueTask<string> Solve_1()
    {
        foreach (var plotType in _plotInfo.Keys.ToList())
        {
            Console.WriteLine($"Working on plotType {plotType}");
            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _cols; c++)
                {
                    // for each node, if type is target and not visited
                    var node = _map.Nodes[r, c];
                    if (node.Value == plotType && !node.Visited)
                    {
                        Console.WriteLine($"Found a plot of type {plotType} at [{r},{c}] not yet visited.");
                        // find extents of plot
                        var result = FindPlotExtents(node);
                        Console.WriteLine($"This plot is {result.area} large");
                        _plotInfo[node.Value].Add(result);
                    }

                }
            }
        }
        long output = _plotInfo.SelectMany(plotKVP => plotKVP.Value.Select(plot => plot.area * plot.perimeter)).Sum();
        return new($"{output}");
    }

    private (long area, long perimeter) FindPlotExtents(GridNode<char> node)
    {
        List<GridNode<char>> nodesInPlot = new();
        Queue<GridNode<char>> nodesToProcess = new();
        nodesToProcess.Enqueue(node);
        while (nodesToProcess.Count > 0)
        {
            var currentNode = nodesToProcess.Dequeue();
            if(!currentNode.Visited)
            {
                currentNode.Visited = true;
                nodesInPlot.Add(currentNode);
                foreach (var edge in currentNode.Edges)
                {
                    nodesToProcess.Enqueue(edge.Node);
                }

            }
        }

        return new(nodesInPlot.Count, 1);
    }

    public override ValueTask<string> Solve_2()
    {
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2");
    }

}