public class Day07 : BaseDay
{
    List<List<long>> _equations = new();

    public Day07()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        int i = 0;
        while ((line = sr.ReadLine()) != null)
        {
            _equations.Add(new());
            var split = line.Split(':');
            _equations[i].Add(long.Parse(split[0]));
            _equations[i].AddRange(split[1].Trim().Split(' ').Select(x => long.Parse(x.Trim())));
            i++;
        }

    }

    public override ValueTask<string> Solve_1()
    {
        long output = 0;
        foreach (var equation in _equations)
        {
            if (IsSolvable(equation))
            {
                output += equation[0];
            }
        }
        return new(output.ToString());
    }



    private bool IsSolvable(List<long> equation)
    {
        // Console.WriteLine($"Working on equation {string.Join(' ', equation)}");
        var target = equation[0];
        // Console.WriteLine($"\tTarget is {target}");
        Queue<long> leftOperands = new();
        Queue<long> rightOperands = new();
        leftOperands.Enqueue(equation[1]);
        for (int i = 2; i < equation.Count; i++)
        {
            rightOperands.Enqueue(equation[i]);
        }

        // Console.WriteLine($"\t{leftOperands.Count} left side and {rightOperands.Count} right side");
        while (rightOperands.Count > 0)
        {
            var rightOperand = rightOperands.Dequeue();
            // Console.WriteLine($"\tRight Operand {rightOperand}");
            int count = leftOperands.Count;
            // Console.WriteLine($"\t{count} left operands");
            for (int i = 0; i < count; i++)
            {
                var leftOperand = leftOperands.Dequeue();
                // Console.WriteLine($"\t{i}-th left operand {leftOperand}");
                var add = leftOperand + rightOperand;
                if (add <= target)
                {
                    leftOperands.Enqueue(add);
                }

                var mul = leftOperand * rightOperand;
                if(mul <= target)
                {
                    leftOperands.Enqueue(mul);
                }

                var concat = long.Parse($"{leftOperand}{rightOperand}");
                if(concat <= target)
                {
                    leftOperands.Enqueue(concat);
                }
            }
        }

        while(leftOperands.Count > 0)
        {
            var result = leftOperands.Dequeue();
            if(result == target) return true;
        }

        return false;
    }

    public override ValueTask<string> Solve_2()
    {
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2");
    }

}

public enum Operand
{
    Add,
    Mul
}

