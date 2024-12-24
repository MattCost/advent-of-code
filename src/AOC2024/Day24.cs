using System.ComponentModel;
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
        var valueNodes = new Dictionary<string, ValueNode>();
        var computeNodes = new List<ComputeNode>();

        // Create Value nodes for all Xs and Ys
        foreach (var inits in _signals)
        {
            var name = inits.Key;
            var value = inits.Value;
            valueNodes[name] = new ValueNode(name) { Value = value ? 1 : 0 };
        }

        // Create Value nodes for all operation outputs
        foreach (var op in _operations)
        {
            var name = op.output;
            if (!valueNodes.ContainsKey(name))
                valueNodes[name] = new ValueNode(name);
        }

        // Create each operation, linked to the correct nodes
        foreach (var op in _operations)
        {
            var input1 = valueNodes[op.in1];
            var input2 = valueNodes[op.in2];
            var output1 = valueNodes[op.output];
            var compNode = new ComputeNode(input1, input2, op.op, output1);
            computeNodes.Add(compNode);
            // This needs to be a list, not a single
            // input1.NextOperation = compNode;
            // input2.NextOperation = compNode;
            output1.PrevOperation = compNode;
        }

        computeNodes.Compute();
        var output = valueNodes.GetZValue();
        Console.WriteLine($"Part 1 confirmation {output}");

        // Print out the chains of interest for manually debugging
        // Console.WriteLine(" **** z12 **** ");
        // PrintChain(valueNodes["z12"]);

        // Console.WriteLine(" **** z13 **** ");
        // PrintChain(valueNodes["z13"]);

        // Console.WriteLine(" **** z21 **** ");
        // PrintChain(valueNodes["z21"]);

        // Console.WriteLine(" **** z22 **** ");
        // PrintChain(valueNodes["z22"]);

        // Console.WriteLine(" **** z25 **** ");
        // PrintChain(valueNodes["z25"]);

        // Console.WriteLine(" **** z26 **** ");
        // PrintChain(valueNodes["z26"]);

        // Console.WriteLine(" **** z33 **** ");
        // PrintChain(valueNodes["z33"]);

        // Console.WriteLine(" **** z34 **** ");
        // PrintChain(valueNodes["z34"]);

        valueNodes.SwapOps("z12", "vdc"); // right i think
        // //pps (close but wrong)
        valueNodes.SwapOps("z21", "nhn"); // right i think
        valueNodes.SwapOps("tvb", "khg"); //brute :)
        // //khg (close but wrong) mfp (close but wrong)
        valueNodes.SwapOps("z33", "gst"); //right maybe

        //BRUTE
        // var swapTrys = FindAllInputs(valueNodes["z25"]).Select(x => x.Output.Name).ToList();
        // swapTrys.AddRange(FindAllInputs(valueNodes["z26"]).Select(x =>x.Output.Name).ToList());

        // // /var swapTrys = new List<string> {"z25", "z26", "mfp", "jhd", "tbn", "grb", "rqd", "rkf", "mmw", "nrq", "vkg", "jkq", "nhn", "rsc", "qfn", "ddw", "dmb", "rtg", "sdp", "ptd", "jmr", "nhh", "tgq", "nqm", "fmf", "tnt", "rrj", "fwg", "srk", "sjm", "dwt", "qsg", "csf", "ggg", "hvg", "fbk", "vdc", "pps", "wdg", "rvg", "wvv", "spg", "ptj", "dtg", "fjn", "bsm", "vsc", "cbk", "ccd", "qtg", "dbm", "pmv", "cnm", "dgh", "bgq", "wdh", "jhv", "chs", "rgv", "ndp", "qfk", "ssj", "ccr", "fpk", "dfh", "bcr", "nmw", "bvk", "gpk", "spq", "khh", "bgp", "psw", "pvj", "nfq", "nph", "gqg", "jft", "knh", "cst", "nbm", "htc", "hhm", "kcp", "wdg", "rvg", "wvv", "spg", "ptj", "dtg", "fjn", "bsm", "vsc", "cbk", "ccd", "qtg", "dbm", "pmv", "cnm", "dgh", "bgq", "wdh", "jhv", "chs", "rgv", "ndp", "qfk", "ssj", "ccr", "fpk", "dfh", "bcr", "nmw", "bvk", "gpk", "spq", "khh", "bgp", "psw", "pvj", "nfq", "nph", "gqg", "jft", "knh", "cst", "nbm", "htc", "hhm", "pps", "tcq", "bck", "gtf", "mft", "khn", "nhm", "kfd", "crv", "fht", "fct", "bbn", "ncf", "mhq", "khg" };
        // // var swapTrys = new List<string> {"z25", "z26", "khg", "jhd", "tbn", "grb", "mfp"};
        // for (int i = 0; i < swapTrys.Count - 1; i++)
        // {
        //     var swap1 = swapTrys[i];
        //     for (int j = i+1; j < swapTrys.Count; j++)
        //     {
        //         var swap2 = swapTrys[j];
        //         valueNodes.SwapOps(swap1, swap2);
        //         try
        //         {
        //             var testCaseBits = Enumerable.Range(1, 28);
        //             foreach (var bit in testCaseBits)
        //             {
        //                 long testCase = Convert.ToInt64(Math.Pow(2, bit));
        //                 valueNodes.ResetComputer();
        //                 valueNodes.LoadXValue(testCase-1);
        //                 valueNodes.LoadYValue(1);
        //                 computeNodes.Compute();
        //                 output = valueNodes.GetZValue();
        //                 if (output != testCase)
        //                 {
        //                     throw new Exception("Failed X test case");
        //                     // Console.WriteLine($"Test Case {bit}: x : {testCase-1} + y : {1} : Output {output}");
        //                 }

        //                 valueNodes.ResetComputer();
        //                 valueNodes.LoadXValue(0);
        //                 valueNodes.LoadYValue(testCase);
        //                 computeNodes.Compute();
        //                 output = valueNodes.GetZValue();
        //                 if (output != testCase)
        //                 {
        //                     throw new Exception("Failed Y test case");
        //                     // Console.WriteLine($"Test Case {bit}: x:{0} + y:{testCase} : Output {output}");
        //                 }
        //             }
        //             Console.WriteLine($"**************************** SWAP {swap1} {swap2} is a winner?");

        //         }
        //         catch (Exception)
        //         {
        //             // Console.WriteLine($"Swap {swap} is a stinker. {ex.Message} ");
        //         }

        //         valueNodes.SwapOps(swap1, swap2);
        //     }
        // }

        // var suspects = FindOpsInCommon(valueNodes["z25"], valueNodes["z26"]);
        // foreach(var suspect in suspects)
        //     Console.WriteLine(suspect);

        //gst,khg,nhn,vdc,z12,z21,z25,z33 (wrong)

        // Test Loop. Generate an input with a single bit set. X+0 should ==X. If not you can figure out which 2 output bits are getting swapped then debug
        // var testCaseBits = Enumerable.Range(1, 44);
        // foreach (var bit in testCaseBits)
        // {
        //     long testCase = Convert.ToInt64(Math.Pow(2, bit));
        //     valueNodes.ResetComputer();
        //     valueNodes.LoadXValue(testCase);
        //     valueNodes.LoadYValue(0);
        //     computeNodes.Compute();
        //     output = valueNodes.GetZValue();
        //     if (output != testCase)
        //     {
        //         Console.WriteLine($"Test Case {bit}: x : {testCase} + y : {0} : Output {output}");
        //     }

        //     valueNodes.ResetComputer();
        //     valueNodes.LoadXValue(0);
        //     valueNodes.LoadYValue(testCase);
        //     computeNodes.Compute();
        //     output = valueNodes.GetZValue();
        //     if (output != testCase)
        //     {
        //         Console.WriteLine($"Test Case {bit}: x:{0} + y:{testCase} : Output {output}");
        //     }
        // }

        var testCases = new List<long> { 
            // bit 12 related
            4095, 4096, 4097, 18255,
            12288, 28672, 8417280, 8392704, 8400896,
            // bit 21 related
            2097152, 6291456, 7340032
             };

        foreach (var testCase in testCases)
        {
            valueNodes.ResetComputer();
            valueNodes.LoadXValue(0);
            valueNodes.LoadYValue(testCase);
            computeNodes.Compute();
            output = valueNodes.GetZValue();
            if (output != testCase)
            {
                Console.WriteLine($"Test Case x: {testCase} + y: {0} : Output {output}");
            }
        }
        
        for (int i = 0; i < 100; i++)
        {
            var x = Random.Shared.NextInt64(0, (long)Math.Pow(2, 44));
            var y = Random.Shared.NextInt64(0, (long)Math.Pow(2, 44));
            valueNodes.ResetComputer();
            valueNodes.LoadXValue(x);
            valueNodes.LoadYValue(y);
            computeNodes.Compute();
            output = valueNodes.GetZValue();
            if (output != (x + y))
            {
                Console.WriteLine($"X {x} Y {y} Expected {x + y} Actual {output}");
            }
        }
        var realOutput = string.Join(",", new List<string> { "gst","khg","nhn","tvb","vdc","z12","z21","z33"}.Order());
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2 {realOutput}");
    }

    private List<ComputeNode> FindOpsInCommon(ValueNode node1, ValueNode node2)
    {
        var node1Inputs = FindAllInputs(node1);
        var node2Inputs = FindAllInputs(node2);
        return node1Inputs.Where(node => node2Inputs.Contains(node)).ToList();
    }

    private List<ComputeNode> FindUniqueOps(ValueNode node1, ValueNode node2)
    {
        var node1Inputs = FindAllInputs(node1);
        var node2Inputs = FindAllInputs(node2);
        var output = node1Inputs.Where(node => !node2Inputs.Contains(node)).ToList();
        output.AddRange(node2Inputs.Where(node => !node1Inputs.Contains(node)).ToList());
        return output;
    }
    private List<ComputeNode> FindAllInputs(ValueNode node)
    {
        if (node.PrevOperation == null)
            return [];

        var output = new List<ComputeNode> { node.PrevOperation };
        output.AddRange(FindAllInputs(node.PrevOperation.Input1));
        output.AddRange(FindAllInputs(node.PrevOperation.Input2));

        return output;
    }

    private string? GeneratePrintChain(ValueNode node)
    {
        //Print out
        // ( PrintChain2(node.prevOp.Input1) ) OP ( PrintChain2(node.PrevOp.Input2) 
        if (node.PrevOperation == null) return null;

        var printOut = node.PrevOperation.GetStackString();
        var input1Replace = GeneratePrintChain(node.PrevOperation.Input1) ?? node.PrevOperation.Input1.Name;
        var input2Replace = GeneratePrintChain(node.PrevOperation.Input2) ?? node.PrevOperation.Input2.Name;

        return printOut.Replace(node.PrevOperation.Input1.Name, $"({input1Replace})").Replace(node.PrevOperation.Input2.Name, $"({input2Replace})");

    }
    private void PrintChain(ValueNode node, int indent = 75)
    {
        var prevOp = node.PrevOperation;
        if (prevOp == null)
            return;

        var input1 = prevOp.Input1;
        var input2 = prevOp.Input2;


        PrintChain(input1, indent - 1);

        for (int t = 0; t < indent; t++)
            Console.Write("\t");

        Console.Write($"{input1.Name} {prevOp.Operation} {input2.Name} --> {node.Name}\n");

        PrintChain(input2, indent - 1);
        return;
    }
}
public static class ValueNodeExtensions
{
    public static void Compute(this List<ComputeNode> computeNodes)
    {
        int loopCount = 0;
        while (true)
        {
            if (loopCount++ > 100)
                throw new Exception("Deadlock");
            var unprocessed = computeNodes.Where(node => node.Output.Value == -1);
            if (!unprocessed.Any()) break;
            foreach (var node in unprocessed)
                node.TryDoOperation();

        }

    }
    public static void SwapOps(this Dictionary<string, ValueNode> values, string op1, string op2)
    {
        var v1 = values[op1];
        var v2 = values[op2];
        var v1Prev = v1.PrevOperation;
        var v2Prev = v2.PrevOperation;
        if (v1Prev == null) throw new Exception("v1 prev op is null!");
        if (v2Prev == null) throw new Exception("v2 prev op is null!");
        v1Prev.Output = v2;
        v1.PrevOperation = v2Prev;
        v2Prev.Output = v1;
        v2.PrevOperation = v1Prev;

    }
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
        for (int bit = 0; bit <= 44; bit++)
        {
            var name = bit < 10 ? $"x0{bit}" : $"x{bit}";
            var bitValue = (value & (long)Math.Pow(2, bit)) > 0 ? 1 : 0;
            values[name].Value = bitValue;
        }
    }

    public static void LoadYValue(this Dictionary<string, ValueNode> values, long value)
    {
        for (int bit = 0; bit <= 44; bit++)
        {
            var name = bit < 10 ? $"y0{bit}" : $"y{bit}";
            var bitValue = (value & (long)Math.Pow(2, bit)) > 0 ? 1 : 0;
            values[name].Value = bitValue;
        }
    }

    public static void ResetComputer(this Dictionary<string, ValueNode> values)
    {
        foreach (var node in values.Values)
            node.Value = -1;
    }

}

public class ValueNode
{
    public string Name { get; set; }
    public int Value { get; set; } = -1;
    // public ComputeNode? NextOperation { get; set; }
    public ComputeNode? PrevOperation { get; set; }
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
    public override string ToString()
    {
        return $"{Input1.Name} {Operation} {Input2.Name} --> {Output.Name}";
    }

    public string GetStackString()
    {
        return $"{Input1.Name} {Operation} {Input2.Name}";

    }
}
