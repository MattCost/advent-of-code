using System.Collections.ObjectModel;
using AdventOfCode.Base.Grid;

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
        foreach(var triplet in triForce)
        {
            if(triplet.Item1.StartsWith("t") || triplet.Item2.StartsWith("t") || triplet.Item3.StartsWith("t") )
                output ++;
        }
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1 {output}");
    }

    private List<(string, string, string)> Find3Ways()
    {
        var output = new List<(string, string, string)>();
        foreach (var nerd in nerds)
        {
            for (int i = 0; i < nerd.Edges.Count -1 ; i++)
            {
                for (int j = 1; j < nerd.Edges.Count ; j++)
                {
                    var linked1 = nerd.Edges[i];
                    var linked2 = nerd.Edges[j];
                    if(linked1.Edges.Contains(linked2))
                    {
                        var tri = new List<string> {nerd.Value, linked1.Value, linked2.Value}.Order().ToList();
                        var x = (tri[0], tri[1], tri[2]);
                        if(!output.Contains(x))
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
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2");
    }

}