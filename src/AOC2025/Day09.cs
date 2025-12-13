public class Day09 : BaseDay
{
    List<string> _lines = new();
    List<(int X, int Y)> points = new();

    public Day09()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            _lines.Add(line);
            var parts = line.Split(',');
            points.Add(new(int.Parse(parts[0]), int.Parse(parts[1])));
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
        var nodes = new List<Node>();
        var edges = new List<Edge>();
        var graph = new Graph();

        var firstNode = new Node { X = points[0].X, Y = points[0].Y };
        nodes.Add(firstNode);
        graph.Nodes.Add(firstNode);

        var prevNode = firstNode;
        for (int i = 1; i < points.Count; i++)
        {
            var node = new Node { X = points[i].X, Y = points[i].Y };
            nodes.Add(node);
            graph.Nodes.Add(node);


            var edge = new Edge { P1 = prevNode, P2 = node };
            edges.Add(edge);
            graph.Edges.Add(edge);

            prevNode = node;
        }

        edges.Add(new Edge { P1 = prevNode, P2 = firstNode });
        graph.Edges.Add(new Edge { P1 = prevNode, P2 = firstNode });

        ////////////////////////
        /// Edge crossing logic :)
        /// 
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                var p1 = points[i];
                var p2 = points[j];

                var minX = Math.Min(p1.X, p2.X);
                var maxX = Math.Min(p1.X, p2.X);
                var minY = Math.Min(p1.Y, p2.Y);
                var maxY = Math.Min(p1.Y, p2.Y);

                var newArea = (1 + Math.Abs(points[i].X - points[j].X)) * (1 + Math.Abs(points[i].Y - points[j].Y));
                if (newArea < output) continue;

                //Horizontal Lines
                //p1.X, p1.Y to p2.X, p1.Y
                //p1.X, p2.Y to p2.X, p2.Y
                // must _cross_ 0 vertical edges
                var xCrossing = edges
                    .Where(edge => !edge.IsHorizontal)
                    .Where(edge => edge.P1.X >= minX && edge.P1.X <= maxX)
                    .Where(edge => edge.MaxY > minY)
                    .Where(edge => edge.MinY < maxY);
                    // .Where(edge =>
                    //     (edge.MinY <= minY && edge.MaxY > minY) ||

                    //     (edge.MinY > minY && edge.MaxY < maxY) ||

                    //     (edge.MinY < maxY && edge.MaxY >= maxY)
                    // );
                if (xCrossing.Any()) continue;

                //Vertical Lines
                //p1.X, p1.Y to p1.X, p2.Y
                //p2.X, p1.Y to p2.X, p2.Y
                var yCrossing = edges
                    .Where(edge => edge.IsHorizontal)
                    .Where(edge => edge.P1.Y >= minY && edge.P1.Y < maxY)
                    .Where(edge => edge.MaxX > minX)
                    .Where(edge => edge.MinX < maxY);
                //     .Where(edge =>
                //     (edge.MinX <= minX && edge.MaxX > minX) ||

                //     (edge.MinX > minX && edge.MaxX < maxX) ||

                //     (edge.MinX < maxX && edge.MaxX >= maxX)

                // );

                if (yCrossing.Any()) continue;


                var centerX = (minX+maxX)/2;
                var centerY = (minY+maxY)/2;
                if(!graph.Contains(centerX, centerY)) continue;
                output = newArea;

            }
        }

        return new($"{output}");
    }

    private class Node
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    private class Edge
    {
        public Node P1 { get; set; } = new();
        public Node P2 { get; set; } = new();

        public bool IsHorizontal => P1.Y == P2.Y;

        public int MinX => Math.Min(P1.X, P2.X);
        public int MaxX => Math.Max(P1.X, P2.X);
        public int MinY => Math.Min(P1.Y, P2.Y);
        public int MaxY => Math.Max(P1.Y, P2.Y);
        public bool Contains(int x, int y)
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
        public bool Contains(int x, int y)
        {
            // Early return edge cases first. If we land on a node, or edge, return true
            if (Nodes.Any(node => node.X == x && node.Y == y)) return true;

            if (Edges.Any(edge => edge.Contains(x, y))) return true;

            // Otherwise do a ray search

            // Get all vertical edges, to the right of the X coord, that overlap our point.
            var verticalEdges = Edges
                .Where(edge => !edge.IsHorizontal)
                .Where(edge => edge.P1.X > x)
                .Where(edge => edge.MinY <= y && edge.MaxY > y);

            // odd means inside
            // even means outside
            if(verticalEdges.Count() % 2 == 1)
                return true;

            return false;
        }

    }
}