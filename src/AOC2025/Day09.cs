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

        // Bitmap logic, works
        var maxX = points.Select(p => p.X).Max() + 3;
        var maxY = points.Select(p => p.Y).Max() + 2;
        var bitmap = new byte[maxX][];

        for (int x = 0; x < maxX; x++)
        {
            bitmap[x] = new byte[maxY];
        }

        foreach (var edge in graph.Edges)
        {
            if (edge.IsHorizontal)
            {
                var y = edge.P1.Y;
                for (int x = Math.Min(edge.P1.X, edge.P2.X); x <= Math.Max(edge.P1.X, edge.P2.X); x++)
                {
                    bitmap[x][y] = 1;
                }
            }
            else
            {
                var x = edge.P1.X;
                for (int y = Math.Min(edge.P1.Y, edge.P2.Y); y <= Math.Max(edge.P1.Y, edge.P2.Y); y++)
                {
                    bitmap[x][y] = 1;
                }
            }
        }

        FloodFill(bitmap, 0, 0, 0, 2, maxX, maxY);

        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                var p1 = points[i];
                var p2 = points[j];

                if(bitmap[p1.X][p1.Y] == 2) continue;
                if(bitmap[p1.X][p2.Y] == 2) continue;
                if(bitmap[p2.X][p1.Y] == 2) continue;
                if(bitmap[p2.X][p2.Y] == 2) continue;

                var newArea = (1 + Math.Abs(points[i].X - points[j].X)) * (1 + Math.Abs(points[i].Y - points[j].Y));
                if(newArea < output) continue;


                // line 1 p1.x, p1.y to p2.y
                // line 2 p2.x, p1.y to p2.y
                bool moveOn = false;
                for (var y = Math.Min(p1.Y, p2.Y); y <= Math.Max(p1.Y, p2.Y); y++)
                {
                    if (bitmap[p1.X][y]==2 || bitmap[p2.X][y]==2)
                    {
                        moveOn = true;
                        break;
                    }
                }
                if (moveOn) continue;

                // line 3 p1.x to p2.x, p1.y
                // line 4 p1.x to p2.x, p2.y
                for (var x = Math.Min(p1.X, p2.X); x <= Math.Max(p1.X, p2.X); x++)
                {
                    if (bitmap[x][p1.Y]==2 || bitmap[x][p2.Y] == 2)
                    {
                        moveOn = true;
                        break;
                    }
                }
                if (moveOn) continue;

                if (newArea > output)
                {
                    Console.WriteLine($"P1 {p1} P2 {p2} are fully contained in graph. Area {newArea}");
                    output = newArea;
                }
            }
        }

        return new($"{output}");
    }

    private void FloodFill(byte[][] bitmap, int x, int y, byte old,  byte target, int maxX, int maxY)
    {
        if (x < 0) return;
        if (x >= maxX) return;
        if (y < 0) return;
        if (y >= maxY) return;
        if(bitmap[x][y] != old) return;

        var queue = new Queue<(int x, int y)>();
        queue.Enqueue((x,y));
        while(queue.Count > 0)
        {
            var (xx, yy) = queue.Dequeue();
            if( xx< 0  || xx>= maxX) continue;
            if( yy<0 || yy>=maxY) continue;
            if(bitmap[xx][yy] != old) continue;
            if(bitmap[xx][yy] == target) continue;
            bitmap[xx][yy] = target;
            queue.Enqueue( (xx-1, yy));
            queue.Enqueue( (xx+1, yy));
            queue.Enqueue( (xx, yy-1));
            queue.Enqueue( (xx, yy+1));
        }
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

        public Dictionary<int, Dictionary<int, bool>> _cache = new();

        // public bool Contains(int x, int y)
        // {

        //     if(_cache.ContainsKey(x) && _cache[x].ContainsKey(y))
        //         return _cache[x][y];
            
        //     bool output = false;

        //     // Early return edge cases first. If we land on a node, or edge, return true
        //     if (Nodes.Where(node => node.X == x && node.Y == y).Any()) 
        //     {
        //         output = true;
        //     }
        //     else if (Edges.Where(edge => edge.Contains(x, y)).Any())
        //     {
        //         output = true;
        //     } 
        //     else
        //     {
        //         // Otherwise do a ray search
        //         /*
        //             Get all vertical edges to the right of the X coord
        //             Do the magic counting algo
        //         */

        //         // Get all horizontal edges in with the same Y, to the right of the X coord
        //         // var horizontalEdges = Edges
        //         //     .Where(edge => edge.IsHorizontal)
        //         //     .Where(edge => edge.P1.Y == y)
        //         //     .Where(edge => Math.Min(edge.P1.X, edge.P2.X) > x);

        //         // Get all vertical edges, to the right of the X coord, that overlap our point.
        //         var verticalEdges = Edges
        //             .Where(edge => !edge.IsHorizontal)
        //             .Where(edge => edge.P1.X > x)
        //             .Where(edge => Math.Min(edge.P1.Y, edge.P2.Y) <= y && y <= Math.Max(edge.P1.Y, edge.P2.Y));

        //         // odd means inside
        //         // even means outside
        //         if (verticalEdges.Count() % 2 == 1)
        //         {
        //             output = true;
        //         }
        //     }

        //     //cache
        //     if(!_cache.ContainsKey(x))
        //     {
        //         _cache[x] = new();
        //     }
        //     _cache[x][y] = output;

        //     //re
        //     return output;
        // }
    }
}