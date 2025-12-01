using System.Collections.Concurrent;

public class Day19 : BaseDay
{
    List<string> AvailableTowels = new();
    List<string> RequestedPatterns = new();

    public Day19()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line = sr.ReadLine();
        AvailableTowels = line!.Split(',').Select(x => x.Trim()).ToList();
        line = sr.ReadLine();//blank

        while ((line = sr.ReadLine()) != null)
        {
            RequestedPatterns.Add(line);
        }

        // Console.WriteLine($"We have {AvailableTowels.Count} different towels available, and {RequestedPatterns.Count} patterns to create");
    }

    public override ValueTask<string> Solve_1()
    {
        var output = RequestedPatterns.Where(pattern => IsPatternPossible(pattern, AvailableTowels)).Count();
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {output}");
    }

    private bool IsPatternPossible(string pattern, List<string> availableTowels)
    {
        var matches = availableTowels.Where(towel => pattern.StartsWith(towel));
        if (!matches.Any())
        {
            return false;
        }
        foreach (var match in matches)
        {
            var newPattern = pattern.Remove(0, match.Length);

            if (string.IsNullOrEmpty(newPattern) || IsPatternPossible(newPattern, availableTowels))
                return true;
        }
        return false;
    }


    ConcurrentDictionary<string, long> _cache = new();
    private long PatternPossibleCounts(string pattern, List<string> availableTowels)
    {
        
        if (_cache.TryGetValue(pattern, out var count))
        {
            return count;
        }

        var matches = availableTowels.Where(towel => pattern.StartsWith(towel));
        long output = 0;
        foreach (var match in matches)
        {
            var newPattern = pattern.Remove(0, match.Length);
            if (string.IsNullOrEmpty(newPattern))
            {
                output++;
            }
            else
            {
                output += PatternPossibleCounts(newPattern, availableTowels);
            }
        }
        _cache[pattern] = output;
        return output;
    }
    public override ValueTask<string> Solve_2()
    {
        var validPatterns = RequestedPatterns.Where(pattern => IsPatternPossible(pattern, AvailableTowels));
        long output = 0;
        Parallel.ForEach( validPatterns, pattern => Interlocked.Add(ref output,PatternPossibleCounts(pattern, AvailableTowels)));
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {output}");
    }

}
