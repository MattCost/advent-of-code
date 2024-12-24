using System.Security;

public class Day24 : BaseDay
{
    Dictionary<string, bool> _signals = new();
    List<(string in1, string op, string in2, string output)> _operations = new();
    int targetZCount = 0;

    public Day24()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while (!string.IsNullOrEmpty(line = sr.ReadLine()))
        {
            _signals[line.Split(':')[0]] = int.Parse(line.Split(':')[1].Trim()) == 1;
        }

        while (!string.IsNullOrEmpty(line = sr.ReadLine()))
        {
            var split1 = line.Split(" -> ");
            var split2 = split1[0].Split(' ');
            _operations.Add((split2[0], split2[1], split2[2], split1[1]));
            if (split1[1].StartsWith('z')) targetZCount++;
        }

    }

    public override ValueTask<string> Solve_1()
    {
        var operations = _operations.ToList();
        var signals = _signals.ToDictionary(x => x.Key, x => x.Value);
        int zCount = 0;
        while (zCount < targetZCount)
        {
            int i = 0;
            while (i < operations.Count)
            {
                var operation = operations[i];
                if (signals.TryGetValue(operation.in1, out var in1) && signals.TryGetValue(operation.in2, out var in2))
                {
                    var result = operation.op switch
                    {
                        "AND" => in1 & in2,
                        "OR" => in1 | in2,
                        "XOR" => in1 ^ in2,
                        _ => throw new Exception("Bad opcode")
                    };
                    signals[operation.output] = result;
                    operations.RemoveAt(i);
                    if (operation.output.StartsWith('z'))
                        zCount++;
                }
                else
                {
                    i++;
                }
            }
        }

        long output = 0;
        checked
        {
            var calculationBits = signals.Where(kvp => kvp.Key.StartsWith('z')).OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToArray();
            for (int i = 0; i < calculationBits.Length; i++)
            {
                output += (calculationBits[i] ? 1 : 0) * (long)Math.Pow(2, i);
            }

        }

        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1 {output}");
    }

    public override ValueTask<string> Solve_2()
    {
        /*
            inputX
            inputY
                round1a (mhq)
                round1b
                    round2a (rqd)
                        output


        Node-Value
        Node-Operation
        make a bunch of nodes for the inputs
        link them to operations
        link those to the next level of values
        then we can do swaps
        but how do I know where to swap?
        input1=0,
        output should equal input2
        run thru 1/2/4/8/16/etc
        */
        var valueNodes = new Dictionary<string, ValueNode>();
        var computeNodes = new List<ComputeNode>();
        // Create Value nodes for all Xs and Ys and op outputs
        foreach (var inits in _signals)
        {
            var name = inits.Key;
            var value = inits.Value;
            valueNodes[name] = new ValueNode(name) { Value = value ? 1 : 0 };
        }
        foreach (var op in _operations)
        {
            var name = op.output;
            if (!valueNodes.ContainsKey(name))
                valueNodes[name] = new ValueNode(name);
        }

        foreach (var op in _operations)
        {
            var input1 = valueNodes[op.in1];
            var input2 = valueNodes[op.in2];
            var compNode = new ComputeNode(input1, input2, op.op, valueNodes[op.output]);
            computeNodes.Add(compNode);
            input1.NextOperation = compNode;
            input2.NextOperation = compNode;
        }
        
        while(true)
        {
            var unprocessed = computeNodes.Where(node => node.Output.Value == -1);
            if(!unprocessed.Any()) break;
            foreach (var node in unprocessed)
                node.TryDoOperation();
        }
        var output = valueNodes.GetZValue();
        Console.WriteLine($"Part 1 confirmation {output}");

        valueNodes.ResetComputer();
        valueNodes.LoadXValue(10);
        valueNodes.LoadYValue(0);
        while(true)
        {
            var unprocessed = computeNodes.Where(node => node.Output.Value == -1);
            if(!unprocessed.Any()) break;
            foreach (var node in unprocessed)
                node.TryDoOperation();
        }
        output = valueNodes.GetZValue();
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2 {output}");
    }

}
public static class ValueNodeExtensions
{
    public static long GetZValue(this Dictionary<string, ValueNode> values)
    {
        long output = 0;
        checked
        {
            var calculationBits = values.Where(kvp => kvp.Key.StartsWith('z')).OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToArray();
            for (int i = 0; i < calculationBits.Length; i++)
            {
                output += calculationBits[i].Value * (long)Math.Pow(2, i);
            }
        }
        return output;
    }

    public static void LoadXValue(this Dictionary<string, ValueNode> values, long value)
    {
        for(int bit=0 ; bit<=44 ; bit++)
        {
            var name = bit < 10 ? $"x0{bit}" : $"x{bit}";
            var bitValue = (value & (long)Math.Pow(2, bit)) > 0 ? 1 : 0;
            values[name].Value = bitValue;
        }
    }

    public static void LoadYValue(this Dictionary<string, ValueNode> values, long value)
    {
        for(int bit=0 ; bit<=44 ; bit++)
        {
            var name = bit < 10 ? $"y0{bit}" : $"y{bit}";
            var bitValue = (value & (long)Math.Pow(2, bit)) > 0 ? 1 : 0;
            values[name].Value = bitValue;
        }
    }

    public static void ResetComputer(this Dictionary<string, ValueNode> values)
    {
        foreach(var node in values.Values)
            node.Value = -1;
    }

}

public class ValueNode
{
    public string Name { get; set; }
    public int Value { get; set; } = -1;
    public ComputeNode? NextOperation { get; set; }
    public ValueNode(string name) => Name = name;
}
public class ComputeNode
{
    public ValueNode Input1 { get; set; }
    public ValueNode Input2 { get; set; }
    public string Operation { get; set; }
    public ComputeNode(ValueNode in1, ValueNode in2, string op, ValueNode output)
    {
        Input1 = in1;
        Input2 = in2;
        Operation = op;
        Output = output;
    }

    public ValueNode Output { get; set; }
    public void TryDoOperation()
    {
        if (Input1.Value != -1 && Input2.Value != -1)
        {
            var result = Operation switch
            {
                "AND" => Input1.Value & Input2.Value,
                "OR" => Input1.Value | Input2.Value,
                "XOR" => Input1.Value ^ Input2.Value,
                _ => throw new Exception("Bad opcode")

            };
            Output.Value = result;
        }
    }
}
