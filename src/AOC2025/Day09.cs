public class Day09 : BaseDay
{
    List<string> _lines = new();
    List<(long X, long Y)> points = new();
    public Day09()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            _lines.Add(line);
            var parts = line.Split(',');
            points.Add(new(long.Parse(parts[0]), long.Parse(parts[1])));
        }

    }

    public override ValueTask<string> Solve_1()
    {
        long output = 0;
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                var newArea = (1 + Math.Abs(points[i].X - points[j].X)) * (1 + Math.Abs(points[i].Y - points[j].Y));
                if (newArea > output)
                {
                    output = newArea;
                }
            }
        }
        return new($"{output}");
    }
    public override ValueTask<string> Solve_2()
    {
        long output = 0;
        var graph = new Graph();
        var firstNode = new Node { X = points[0].X, Y = points[0].Y };
        graph.Nodes.Add(firstNode);

        var prevNode = firstNode;
        for (int i = 1; i < points.Count; i++)
        {
            var node = new Node { X = points[i].X, Y = points[i].Y };
            graph.Nodes.Add(node);

            var edge = new Edge { P1 = prevNode, P2 = node };
            graph.Edges.Add(edge);

            prevNode = node;
        }

        graph.Edges.Add(new Edge { P1 = prevNode, P2 = firstNode });

        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                // Process 2 points
                var p1 = points[i];
                var p2 = points[j];

                // if(p1.X == 11 && p1.Y==7 && p2.X==2 && p2.Y==5)
                //     Console.WriteLine("whoops");

                if( graph.Contains(p1.X, p1.Y) && graph.Contains(p2.X, p2.Y) && graph.Contains(p1.X, p2.Y) && graph.Contains(p2.X, p1.Y))
                {
                    // TODO all points along all 4 edges must be fully contained in the graph
                        
                    // Console.WriteLine($"P1 {p1} P2 {p2} are fully contained in graph");

                    //Area calc like as in part 1
                    var newArea = (1 + Math.Abs(points[i].X - points[j].X)) * (1 + Math.Abs(points[i].Y - points[j].Y));
                    if (newArea > output)
                    {
                        output = newArea;
                    }
                }
                else
                {
                    // Console.WriteLine($"P1 {p1} P2 {p2} NOT fully contained in graph");                    
                }


            }
        }

        return new($"{output}");
    }

    private class Node
    {
        public long X { get; set; }
        public long Y { get; set; }
    }
    private class Edge
    {
        public Node P1 { get; set; } = new();
        public Node P2 { get; set; } = new();

        public bool IsHorizontal => P1.Y == P2.Y;
        public bool Contains(long x, long y)
        {
            if (IsHorizontal)
            {
                if (y != P1.Y)
                    return false;

                if ((x >= Math.Min(P1.X, P2.X)) && (x <= Math.Max(P1.X, P2.X)))
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (x != P1.X)
                    return false;

                if ((y >= Math.Min(P1.Y, P2.Y)) && (y <= Math.Max(P1.Y, P2.Y)))
                {
                    return true;
                }
                return false;

            }

        }
    }

    private class Graph
    {
        public List<Node> Nodes { get; set; } = new();
        public List<Edge> Edges { get; set; } = new();

        public bool Contains(long x, long y)
        {
            // Early return edge cases first. If we land on a node, or edge, return true
            if (Nodes.Where(node => node.X == x && node.Y == y).Any()) return true;

            if (Edges.Where(edge => edge.Contains(x, y)).Any()) return true;

            // Otherwise do a ray search
            /*
                Get all vertical edges to the right of the X coord
                Do the magic counting algo
            */

            // Get all horizontal edges in with the same Y, to the right of the X coord
            var horizontalEdges = Edges
                .Where(edge => edge.IsHorizontal)
                .Where(edge => edge.P1.Y == y)
                .Where(edge => Math.Min(edge.P1.X, edge.P2.X) > x);
            
            // Get all vertical edges, to the right of the X coord, that overlap our point.
            var verticalEdges = Edges
                .Where(edge => !edge.IsHorizontal)
                .Where(edge => edge.P1.X > x)
                .Where(edge => Math.Min(edge.P1.Y, edge.P2.Y) <= y && y<=Math.Max(edge.P1.Y, edge.P2.Y));

            // odd means inside
            // even means outside
            if(verticalEdges.Count() % 2 == 1)
                return true;

            return false;
        }
    }

}
// 4728331461 too high
// Probably need to do my extra credit