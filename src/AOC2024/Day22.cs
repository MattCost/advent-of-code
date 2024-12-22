public class Day22 : BaseDay
{
    List<string> _lines = new();

    public Day22()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            _lines.Add(line);
        }

    }

    long GenerateNextSecret(long secretNumber)
    {
        var part1 = secretNumber * 64;
        var part2 = secretNumber ^ part1;
        secretNumber = part2 % 16777216;
        part1 = Convert.ToInt64(Math.Truncate(Convert.ToDouble(secretNumber) / Convert.ToDouble(32)));
        part2 = secretNumber ^ part1;
        secretNumber = part2 % 16777216;
        secretNumber = (secretNumber ^ (secretNumber * 2048)) % 16777216;

        return secretNumber;
    }

    public override ValueTask<string> Solve_1()
    {
        long secretNumber = 123;
        for (int i = 0; i < 10; i++)
        {
            secretNumber = GenerateNextSecret(secretNumber);
            Console.WriteLine(secretNumber);
        }
        long output = 0;
        foreach (var line in _lines)
        {
            secretNumber = long.Parse(line);
            Console.Write($"{secretNumber}: ");
            for (int i = 0; i < 2000; i++)
            {
                secretNumber = GenerateNextSecret(secretNumber);
            }
            Console.WriteLine($"{secretNumber}");
            output += secretNumber;
        }
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1 {output}");
    }

    public override ValueTask<string> Solve_2()
    {
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2");
    }

}