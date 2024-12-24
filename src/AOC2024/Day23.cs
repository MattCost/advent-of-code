public class LinkedNode<T>
{
    public T Value { get; init; }
    public bool Visited { get; set; }

    public LinkedNode(T value)
    {
        Value = value;
    }

    public List<LinkedNode<T>> Edges { get; init; } = new();
}


public class Day23 : BaseDay
{
    List<string> _lines = new();
    List<LinkedNode<string>> nerds = new();

    public Day23()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
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
        }
    }

    public override ValueTask<string> Solve_1()
    {
        // var triForce = Find3Ways();
        var triForce = FindTriangles(nerds);
        Console.WriteLine($"There are {triForce.Count} 3 ways to search");
        var output = triForce.Where( t => t.Item1.StartsWith('t') || t.Item2.StartsWith('t') || t.Item3.StartsWith('t')).Count();
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1 {output}");
    }
    private List<(string, string, string)> FindTriangles(List<LinkedNode<string>> graph)
    {
        var output = new List<(string, string, string)>();
        var nodes = graph.OrderBy(node => node.Edges.Count).ToList();
        while (nodes.Count > 2)
        {
            var vi = nodes[0];
            vi.Edges.ForEach(edge => edge.Visited = true);
            // var marks = vi.Edges.ToDictionary(x => x.Value, _ => true);
            foreach (var u in vi.Edges)
            {
                foreach (var w in u.Edges)
                {
                    if(w.Visited)
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
            vi.Edges.ForEach(edge => edge.Visited =false);
            nodes.RemoveAt(0);
        }

        return output;
    }

    private List<(string, string, string)> Find3Ways()
    {
        var output = new List<(string, string, string)>();
        foreach (var nerd in nerds)
        {
            for (int i = 0; i < nerd.Edges.Count - 1; i++)
            {
                for (int j = 1; j < nerd.Edges.Count; j++)
                {
                    var linked1 = nerd.Edges[i];
                    var linked2 = nerd.Edges[j];
                    if (linked1.Edges.Contains(linked2))
                    {
                        var tri = new List<string> { nerd.Value, linked1.Value, linked2.Value }.Order().ToList();
                        var x = (tri[0], tri[1], tri[2]);
                        if (!output.Contains(x))
                        {
                            output.Add(x);
                        }
                    }
                }
            }
        }
        return output;
    }
    public override ValueTask<string> Solve_2()
    {
        var allCliques = BronKerbosch1([], nerds, []);
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

    private List<List<LinkedNode<string>>> BronKerbosch1(List<LinkedNode<string>> rPotentialClique, List<LinkedNode<string>> pRemaining, List<LinkedNode<string>> xSkip)
    {
        var output = new List<List<LinkedNode<string>>>();
        if (pRemaining.Count == 0 && xSkip.Count == 0)
        {
            output.Add(rPotentialClique);
            return output;
        }

        while (pRemaining.Count > 0)
        {
            var node = pRemaining[0];
            var neighbors = node.Edges.Select(edge => edge.Value);
            var results = BronKerbosch1([.. rPotentialClique, node], pRemaining.Where(n => neighbors.Contains(n.Value)).ToList(), xSkip.Where(n => neighbors.Contains(n.Value)).ToList());

            foreach (var result in results)
                output.Add(result);

            pRemaining.RemoveAt(0);
            xSkip.Add(node);
        }
        return output;
    }

    private string FindLanParty2()
    {
        var output = string.Empty;
        foreach (var nerd in nerds)
        {
            List<List<LinkedNode<string>>> groups = new();
            foreach (var link in nerd.Edges)
            {
                groups.Add(new List<LinkedNode<string>> { link, nerd });
            }
            for (int i = 1; i < nerd.Edges.Count; i++)
            {
                for (int g = 0; g < groups.Count; g++)
                {
                    if (MutuallyLinked(groups[g].Append(nerd.Edges[i])))
                    {
                        groups[g].Add(nerd.Edges[i]);
                    }
                }
            }
            var biggestGroup = groups.OrderBy(g => g.Count).Last();
            var bgPassword = string.Join(",", biggestGroup.Select(x => x.Value).Order());
            if (bgPassword.Length > output.Length)
                output = bgPassword;
        }

        return output;
    }

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