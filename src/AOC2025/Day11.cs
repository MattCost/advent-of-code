using System.Security.Cryptography.X509Certificates;

public class Day11 : BaseDay
{
    List<string> _lines = new();

    Graph graph = new();
    Dictionary<string, Node> Nodes => graph.Nodes;
    public Day11()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            _lines.Add(line);
            var node = new Node { Id = line.Split(':')[0] };
            Nodes.Add(node.Id, node);
        }
        Nodes["out"] = new Node { Id = "out" };

        foreach (var l in _lines)
        {
            var parts = l.Split(':');
            var nodeId = parts[0];
            var nextNodes = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var n in nextNodes)
            {
                Nodes[nodeId].NextNodes.Add(Nodes[n]);
            }
        }
    }

    public override ValueTask<string> Solve_1()
    {
        long output = CountPathsFrom(Nodes["you"], Nodes["out"]);
        return new($"{output}");
    }

    private long CountPathsFrom(Node node1, Node node2)
    {
        long output = 0;
        if (node1.NextNodes.Contains(node2))
        {
            output++;
        }
        
        foreach (var node in node1.NextNodes)
        {
            output += CountPathsFrom(node, node2);
        }

        return output;
    }

    public override ValueTask<string> Solve_2()
    {
        long output = 0;
        graph.EnsureNoCycles();

        Console.WriteLine("Searching for svr to fft");
        var svrToFFT = graph.FindAllPathsCount("svr", "fft");
        Console.WriteLine($"\t {svrToFFT} svr to fft");

        Console.WriteLine("Searching for fft to dac");
        var fftToDAC = graph.FindAllPathsCount("fft", "dac");
        Console.WriteLine($"\t {fftToDAC} fft to dac");
        
        Console.WriteLine("Searching for dac to out");
        var dacToOUT = graph.FindAllPathsCount("dac", "out");
        Console.WriteLine($"\t {dacToOUT} dac to out");

       
        Console.WriteLine("Searching for svr to dac");
        var svrToDAC = graph.FindAllPathsCount("svr", "dac");
        Console.WriteLine($"\t {svrToDAC} svr to dac");

        Console.WriteLine("Searching for dac to fft");
        var dacToFFT = graph.FindAllPathsCount("dac", "fft");
        Console.WriteLine($"\t {dacToFFT} dac to fft");
        
        Console.WriteLine("Searching for fft to out");
        var fftToOUT = graph.FindAllPathsCount("fft", "out");
        Console.WriteLine($"\t {fftToOUT} fft to out");
        
        output +=  (long)svrToFFT * (long)fftToDAC * (long)dacToOUT;
        output +=  (long)svrToDAC * (long)dacToFFT * (long)fftToOUT; 
        return new($"{output}");
    }

    private class Node
    {
        public string Id { get; set; } = string.Empty;
        public List<Node> NextNodes { get; set; } = new();
    }

    private class Graph
    {
        public Dictionary<string, Node> Nodes = new();
        public void EnsureNoCycles()
        {
            Dictionary<string, int> color = new();
            Dictionary<string, string> parent = new();
            string cycle_start = string.Empty;
            string cycle_end = string.Empty;
            foreach (var node in Nodes.Values)
            {
                color[node.Id] = 0;
                parent[node.Id] = string.Empty;
            }

            foreach (var nodeId in Nodes.Keys)
            {
                if (color[nodeId] == 0)
                {
                    var subResult = FindCycleHelper(nodeId, color, parent);
                    if (subResult.Item1)
                    {
                        //Item 2 and 3 will be start and end
                        cycle_start = subResult.Item2;
                        cycle_end = subResult.Item3;
                        break;
                    }

                }
            }
            if (string.IsNullOrEmpty(cycle_start))
            {
                Console.WriteLine("Acyclic");
            }
            else
            {
                List<string> cyclePath = new();
                cyclePath.Add(cycle_start);
                var cycleNode = cycle_end;
                while (cycle_end != cycle_start && !string.IsNullOrEmpty(cycleNode))
                {
                    cyclePath.Add(cycleNode);
                    cycleNode = parent[cycleNode];
                }
                cyclePath.Reverse();

                Console.WriteLine("Cycle Found");
                Console.WriteLine(string.Join(" -> ", cyclePath));
                throw new Exception("Cycle Found!");
            }
        }
        private (bool, string, string) FindCycleHelper(string nodeId, Dictionary<string, int> color, Dictionary<string, string> parent)
        {
            color[nodeId] = 1;
            foreach (var adjNode in Nodes[nodeId].NextNodes)
            {
                if (color[adjNode.Id] == 0)
                {
                    parent[adjNode.Id] = nodeId;
                    var subResult = FindCycleHelper(adjNode.Id, color, parent);
                    if (subResult.Item1)
                    {
                        return subResult;
                    }
                }
                else if (color[adjNode.Id] == 1)
                {
                    return (true, adjNode.Id, nodeId);
                }
            }

            color[nodeId] = 2;
            return (false, string.Empty, string.Empty);
        }

        public Dictionary<string, List<string>> FindAllParents(string nodeId)
        {
            var parents = new Dictionary<string, List<string>>();
            foreach (var n in Nodes.Keys)
            {
                parents[n] = new();
            }
            var queue = new Queue<string>();
            queue.Enqueue(nodeId);
            while (queue.Count > 0)
            {
                var currentNodeId = queue.Dequeue();
                foreach (var adjNode in Nodes[currentNodeId].NextNodes)
                {
                    if (!parents[adjNode.Id].Contains(currentNodeId))
                    {
                        parents[adjNode.Id].Add(currentNodeId);
                        queue.Enqueue(adjNode.Id);
                    }
                }
            }

            return parents;
        }

        public List<List<string>> FindAllPaths(Dictionary<string, List<string>> parents, string startId, string endId)
        {
            var output = new List<List<string>>();
            if (startId == endId)
            {
                output.Add(new List<string> { startId });
            }
            else
            {
                foreach (var x in parents[endId])
                {
                    foreach (var y in FindAllPaths(parents, startId, x))
                    {
                        y.Add(endId);
                        output.Add(y);
                    }
                }
            }
            return output;
        }

        private Dictionary<string, int> _pathCountCache = new();
        public int FindAllPathsCount(string startId, string endId, Dictionary<string, List<string>>? parents = null)
        {
            if(parents == null)
            {
                parents = FindAllParents(startId);
                // _pathCountCache = new();
            }
            
            if(_pathCountCache.ContainsKey($"{startId}-{endId}"))
                return _pathCountCache[$"{startId}-{endId}"];

            var output = 0;
            if (startId == endId)
            {
                output++;
            }
            else
            {
                foreach (var x in parents[endId])
                {
                    output += FindAllPathsCount(startId, x, parents);
                }
            }
            _pathCountCache[$"{startId}-{endId}"] = output;
            return output;

        }
    }
}