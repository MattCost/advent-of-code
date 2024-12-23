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
        Dictionary<string, (int score, int buyer)> totalScores = new();
        for (int b = 0; b < _lines.Count; b++)
        {
            var line = _lines[b];
            var secretNumber = long.Parse(line);
            var deltas = new List<int>();
            int prevPrice = (int)secretNumber % 10;
            int price = 0;
            for (int i = 0; i < 2000; i++)
            {
                secretNumber = GenerateNextSecret(secretNumber);
                price = (int)(secretNumber % 10);
                deltas.Add(price - prevPrice);
                if (i >= 3)
                {
                    var key = $"{deltas[i - 3]},{deltas[i - 2]},{deltas[i - 1]},{deltas[i]}";
                    if (totalScores.TryGetValue(key, out var scoreEntry))
                    {
                        if (scoreEntry.buyer != b)
                        {
                            totalScores[key] = (scoreEntry.score + price, b);
                        }
                    }
                    else
                    {
                        totalScores[key] = (price, b);
                    }
                }
                prevPrice = price;
            }
        }

        var winningSequence = totalScores.OrderBy(x => x.Value).Last().Key;
        var winningScore = totalScores[winningSequence];
        Console.WriteLine($"Sequence {winningSequence} Score {winningScore}");
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2 {winningSequence}");
    }

}