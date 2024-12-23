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
        Console.WriteLine($"We have {nerds.Count} computers to search");
        var triForce = Find3Ways();
        Console.WriteLine($"There are {triForce.Count} 3 ways to search");
        int output = 0;
        foreach (var triplet in triForce)
        {
            if (triplet.Item1.StartsWith("t") || triplet.Item2.StartsWith("t") || triplet.Item3.StartsWith("t"))
                output++;
        }
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1 {output}");
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
        Console.WriteLine($"Still have {nerds.Count} computers to search");
        var output = FindLanParty2();
        Console.WriteLine("Expecting `az,cg,ei,hz,jc,km,kt,mv,sv,sx,wc,wq,xy`");
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1 `{output}`");
    }

    private bool MutuallyLinked(IEnumerable<LinkedNode<string>> nodes)
    {
        foreach (var node in nodes)
        {
            // var targetLinkValues = nodes.Where(n => n.Value != node.Value).Select(n => n.Value);
            var targetLinkValues = nodes.Select(n => n.Value);
            var matches = node.Edges.Count(e => targetLinkValues.Contains(e.Value));
            if (matches != targetLinkValues.Count() - 1) return false;
        }
        return true;

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
                        //update output
                        output = string.Join(",", possibleLan.Select(x => x.Value).Order());
                    }
                }
            }
        }
        return output;
    }


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

public static class ComboExtensions
{
    public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
    {
        return k == 0 ? new[] { new T[0] } :
        elements.SelectMany((e, i) =>
        elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
    }

}