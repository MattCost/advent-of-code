using System.Collections;
using System.Text;
using Spectre.Console;

public class Day17 : BaseDay
{
    long RegA, RegB, RegC;
    List<int> Program;

    public Day17()
    {
        StreamReader sr = new StreamReader(InputFilePath);
        string? line;
        RegA = long.Parse((line = sr.ReadLine())!.Split(':')[1].Trim());
        RegB = long.Parse((line = sr.ReadLine())!.Split(':')[1].Trim());
        RegC = long.Parse((line = sr.ReadLine())!.Split(':')[1].Trim());
        line = sr.ReadLine();
        Program = (line = sr.ReadLine())!.Split(':')[1].Trim().Split(',').Select(x => int.Parse(x)).ToList();
        Console.WriteLine($"RegA {RegA}, RegB {RegB}, RegC {RegC}");
    }

    public override ValueTask<string> Solve_1()
    {
        StringBuilder tempOutput = new();
        int intPointer = 0;
        bool hasOutput = false;
        do
        {
            int instruction = Program[intPointer];
            int operand = Program[intPointer + 1];
            switch (instruction)
            {
                case 0:
                    var den = EvalCombo(operand);
                    RegA = Convert.ToInt64(Math.Truncate(Convert.ToDouble(RegA) / Math.Pow(2, den)));
                    break;
                case 1:
                    RegB ^= operand;
                    break;
                case 2:
                    RegB = EvalCombo(operand) % 8;
                    break;
                case 3:
                    if (RegA == 0)
                        break;
                    intPointer = operand;
                    continue;
                case 4:
                    RegB ^= RegC;
                    break;
                case 5:
                    if (hasOutput)
                        tempOutput.Append(',');
                    hasOutput = true;
                    tempOutput.Append($"{EvalCombo(operand) % 8}");
                    break;
                case 6:
                    den = EvalCombo(operand);
                    RegB = Convert.ToInt64(Math.Truncate(Convert.ToDouble(RegA) / Math.Pow(2, den)));
                    break;
                case 7:
                    den = EvalCombo(operand);
                    RegC = Convert.ToInt64(Math.Truncate(Convert.ToDouble(RegA) / Math.Pow(2, den)));
                    break;
            }
            intPointer += 2;
        }
        while (intPointer <= Program.Count - 2);
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {tempOutput}");
    }

    private long EvalCombo(int operand)
    {
        return operand switch
        {
            0 => 0,
            1 => 1,
            2 => 2,
            3 => 3,
            4 => RegA,
            5 => RegB,
            6 => RegC,
            7 => throw new Exception("invalid Program"),
            _ => throw new Exception("invalid Program")
        };
    }

    bool SatisfiesDigit(long input, int digit)
    {
        checked
        {
            var octalDigit = (int)(input % 8);
            int shiftCount = 7 - octalDigit;
            for (int i = 0; i < shiftCount; i++)
            {
                input /= 2;
            }
            input ^= octalDigit;
            var result = input % 8;
            return digit == result;
        }
    }

    bool SatisfiesDigitPrevGeneration(long input, int generation, int digit)
    {
        checked
        {
            for (int i = 0; i < generation; i++)
            {
                input = Convert.ToInt64(Math.Truncate(input / 8.0));
            }
            return SatisfiesDigit(input, digit);
        }
    }

    List<long> FindInputs(int digit, long min, long max)
    {
        checked
        {
            var offsets = Enumerable.Range(0, (int)(max - min)).Where(offset => SatisfiesDigit(min + offset, digit));
            return offsets.Select(offset => offset + min).ToList();
        }
    }

    public override ValueTask<string> Solve_2()
    {
        int targetPointer = Program.Count - 2;
        int generation = 1;

        List<long> validInputs = [7];

        while (targetPointer >= 0)
        {
            Console.WriteLine($"*************************************************************");
            Console.WriteLine($"Starting search. Generation {generation}. Target {Program[targetPointer]}. We have {validInputs.Count} valid inputs");
            var newValidInputs = new List<long>();
            for (int i = 0; i < validInputs.Count; i++)
            {
                var input = validInputs[i];

                var minSearch = input * 8;
                var maxSearch = minSearch + 7;
                Console.WriteLine($"Output digit needs to be {Program[targetPointer]}. (Solution / 8) needs to be  {input}. Output Search range: Min {minSearch} Max {maxSearch}");
                var possibleInputs = FindInputs(Program[targetPointer], minSearch, maxSearch);
                Console.WriteLine($"We found {possibleInputs.Count} value(s) to use for the input");


                // Assumption - If we get only 1 solution, it must be valid for the rest of the chain. TODO verify assumption
                if (possibleInputs.Count == 1)
                {
                    Console.WriteLine($"Adding {possibleInputs.First()} to the input list");
                    newValidInputs.Add(possibleInputs.First());
                }
                else
                {
                    Console.WriteLine("We have multiple inputs for the current generation. Checking the previous generations");
                    for (int genCheck = 1; genCheck < generation; genCheck++)
                    {
                        var prevGenChecks = possibleInputs.Where(input => SatisfiesDigitPrevGeneration(input, genCheck, Program[targetPointer + genCheck]));
                        possibleInputs = prevGenChecks.ToList();
                    }
                    if (possibleInputs.Any())
                    {
                        Console.WriteLine($"We have {possibleInputs.Count()} that will satisfy current and all previous generations");
                        newValidInputs.AddRange(possibleInputs);
                    }
                }
            }
            validInputs = newValidInputs;
            generation++;
            targetPointer--;

        }

        Console.WriteLine($"We have {validInputs.Count} options for the final answer");

        Console.Write($"\t{string.Join("\n\t", validInputs)}");
        var output = validInputs.Order().First();

        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {output}");
    }

}