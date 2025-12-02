using System.Collections;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

public class Day02 : BaseDay
{
    private class ProductIdRange
    {
        public ProductIdRange(string input)
        {
            var ranges = input.Split('-');
            if (ranges.Length != 2)
            {
                throw new ArgumentOutOfRangeException(nameof(input));
            }
            Start = long.Parse(ranges[0]);
            End = long.Parse(ranges[1]);
        }
        public long Start { get; set; }
        public long End { get; set; }
    }

    List<ProductIdRange> ranges = new();

    public Day02()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            var rangeInputs = line.Split(',');
            foreach (var rangeInput in rangeInputs)
                ranges.Add(new ProductIdRange(rangeInput));
        }
    }

    public override ValueTask<string> Solve_1()
    {
        long output = 0;
        foreach (var range in ranges)
        {
            for (var id = range.Start; id <= range.End; id++)
            {
                if (IsInvalidId(id.ToString()))
                {
                    output += id;
                }
            }
        }

        return new($"{output}");

    }

    private static bool IsInvalidId(string id)
    {
        if (id.Length % 2 == 1) return false;    //Only odd lengths can be invalid

        // Console.WriteLine($"\tChecking Id {id}");
        var half = id.Length / 2;
        for (int i = 0; i < half; i++)
        {
            if (id[i] != id[i + half]) return false;
        }

        return true;
    }



    public override ValueTask<string> Solve_2()
    {
        long output = 0;
        foreach (var range in ranges)
        {
            for (var id = range.Start; id <= range.End; id++)
            {
                if (IsInvalidIdPart2(id.ToString()))
                {
                    output += id;
                }
            }
        }

        return new($"{output}");

    }

    private static bool IsInvalidIdPart2(string id)
    {
        // Console.WriteLine($"\tChecking Id {id}");
        var maxPatternSize = id.Length / 2;
        for (int i = 1; i <= maxPatternSize; i++)
        {
            var pattern = id[0..i];
            var count = CountOccurrenceOf(id, pattern);
            if (count * pattern.Length == id.Length)
            {
                // Console.WriteLine($"\tId {id} - Pattern {pattern} - Count {count}");
                return true;
            }
        }

        return false;
    }

    private static int CountOccurrenceOf(string id, string pattern)
    {
        int count = 0;
        int indexOf = id.IndexOf(pattern);
        while (indexOf != -1 && indexOf < id.Length)
        {
            count++;
            indexOf = id.IndexOf(pattern, indexOf + pattern.Length);
        }

        return count;
    }
}

// 4174379265