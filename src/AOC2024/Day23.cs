public class LinkedNode<T>
{
    public T Value { get; init; }
    public bool Visited { get; set; }

    public LinkedNode(T value)
    {
        Value = value;
    }

    public HashSet<LinkedNode<T>> Edges { get; init; } = new();
}


public class Day23 : BaseDay
{
    List<string> _lines = new();
    List<LinkedNode<string>> nerds = new();

    Dictionary<string, Computer> computers = new Dictionary<string, Computer>();

    public Day23()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            // mine
            _lines.Add(line);
            var names = line.Split('-');
            var n1 = nerds.Where(name => name.Value == names[0]).FirstOrDefault();
            var n2 = nerds.Where(name => name.Value == names[1]).FirstOrDefault();
            if (n1 == null)
            {
                n1 = new LinkedNode<string>(names[0]);
                nerds.Add(n1);
            }
            if (n2 == null)
            {
                n2 = new LinkedNode<string>(names[1]);
                nerds.Add(n2);
            }
            n1.Edges.Add(n2);
            n2.Edges.Add(n1);

            //other
            foreach (var name in names)
                if (!computers.ContainsKey(name))
                    computers.Add(name, new Computer(name));

            computers[names[0]].Connections.Add(computers[names[1]]);
            computers[names[1]].Connections.Add(computers[names[0]]);

        }
    }



    public override ValueTask<string> Solve_1()
    {
        var answer = string.Empty;
        foreach (var computerName in computers.Keys)
        {
            var connections = findMutualConnections(computers[computerName]);
            removeComputersThatAreNotConnectedToAllOthers(connections);
            setAnswer(computerName, connections);
        }

        Console.WriteLine(answer);
        return new(answer);

        HashSet<string> findMutualConnections(Computer computer)
        {
            var mutualConnections = new HashSet<string>();
            foreach (var computerConnection in computer.Connections)
                foreach (var secondGradeConnection in computerConnection.Connections)
                    if (computer.Connections.Contains(secondGradeConnection))
                    {
                        mutualConnections.Add(computerConnection.Name);
                        mutualConnections.Add(secondGradeConnection.Name);
                    }

            return mutualConnections;
        }
        void removeComputersThatAreNotConnectedToAllOthers(HashSet<string> computerNames)
        {
            if (computerNames.Count == 0) return;

            var toRemove = new List<string>();
            foreach (var name1 in computerNames)
                foreach (var name2 in computerNames)
                    if (name1 != name2)
                        if (!computers[name1].Connections.Contains(computers[name2]))
                            toRemove.Add(name1);

            foreach (var name in toRemove)
                computerNames.Remove(name);
        }

        void setAnswer(string computerName, HashSet<string> connections)
        {
            if (connections.Count == 0) return;

            var list = connections.ToList();
            list.Add(computerName);
            list.Sort();

            var password = string.Empty;
            foreach (var name in list)
                password += $"{name},";

            if (password.Length > answer.Length + 1)
                answer = password[..^1];
        }
        // var triForce = Find3Ways();
        // var triForce = FindTriangles(nerds);
        // Console.WriteLine($"There are {triForce.Count} 3 ways to search");
        // var output = triForce.Where( t => t.Item1.StartsWith('t') || t.Item2.StartsWith('t') || t.Item3.StartsWith('t')).Count();
        // return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1 {output}");
    }
    private List<(string, string, string)> FindTriangles(List<LinkedNode<string>> graph)
    {
        var output = new List<(string, string, string)>();
        var nodes = graph.OrderBy(node => node.Edges.Count).ToList();
        while (nodes.Count > 2)
        {
            var vi = nodes[0];
            vi.Edges.ToList().ForEach(edge => edge.Visited = true);
            // var marks = vi.Edges.ToDictionary(x => x.Value, _ => true);
            foreach (var u in vi.Edges)
            {
                foreach (var w in u.Edges)
                {
                    if (w.Visited)
                    // if (marks.ContainsKey(w.Value))
                    {
                        var tri = new List<string> { vi.Value, u.Value, w.Value }.Order().ToList();
                        var x = (tri[0], tri[1], tri[2]);
                        if (!output.Contains(x))
                        {
                            output.Add(x);
                        }
                        u.Visited = false;
                        // marks.Remove(u.Value); 
                    }
                }
            }
            vi.Edges.ToList().ForEach(edge => edge.Visited = false);
            nodes.RemoveAt(0);
        }

        return output;
    }

    // private List<(string, string, string)> Find3Ways()
    // {
    //     var output = new List<(string, string, string)>();
    //     foreach (var nerd in nerds)
    //     {
    //         for (int i = 0; i < nerd.Edges.Count - 1; i++)
    //         foreach(var linked1 in nerd.Edges)
    //         {
    //             for (int j = 1; j < nerd.Edges.Count; j++)
    //             {
                    
    //                 var linked1 = nerd.Edges[i];
    //                 var linked2 = nerd.Edges[j];
    //                 if (linked1.Edges.Contains(linked2))
    //                 {
    //                     var tri = new List<string> { nerd.Value, linked1.Value, linked2.Value }.Order().ToList();
    //                     var x = (tri[0], tri[1], tri[2]);
    //                     if (!output.Contains(x))
    //                     {
    //                         output.Add(x);
    //                     }
    //                 }
    //             }
    //         }
    //     }
    //     return output;
    // }
    public override ValueTask<string> Solve_2()
    {
        var allCliques = BronKerbosch1([], nerds.ToHashSet(), []);
        var biggestGroup = allCliques.OrderBy(g => g.Count).Last();
        var output = string.Join(",", biggestGroup.Select(x => x.Value).Order());

        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2 `{output}`");
    }

    private bool MutuallyLinked(IEnumerable<LinkedNode<string>> nodes)
    {
        foreach (var node in nodes)
        {
            var targetLinkValues = nodes.Select(n => n.Value);
            var matches = node.Edges.Count(e => targetLinkValues.Contains(e.Value));
            if (matches != targetLinkValues.Count() - 1) return false;
        }
        return true;

    }

    private List<HashSet<LinkedNode<string>>> BronKerbosch1(HashSet<LinkedNode<string>> rPotentialClique, HashSet<LinkedNode<string>> pRemaining, HashSet<LinkedNode<string>> xSkip)
    {
        var output = new List<HashSet<LinkedNode<string>>>();
        if (pRemaining.Count == 0 && xSkip.Count == 0)
        {
            output.Add(rPotentialClique);
            return output;
        }
        //choose a pivot vertex u in P ⋃ X
        var temp = pRemaining.ToHashSet();
        temp.UnionWith(xSkip);
        // pick a pivot node from temp.
        var maxEdges = temp.Select(n => n.Edges.Count).Max();
        var pivotU = temp.Where( n => n.Edges.Count == maxEdges).First();
        Console.WriteLine($"The node with the most edges is pivotU {pivotU.Value}");
        //(d, pivot) = max([(len(G[v]), v) for v in P.union(X)])
        // for each vertex v in P \ N(u) do
        var toProcess = pRemaining.ToHashSet();
        toProcess.ExceptWith(pivotU.Edges);

        while (toProcess.Count > 0)
        {
            Console.WriteLine($"ToProcess: {toProcess.Count}");
            var node = toProcess.First();
            
            var newPotentialClique = rPotentialClique.ToHashSet();
            newPotentialClique.Add(node);
            
            var newPRemaining = pRemaining.ToHashSet();
            newPRemaining.IntersectWith(node.Edges);
            
            var newXSkip = xSkip.ToHashSet();
            newXSkip.IntersectWith(node.Edges);
            
            var results = BronKerbosch1(newPotentialClique, newPRemaining, newXSkip);

            foreach (var result in results)
                output.Add(result);

            toProcess.Remove(node);
            xSkip.Add(node);
        }
        return output;
    }

    // private string FindLanParty2()
    // {
    //     var output = string.Empty;
    //     foreach (var nerd in nerds)
    //     {
    //         List<List<LinkedNode<string>>> groups = new();
    //         foreach (var link in nerd.Edges)
    //         {
    //             groups.Add(new List<LinkedNode<string>> { link, nerd });
    //         }
    //         for (int i = 1; i < nerd.Edges.Count; i++)
    //         {
    //             for (int g = 0; g < groups.Count; g++)
    //             {
    //                 if (MutuallyLinked(groups[g].Append(nerd.Edges[i])))
    //                 {
    //                     groups[g].Add(nerd.Edges[i]);
    //                 }
    //             }
    //         }
    //         var biggestGroup = groups.OrderBy(g => g.Count).Last();
    //         var bgPassword = string.Join(",", biggestGroup.Select(x => x.Value).Order());
    //         if (bgPassword.Length > output.Length)
    //             output = bgPassword;
    //     }

    //     return output;
    // }

    private string FindLanParty()
    {
        var output = string.Empty;
        int largestGroup = 0;
        foreach (var nerd in nerds)
        {
            var possibleLans = GenerateCombos(nerd, largestGroup + 1);
            foreach (var possibleLan in possibleLans)
            {
                if (MutuallyLinked(possibleLan))
                {
                    if (possibleLan.Count() > largestGroup)
                    {
                        largestGroup = possibleLan.Count();
                        output = string.Join(",", possibleLan.Select(x => x.Value).Order());
                    }
                }
            }
        }
        return output;
    }


    // Generates all possible combos
    IEnumerable<IEnumerable<LinkedNode<string>>> GenerateCombos(LinkedNode<string> node, int minSize)
    {
        var output = new List<IEnumerable<LinkedNode<string>>>();
        var largestPossible = node.Edges.Count + 1;
        var options = node.Edges.ToList();
        options.Add(node);
        for (int i = minSize; i <= largestPossible; i++)
        {
            var newCombos = options.Combinations(i);
            foreach (var combo in newCombos)
                output.Add(combo);
        }
        return output;
    }

    /*

    foreach (var line in lines)
    {
        var names = line.Split('-');
        foreach (var name in names)
            if (!computers.ContainsKey(name))
                computers.Add(name, new Computer(name));

        computers[names[0]].connections.Add(computers[names[1]]);
        computers[names[1]].connections.Add(computers[names[0]]);
    }

    var answer = string.Empty;
    foreach (var computerName in computers.Keys)
    {
        var connections = findMutualConnections(computers[computerName]);
        removeComputersThatAreNotConnectedToAllOthers(connections);
        setAnswer(computerName, connections);
    }

    Console.WriteLine(answer);

    HashSet<string> findMutualConnections(Computer computer)
    {
        var mutualConnections = new HashSet<string>();
        foreach (var computerConnection in computer.connections)
            foreach (var secondGradeConnection in computerConnection.connections)
                if (computer.connections.Contains(secondGradeConnection))
                {
                    mutualConnections.Add(computerConnection.name);
                    mutualConnections.Add(secondGradeConnection.name);
                }

        return mutualConnections;
    }

    void removeComputersThatAreNotConnectedToAllOthers(HashSet<string> computerNames)
    {
        if (computerNames.Count == 0) return;

        var toRemove = new List<string>();
        foreach (var name1 in computerNames)
            foreach (var name2 in computerNames)
                if (name1 != name2)
                    if (!computers[name1].connections.Contains(computers[name2]))
                        toRemove.Add(name1);

        foreach (var name in toRemove)
            computerNames.Remove(name);
    }

    void setAnswer(string computerName, HashSet<string> connections)
    {
        if (connections.Count == 0) return;

        var list = connections.ToList();
        list.Add(computerName);
        list.Sort();

        var password = string.Empty;
        foreach (var name in list)
            password += $"{name},";

        if (password.Length > answer.Length + 1)
            answer = password[..^1];
    }

    */


}

public class Computer(string name)
{
    public string Name { get; set; } = name;
    public List<Computer> Connections { get; set; } = [];
}

/*
Extension method for generating combinations of elements
*/
public static class ComboExtensions
{
    public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
    {
        return k == 0 ? [[]] :
        elements.SelectMany((e, i) =>
        elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
    }

}