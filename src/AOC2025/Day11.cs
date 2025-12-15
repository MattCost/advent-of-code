public class Day11 : BaseDay
{
    List<string> _lines = new();

    Dictionary<string, Node> Nodes = new();
    public Day11()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            _lines.Add(line);
            var node = new Node { Id = line.Split(':')[0]};
            Nodes.Add(node.Id, node);
        }
        Nodes["out"] = new Node {Id = "out"};

        foreach(var l in _lines)
        {
            var parts = l.Split(':');
            var nodeId = parts[0];
            var nextNodes = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach(var n in nextNodes)
            {
                Nodes[nodeId].NextNodes.Add( Nodes[n]);
            }
        }
    }

    public override ValueTask<string> Solve_1()
    {
        long output = 0;
        output = CountPathsFrom(Nodes["you"], Nodes["out"]);
        return new($"{output}");
    }

    private long CountPathsFrom(Node node1, Node node2)
    {
        long output = 0;
        if(node1.NextNodes.Contains(node2))
        {
            output++;
        }
        foreach(var node in node1.NextNodes)
        {
            output += CountPathsFrom(node, node2);
        }

        return output;
    }

    public override ValueTask<string> Solve_2()
    {
        long output = 0;

        return new($"{output}");
    }

    private class Node
    {
        public string Id { get; set; }
        public List<Node> NextNodes { get; set; } = new();
    }

}