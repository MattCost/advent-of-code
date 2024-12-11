

using System.Xml;

public class Day11 : BaseDay
{
    string line;
    public Day11()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        line = sr.ReadLine() ?? string.Empty;
    }

    public override ValueTask<string> Solve_1()
    {
        var stones = line.Split(' ').Select(long.Parse);
        return new($"{StoneCountIterative(stones, 25)}");
    }

    public override ValueTask<string> Solve_2()
    {
        var stones = line.Split(' ').Select(long.Parse);
        return new($"{StoneCountIterative(stones, 75)}");
    }

    private static long StoneCountIterative(IEnumerable<long> stones, int blinkCount)
    {
        var stoneTracker = new Dictionary<long, long>();
        foreach (var stone in stones)
        {
            if (!stoneTracker.ContainsKey(stone))
                stoneTracker[stone] = 0;

            stoneTracker[stone] += 1;
        }

        for (int i = 0; i < blinkCount; i++)
        {
            var newStoneTracker = new Dictionary<long, long>();
            foreach (var stoneValue in stoneTracker.Keys)
            {
                foreach (var newStone in ProcessStone(stoneValue))
                {
                    if (!newStoneTracker.ContainsKey(newStone))
                        newStoneTracker[newStone] = 0;

                    newStoneTracker[newStone] += stoneTracker[stoneValue];
                }
            }
            stoneTracker = newStoneTracker;
        }

        return stoneTracker.Values.Sum();
    }

    static Dictionary<long, IEnumerable<long>> ProcessStoneCache = new();
    static Day11()
    {
        ProcessStoneCache[0] = [1];
        ProcessStoneCache[1] = [1 * 2024];
        ProcessStoneCache[2] = [2 * 2024];
        ProcessStoneCache[3] = [3 * 2024];
        ProcessStoneCache[4] = [4 * 2024];
        ProcessStoneCache[5] = [5 * 2024];
        ProcessStoneCache[6] = [6 * 2024];
        ProcessStoneCache[7] = [7 * 2024];
        ProcessStoneCache[8] = [8 * 2024];
        ProcessStoneCache[9] = [9 * 2024];
    }
    private static IEnumerable<long> ProcessStone(long stone)
    {
        if (ProcessStoneCache.TryGetValue(stone,out var output))
        {
            return output;
        }
        
        var stringVer = stone.ToString();
        if (stringVer.Length % 2 == 0)
        {
            var left = long.Parse($"{stringVer.Substring(0, stringVer.Length / 2)}");
            var right = long.Parse($"{stringVer.Substring(stringVer.Length / 2, stringVer.Length / 2)}");
            ProcessStoneCache[stone] = [left, right];
            return [left, right];
        }
        else
        {
            ProcessStoneCache[stone] = [stone * 2024L];

            return [stone * 2024L];
        }

    }
}