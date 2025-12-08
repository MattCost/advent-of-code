public class Day08 : BaseDay
{
    List<string> _lines = new();

    private class JunctionBox
    {
        public string Id { get; } = string.Empty;
        public int X { get; }
        public int Y { get; }
        public int Z { get; }
        public JunctionBox(string input)
        {
            Id = input;
            X = int.Parse(Id.Split(",")[0]);
            Y = int.Parse(Id.Split(",")[1]);
            Z = int.Parse(Id.Split(",")[2]);
        }

        public double DistanceTo(JunctionBox other)
        {
            return Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2) + Math.Pow(Z - other.Z, 2));
        }

        public override string ToString()
        {
            return Id;
        }
    }

    private class JunctionBoxNetwork
    {
        public List<JunctionBox> Nodes { get; set; } = new();
        public void AddJunctionBox(JunctionBox node)
        {
            Nodes.Add(node);
        }
        public bool ContainsJunctionBox(JunctionBox node)
        {
            return Nodes.Contains(node);
        }
        public void AddNetwork(JunctionBoxNetwork network)
        {
            Nodes.AddRange(network.Nodes);
        }
    }
    List<JunctionBox> junctionBoxes = new();
    public Day08()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            _lines.Add(line);
            junctionBoxes.Add(new JunctionBox(line));
        }

    }

    public override ValueTask<string> Solve_1()
    {
        long output = 0;

        List<(double Distance, JunctionBox Node1, JunctionBox Node2)> distanceTracker = new();
        for (int i = 0; i < junctionBoxes.Count - 1; i++)
        {
            for (int j = i + 1; j < junctionBoxes.Count; j++)
            {
                distanceTracker.Add(new(junctionBoxes[i].DistanceTo(junctionBoxes[j]), junctionBoxes[i], junctionBoxes[j]));
            }
        }
        Console.WriteLine("Distance Map Calculated");
        distanceTracker = distanceTracker.OrderBy(x => x.Distance).ToList();

        List<JunctionBoxNetwork> networkTracker = new();

        int linkCounter = 0;
        int distanceIndex = 0;
        while (linkCounter < 1000)
        {
            // Get closest 2 boxes;
            var node1 = distanceTracker[distanceIndex].Node1;
            var node2 = distanceTracker[distanceIndex].Node2;
            distanceIndex++;

            // Search for networks
            var node1Network = networkTracker.Where(network => network.ContainsJunctionBox(node1));
            var node2Network = networkTracker.Where(network => network.ContainsJunctionBox(node2));

            // The puzzle description is confusing "nothing happens" implies you don't need to make a link, but it seems to mean nothing happens to the circuit, as it was already complete
            // which means this could have been a for loop
            linkCounter++;
            
            // Nodes already in same network
            if (node1Network.Any() && node2Network.Any() && node1Network.Single() == node2Network.Single())
            {
                continue;
            }

            //Node 1 in network, node 2 in network -> add node 2's network to node 1's network, and delete node 2's network
            if(node1Network.Any() && node2Network.Any())
            {
                var node1N = node1Network.Single();
                var node2N = node2Network.Single();
                node1N.AddNetwork(node2N);
                networkTracker.Remove(node2N);
            } 
            //Node 1 IN network, node 2 not in network  -> add node 2 to node 1's network
            else if(node1Network.Any() && !node2Network.Any())
            {
                node1Network.Single().AddJunctionBox(node2);
            }
            else if(node2Network.Any() && !node1Network.Any())
            {
                node2Network.Single().AddJunctionBox(node1);
            }
            //Node 1 NOT in network, node 2 NOT in netwrk -> create new network
            else
            {
                var network = new JunctionBoxNetwork();
                network.AddJunctionBox(node1);
                network.AddJunctionBox(node2);
                networkTracker.Add(network);
            }
        }

        var x = networkTracker.OrderByDescending(network => network.Nodes.Count).Take(3).Select(network => network.Nodes.Count).ToList();
        output = x[0] * x[1] * x[2];

        return new($"{output}");
    }

    public override ValueTask<string> Solve_2()
    {
        long output = 0;

        return new($"{output}");
    }

}