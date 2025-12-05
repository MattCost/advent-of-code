public class Day05 : BaseDay
{

    private class FreshRange
    {
        public long Start { get; set; }
        public long End { get; set; }
        public FreshRange(long start, long end)
        {
            Start = start;
            End = end;
        }
        public bool IsFresh(long ingredient)
        {
            return ingredient >= Start && ingredient <= End;
        }
    }
    List<FreshRange> FreshRanges = new();
    List<long> Ingredients = new();
    public Day05()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        bool processingRanges = true;
        while ((line = sr.ReadLine()) != null)
        {
            if (line == string.Empty)
            {
                processingRanges = false;
                continue;
            }
            if (processingRanges)
            {
                var range = line.Split('-');
                FreshRanges.Add(new FreshRange(long.Parse(range[0]), long.Parse(range[1])));
            }
            else
            {
                Ingredients.Add(long.Parse(line));
            }
        }
    }

    public override ValueTask<string> Solve_1()
    {
        long output = 0;
        foreach (var ingredient in Ingredients)
        {
            foreach (var range in FreshRanges)
            {
                if (range.IsFresh(ingredient))
                {
                    output++;
                    break;
                }
            }
        }

        return new($"{output}");
    }

    public override ValueTask<string> Solve_2()
    {
        long output = 0;
        
        // Sort the ranges by the start. Copy the ranges to a consolidated range list, merging any overlapping ranges into 1. If there is no overlap, the range gets added. If the ranges overlap, extend the end of the range in the consolidated list and up the index.

        var sorted = FreshRanges.OrderBy(range => range.Start).ToList();
        
        var combinedRanges = new List<FreshRange>
        {
            sorted[0]
        };

        int combinedIndex = 0;
        
        for (int i = 1; i < sorted.Count; i++)
        {
            if(combinedRanges[combinedIndex].End < sorted[i].Start)
            {
                combinedRanges.Add(sorted[i]);
                combinedIndex++;
            }
            else
            {
                combinedRanges[combinedIndex].End = Math.Max(combinedRanges[combinedIndex].End, sorted[i].End);
            }
        }
        foreach(var range in combinedRanges)
        {
            output += range.End - range.Start + 1;
        }
        return new($"{output}");
    }

}