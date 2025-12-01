using System.Collections;

public class Day01 : BaseDay
{
    List<int> operations = new();

    public Day01()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            var isPositive = line[0] == 'R';
            var number = int.Parse($"{(isPositive ? string.Empty : "-")}{line[1..]}");
            operations.Add(number);
        }
    }

    public override ValueTask<string> Solve_1()
    {
        int zeroCount = 0;
        int position = 50;
        foreach(var operation in operations)
        {
            position += operation;
            position %= 100;
            if(position == 0)
                zeroCount++;

        }
        return new($"{zeroCount}");
    }

    public override ValueTask<string> Solve_2()
    {
        return new("todo");
    }

}