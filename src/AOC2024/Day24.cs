using System.ComponentModel;
using System.Reflection.Metadata;
using System.Security;
using AdventOfCode.Base.Misc;

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
        Console.WriteLine($"Part 1 confirmation {valueNodes.GetZValue()}");

        // Print out the chains of interest for manually debugging
        // Console.WriteLine(" **** z12 **** ");
        // PrintChain(valueNodes["z12"]);

        // Fix the broken input
        // valueNodes.SwapOps("z12", "vdc");
        // valueNodes.SwapOps("z21", "nhn");
        // valueNodes.SwapOps("tvb", "khg");
        // valueNodes.SwapOps("z33", "gst");

        // // Then see if the logic can undo these swaps.
        // valueNodes.SwapOps("z06", "jft");
        // valueNodes.SwapOps("z12", "z11");
        // valueNodes.SwapOps("z30", "z31");
        // valueNodes.SwapOps("z40", "fdv"); //dfs,fdv

        // int swapsFound = 0;
        var allSwaps = new List<string>();
        while (allSwaps.Count < 8)
        {
            // Run the bit test case thru until it fails.            
            long testOutput = 0;
            int inputBit = 0;
            long testCase = 0;
            for (inputBit = 1; inputBit <= 44; inputBit++)
            {
                testCase = Convert.ToInt64(Math.Pow(2, inputBit));
                valueNodes.ResetComputer();
                valueNodes.LoadXValue(testCase);
                valueNodes.LoadYValue(0);
                computeNodes.Compute();
                testOutput = valueNodes.GetZValue();
                if (testOutput != testCase)
                {
                    break;
                }
            }

            // Generate list of possible swaps, involving all precursors to the expected output bit, and all precursors to the actual output bit
            var outputBit = testOutput.GetSingleBitSet();
            var expectedOutValue = valueNodes[inputBit > 10 ? $"z{inputBit}" : $"z0{inputBit}"];
            var actualOutValue = valueNodes[outputBit > 10 ? $"z{outputBit}" : $"z0{outputBit}"];
            var swapTries = FindAllPrecursorOperations(expectedOutValue).Select(x => x.Output.Name).ToList();
            swapTries.AddRange(FindAllPrecursorOperations(actualOutValue).Select(x => x.Output.Name).ToList());

            Console.WriteLine($"Test Case failed. InputBit {inputBit}. TestCase {testCase}. Actual {testOutput} Bit {outputBit}.");
            Console.WriteLine($"There are {swapTries.Count} items to try and swap. {string.Join(",", swapTries)}");

            // Swap loop
            bool swapFound = false;
            string swap1 = string.Empty, swap2 = string.Empty;

            for (int i = 0; i < swapTries.Count - 1; i++)
            {
                swap1 = swapTries[i];
                for (int j = i + 1; j < swapTries.Count; j++)
                {
                    swap2 = swapTries[j];

                    valueNodes.SwapOps(swap1, swap2);
                    try
                    {
                        var testCaseBits = Enumerable.Range(1, Math.Max(inputBit, outputBit));
                        foreach (var bit in testCaseBits)
                        {
                            testCase = Convert.ToInt64(Math.Pow(2, bit));

                            valueNodes.ResetComputer();
                            valueNodes.LoadXValue(testCase);
                            valueNodes.LoadYValue(0);
                            computeNodes.Compute();
                            if (valueNodes.GetZValue() != testCase)
                            {
                                throw new Exception("Failed X only test case");
                            }

                            valueNodes.ResetComputer();
                            valueNodes.LoadXValue(testCase - 1);
                            valueNodes.LoadYValue(1);
                            computeNodes.Compute();
                            if (valueNodes.GetZValue() != testCase)
                            {
                                throw new Exception("Failed X-1, 1 test case");
                            }

                            valueNodes.ResetComputer();
                            valueNodes.LoadXValue(0);
                            valueNodes.LoadYValue(testCase);
                            computeNodes.Compute();
                            if (valueNodes.GetZValue() != testCase)
                            {
                                throw new Exception("Failed Y only test case");
                            }

                            valueNodes.ResetComputer();
                            valueNodes.LoadXValue(1);
                            valueNodes.LoadYValue(testCase - 1);
                            computeNodes.Compute();
                            if (valueNodes.GetZValue() != testCase)
                            {
                                throw new Exception("Failed 1 Y-1 only test case");
                            }

                            valueNodes.ResetComputer();
                            valueNodes.LoadXValue(testCase);
                            valueNodes.LoadYValue(testCase);
                            computeNodes.Compute();
                            if (valueNodes.GetZValue() != 2 * testCase)
                            {
                                throw new Exception("Failed XY double test case");
                            }

                        }

                        //Assuming only 1 swap will pass our test cases
                        //Swap was good, break out of swap loop
                        swapFound = true;
                        break;
                    }
                    catch (Exception)
                    {
                        // Undo the bad swap
                        valueNodes.SwapOps(swap1, swap2);
                    }
                }

                if (swapFound)
                {
                    //If we found a swap, add to the list and then get out of outer loop
                    allSwaps.Add(swap1);
                    allSwaps.Add(swap2);
                    break;
                }
            }

            // Debug lines
            if (swapFound)
            {
                Console.WriteLine($"Great success swapping {swap1} and {swap2}");
            }
            else
            {
                Console.WriteLine($"Swaps all failed :(");
                throw new Exception("All swaps failed. Recheck assumptions");
            }
        }


        Console.WriteLine($"Found 8 swaps. {string.Join(",", allSwaps)}");

        Console.WriteLine("Shakedown testing. You should see no failures");
        // Random testing. 
        for (int i = 0; i < 100; i++)
        {
            var x = Random.Shared.NextInt64(0, (long)Math.Pow(2, 44));
            var y = Random.Shared.NextInt64(0, (long)Math.Pow(2, 44));
            valueNodes.ResetComputer();
            valueNodes.LoadXValue(x);
            valueNodes.LoadYValue(y);
            computeNodes.Compute();
            var toutput = valueNodes.GetZValue();
            if (toutput != (x + y))
            {
                Console.WriteLine($"X {x} Y {y} Expected {x + y} Actual {toutput}");
            }
        }
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2 {string.Join(",", allSwaps.Order())}");
    }

    // private List<ComputeNode> FindOpsInCommon(ValueNode node1, ValueNode node2)
    // {
    //     var node1Inputs = FindAllPrecursorOperations(node1);
    //     var node2Inputs = FindAllPrecursorOperations(node2);
    //     return node1Inputs.Where(node => node2Inputs.Contains(node)).ToList();
    // }

    // private List<ComputeNode> FindUniqueOps(ValueNode node1, ValueNode node2)
    // {
    //     var node1Inputs = FindAllPrecursorOperations(node1);
    //     var node2Inputs = FindAllPrecursorOperations(node2);
    //     var output = node1Inputs.Where(node => !node2Inputs.Contains(node)).ToList();
    //     output.AddRange(node2Inputs.Where(node => !node1Inputs.Contains(node)).ToList());
    //     return output;
    // }

    private static List<ComputeNode> FindAllPrecursorOperations(ValueNode node)
    {
        if (node.PrevOperation == null)
            return [];

        var output = new List<ComputeNode> { node.PrevOperation };
        output.AddRange(FindAllPrecursorOperations(node.PrevOperation.Input1));
        output.AddRange(FindAllPrecursorOperations(node.PrevOperation.Input2));

        return output;
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
    // public ComputeNode? NextOperation { get; set; } //would need to be a list if we need forward tracking
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
