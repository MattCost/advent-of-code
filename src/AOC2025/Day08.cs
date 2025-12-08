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

        distanceTracker = distanceTracker.OrderBy(x => x.Distance).ToList();

        int networkId = 1;
        Dictionary<string, int> networksByNodeId = new()
        {
            { distanceTracker[0].Node1.Id, networkId },
            { distanceTracker[0].Node2.Id, networkId }
        };

        Dictionary<int, List<string>> nodeIdsByNetwork = new()
        {
            { networkId, [distanceTracker[0].Node1.Id, distanceTracker[0].Node2.Id] }
        };
        networkId++;

        int linkCounter = 1;
        int distanceIndex = 1;
        while (linkCounter < 1000)
        {
            // Get closest 2 boxes;
            var node1 = distanceTracker[distanceIndex].Node1;
            var node2 = distanceTracker[distanceIndex].Node2;
            distanceIndex++;

            // Search for networks
            var node1Network = networksByNodeId.TryGetValue(node1.Id, out int value) ? value : -1;
            var node2Network = networksByNodeId.TryGetValue(node2.Id, out value) ? value : -1;

            // The puzzle description is confusing "nothing happens" implies you don't need to make a link, but it seems to mean nothing happens to the circuit, as it was already complete
            // which means this could have been a for loop
            linkCounter++;

            // Nodes already in same network
            if (node1Network != -1 && node2Network != -1 && node1Network == node2Network)
            {
                continue;
            }

            //Node 1 in network, node 2 in network -> add node 2's network to node 1's network, and delete node 2's network
            if (node1Network != -1 && node2Network != -1)
            {
                var nodesInNetwork2 = nodeIdsByNetwork[node2Network];
                foreach (var node in nodesInNetwork2)
                {
                    networksByNodeId[node] = node1Network;
                }
                nodeIdsByNetwork.Remove(node2Network);

                nodeIdsByNetwork[node1Network].AddRange(nodesInNetwork2);
            }
            //Node 1 IN network, node 2 not in network  -> add node 2 to node 1's network
            else if (node1Network != -1 && node2Network == -1)
            {
                networksByNodeId[node2.Id] = node1Network;
                nodeIdsByNetwork[node1Network].Add(node2.Id);
            }
            //opposite
            else if (node2Network != -1 && node1Network == -1)
            {
                networksByNodeId[node1.Id] = node2Network;
                nodeIdsByNetwork[node2Network].Add(node1.Id);
            }
            // else neither node in a network            
            else
            {
                networksByNodeId[node1.Id] = networkId;
                networksByNodeId[node2.Id] = networkId;
                nodeIdsByNetwork[networkId++] = [node1.Id, node2.Id];
            }
        }

        var x = nodeIdsByNetwork.Values.OrderByDescending(x => x.Count).Take(3).Select(x => x.Count).ToList();
        output = x[0] * x[1] * x[2];

        return new($"{output}");
    }

    public override ValueTask<string> Solve_2()
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

        distanceTracker = distanceTracker.OrderBy(x => x.Distance).ToList();

        int networkId = 1;
        Dictionary<string, int> networksByNodeId = new()
        {
            { distanceTracker[0].Node1.Id, networkId },
            { distanceTracker[0].Node2.Id, networkId }
        };

        Dictionary<int, List<string>> nodeIdsByNetwork = new()
        {
            { networkId, [distanceTracker[0].Node1.Id, distanceTracker[0].Node2.Id] }
        };
        networkId++;


        int distanceIndex = 0;
        while (true)
        {
            // Get closest 2 boxes;
            var node1 = distanceTracker[distanceIndex].Node1;
            var node2 = distanceTracker[distanceIndex].Node2;
            distanceIndex++;

            // Search for networks
            var node1Network = networksByNodeId.TryGetValue(node1.Id, out int value) ? value : -1;
            var node2Network = networksByNodeId.TryGetValue(node2.Id, out value) ? value : -1;

            // Nodes already in same network
            if (node1Network != -1 && node2Network != -1 && node1Network == node2Network)
            {
                continue;
            }


            //Node 1 in network, node 2 in network -> add node 2's network to node 1's network, and delete node 2's network
            if (node1Network != -1 && node2Network != -1)
            {
                var nodesInNetwork2 = nodeIdsByNetwork[node2Network];
                foreach (var node in nodesInNetwork2)
                {
                    networksByNodeId[node] = node1Network;
                }
                nodeIdsByNetwork.Remove(node2Network);

                nodeIdsByNetwork[node1Network].AddRange(nodesInNetwork2);
            }
            //Node 1 IN network, node 2 not in network  -> add node 2 to node 1's network
            else if (node1Network != -1 && node2Network == -1)
            {
                networksByNodeId[node2.Id] = node1Network;
                nodeIdsByNetwork[node1Network].Add(node2.Id);
            }            //opposite
            else if (node2Network != -1 && node1Network == -1)
            {
                networksByNodeId[node1.Id] = node2Network;
                nodeIdsByNetwork[node2Network].Add(node1.Id);
            }

            // else neither node in a network            
            else
            {
                networksByNodeId[node1.Id] = networkId;
                networksByNodeId[node2.Id] = networkId;
                nodeIdsByNetwork[networkId++] = [node1.Id, node2.Id];
            }

            if (nodeIdsByNetwork.Values.Count == 1 && nodeIdsByNetwork.Values.First().Count == _lines.Count)
            {
                var pair = distanceTracker[distanceIndex - 1];
                output = (long)pair.Node1.X * (long)pair.Node2.X;
                break;
            }
        }

        return new($"{output}");
    }
}