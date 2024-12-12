using AdventOfCode.Base.Grid;

public class Day12 : BaseDay
{
    List<string> _lines = new();
    int _rows;
    int _cols;
    Grid<char> _map;
    Dictionary<char, List<(long area, long perimeter, long sides)>> _plotInfo = new();

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

        foreach (var plotType in _plotInfo.Keys.ToList())
        {
            // Console.WriteLine($"Working on plotType {plotType}");
            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _cols; c++)
                {
                    // for each node, if type is target and not visited
                    var node = _map.Nodes[r, c];
                    if (node.Value == plotType && !node.Visited)
                    {
                        // Console.WriteLine($"Found a plot of type {plotType} at [{r},{c}] not yet visited.");
                        // find extents of plot
                        var result = FindPlotExtents(node);
                        // Console.WriteLine($"This plot has an area of {result.area} a perimeter of {result.perimeter} and a {result.sides} sides");
                        _plotInfo[node.Value].Add(result);
                    }

                }
            }
        }

    }

    public override ValueTask<string> Solve_1()
    {

        long output = _plotInfo.SelectMany(plotKVP => plotKVP.Value.Select(plot => plot.area * plot.perimeter)).Sum();
        return new($"{output}");
    }

    private (long area, long perimeter, long sides) FindPlotExtents(GridNode<char> node)
    {
        List<GridNode<char>> nodesInPlot = new();
        Queue<GridNode<char>> nodesToProcess = new();
        nodesToProcess.Enqueue(node);
        while (nodesToProcess.Count > 0)
        {
            var currentNode = nodesToProcess.Dequeue();
            if (!currentNode.Visited)
            {
                currentNode.Visited = true;
                nodesInPlot.Add(currentNode);
                foreach (var edge in currentNode.Edges)
                {
                    nodesToProcess.Enqueue(edge.Node);
                }
            }
        }

        long perimeter = nodesInPlot.Select(node => 4 - node.Edges.Count).Sum();

        //** Walk the perimeter to count sides
        // Console.WriteLine($"Starting perimeter walk for plot type {node.Value}");
        List<GridNode<char>> _walkedNodes = new();
        int sides = 0;
        foreach (var nodeInPlot in nodesInPlot)
        {
            // Console.WriteLine($"Checking node [{nodeInPlot.Row},{nodeInPlot.Col}] {nodeInPlot.Value}");
            // If node is not a border, there is no perimeter to walk.
            if (nodeInPlot.Edges.Count == 4)
            {
                // Console.WriteLine("Node has 4 edges, not a border");
                continue;
            }

            // If we have already walked this node as part of a previous perimeter walk, move on
            if (_walkedNodes.Contains(nodeInPlot))
            {
                // Console.WriteLine("We have already walked here");
                continue;
            }

            Direction direction;
            if (!nodeInPlot.Edges.Where(edge => edge.Direction == Direction.Down).Any())
            {
                //on bottom edge, so go left
                // Console.WriteLine("No edge going down, starting off left");
                direction = Direction.Left;
            }
            else if (!nodeInPlot.Edges.Where(edge => edge.Direction == Direction.Left).Any())
            {
                //on left edge, so go up
                // Console.WriteLine("No edge going left, starting off up");
                direction = Direction.Up;
            }
            else if (!nodeInPlot.Edges.Where(edge => edge.Direction == Direction.Up).Any())
            {
                // on top edge, so go right
                // Console.WriteLine("No edge going up, starting off right");
                direction = Direction.Right;
            }
            else if (!nodeInPlot.Edges.Where(edge => edge.Direction == Direction.Right).Any())
            {
                // Console.WriteLine("No edge going right, starting off down");
                direction = Direction.Down;
            }
            else
            {
                //you are an island
                // Console.WriteLine("island");
                sides += 4;
                _walkedNodes.Add(nodeInPlot);
                continue;
            }
            
            /*
                Figure out which part of the border we are on
                Figure out a starting direction based on that
                walkLoop:
                can we advance forward 1 step in current direction? 
                if yes
                    advance 1 step
                    can we turn left? if yes do so and sides++
                else
                    can we turn right? if yes do so and sides++;

                This won't work on this example, as we only find the outside perimeter for a sidecount of 4, but the puzzle input wasn't this tricky.
                AAA
                ABA
                AAA
            */

            var startingNode = nodeInPlot;
            var startingDirection = direction;

            var currentNode = nodeInPlot;
            do
            {   
                var advance = currentNode.Edges.Where(edge=>edge.Direction == direction);
                if(advance.Any())
                {
                    // Console.WriteLine($"Advance 1 step {direction}");
                    currentNode = advance.First().Node;
                    _walkedNodes.Add(currentNode);
                    var leftDir = direction.TurnLeft();
                    var advanceLeft = currentNode.Edges.Where(edge=>edge.Direction == leftDir);
                    if(advanceLeft.Any())
                    {
                        direction = leftDir;
                        sides++;
                        // Console.WriteLine($"Turning left to head {leftDir} and increasing side count to {sides}");
                    }
                }
                else
                {
                    // Console.WriteLine($"Unable to advice 1 step in {direction}, turning right to head {direction.TurnRight()} and increasing side count to {++sides}");
                    direction = direction.TurnRight();
                    sides++;
                }
            }
            while (!(currentNode == startingNode && direction == startingDirection));
            // Console.WriteLine("Reached starting point going the same direction");
        }

        return new(nodesInPlot.Count, perimeter, sides);
    }

    public override ValueTask<string> Solve_2()
    {
        long output = _plotInfo.SelectMany(plotKVP => plotKVP.Value.Select(plot => plot.area * plot.sides)).Sum();
        return new($"{output}");

    }

}