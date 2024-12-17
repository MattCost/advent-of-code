using System.Collections;
using System.Text;

public class Day17 : BaseDay
{
    List<string> _lines = new();
    long RegA, RegB, RegC;
    long RegAinit, RegBinit, RegCinit;
    List<int> Program;

    public Day17()
    {
        StreamReader sr = new StreamReader(InputFilePath);
        // Console.WriteLine("Test Reg B should be 1");
        // RegC = 9;
        // Program = [2, 6];
        // Solve_1();
        // Console.WriteLine($"RegB {RegB}");

        // Console.WriteLine("Test Should output 0,1,2");
        // RegA = 10;
        // Program = [5, 0, 5, 1, 5, 4];
        // Solve_1();

        // Console.WriteLine("Test Should output 4,2,5,6,7,7,7,7,3,1,0 and A should be 0");
        // RegA = 2024;
        // Program = [0, 1, 5, 4, 3, 0];
        // Solve_1();
        // Console.WriteLine($"RegA {RegA}");

        // Console.WriteLine("Test Reg B should be 26");
        // RegB = 29;
        // Program = [1, 7];
        // Solve_1();
        // Console.WriteLine($"RegB {RegB}");


        // Console.WriteLine("Test complete");

        string? line;
        RegA = long.Parse((line = sr.ReadLine())!.Split(':')[1].Trim());
        RegB = long.Parse((line = sr.ReadLine())!.Split(':')[1].Trim());
        RegC = long.Parse((line = sr.ReadLine())!.Split(':')[1].Trim());
        RegAinit = RegA;
        RegBinit = RegB;
        RegCinit = RegC;
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
    public override ValueTask<string> Solve_2()
    {
        // long regA = 17104405500; //1031612834719; 

        var target = string.Join(',', Program);
        long testA = 6173100802971L; //5207806903195L;32 //1293445854109L;
        string output;
        do
        {
            var _regA = testA;
            StringBuilder tempOutput = new();
            do
            {
                var a1 = _regA % 8;
                var outputDigit = a1 ^ Convert.ToInt64(Math.Truncate(_regA / Math.Pow(2, a1 ^ 7))) % 8;
                tempOutput.Append(outputDigit);
                tempOutput.Append(',');
                _regA = Convert.ToInt64(Math.Truncate(_regA / 8.0));
            }
            while (_regA > 0);
            tempOutput.Remove(tempOutput.Length-1,1);
            output = tempOutput.ToString();

            if(output == target)
            {
                Console.WriteLine($"WINNER WINNER {_regA} OUTPUT: {output}");
                break;
            }
            if(output.Length > target.Length)
            {
                Console.WriteLine($"TOO FAR SO SAD :(\nRegA was {testA} OUTPUT: {output}");
                break;
            }
            // Console.WriteLine($"A {_regA} Output: {output}");

            if(output.StartsWith("2,4,1,7,7,5,1,7,0"))
            {
                Console.WriteLine($"RegA was {testA}");
                Console.WriteLine($"Target: {target}");
                Console.WriteLine($"Output: {output}");
                testA += 4095L * 4095L * 32;
            }
            else
            {
                testA++;
            }
        }
        while(output != target);


        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {output}");
    }

}