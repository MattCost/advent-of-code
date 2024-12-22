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
        List<List<int>> allPrices = new();
        foreach (var line in _lines)
        {
            var secretNumber = long.Parse(line);
            List<int> prices = [(int)(secretNumber % 10)];
            Console.Write($"{secretNumber}: ");
            for (int i = 0; i < 2000; i++)
            {
                secretNumber = GenerateNextSecret(secretNumber);
                prices.Add((int)(secretNumber % 10));
            }
            Console.WriteLine($"{secretNumber}");
            allPrices.Add(prices);
        }
        List<List<int>> allDeltas = new();
        List<Dictionary<string, (int price, int index)>> trackers = new();
        for (int i = 0; i < allPrices.Count; i++)
        {
            trackers.Add(new());
            var prices = allPrices[i];
            var deltas = new List<int>();
            for (int j = 0; j < prices.Count - 1; j++)
            {
                deltas.Add(prices[j + 1] - prices[j]);
                if (j >= 3)
                {
                    var key = $"{deltas[j - 3]},{deltas[j - 2]},{deltas[j - 1]},{deltas[j]}";
                    if (!trackers[i].ContainsKey(key))
                    {
                        trackers[i][key] = (prices[j + 1], j + 1);
                    }
                }
            }

            allDeltas.Add(deltas);
        }

        Console.WriteLine("Debug");

        string winningSequence = string.Empty;
        int bestPrice = 0;
        var allSequences = trackers.SelectMany(tracker => tracker.Keys.ToList()).Distinct();
        foreach (var sequence in allSequences)
        {
            var score = 0;
            foreach(var tracker in trackers)
            {
                if(tracker.TryGetValue(sequence, out var entry)) score += entry.price;
            }
            if(score > bestPrice)
            {
                bestPrice = score;
                winningSequence = sequence;
            }
        }

        /*
            Search all the lists of deltas for a pattern that indicates the highest price.
            Find a sequence that predicts 9.
            Find a sequence that predicts 8.
            Find a sequence that predicts 7.
            etc.
            Dictionary<(sequence), List<(index - price)>
            find ind

        */



        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2 {winningSequence}");
    }

}