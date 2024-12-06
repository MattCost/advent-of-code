using System.Text;
using System.Text.RegularExpressions;

public class Day03 : BaseDay
{
    string _lines = string.Empty;

    public Day03()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        StringBuilder lines = new();
        while ((line = sr.ReadLine()) != null)
        {
            lines.Append(line);
        }
        _lines = lines.ToString();

    }

    public override ValueTask<string> Solve_1()
    {
        long output = 0;
        string pattern = @"mul\((\d+),(\d+)\)";
        
        Regex rg = new Regex(pattern);
        MatchCollection matches = Regex.Matches(_lines, pattern);
        foreach (Match match in matches)
        {
            if (match.Groups.Count == 3)
            {
                output += int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value);
            }
        }
        return new(output.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        long output = 0;
        string mulPattern = @"mul\((\d+),(\d+)\)";
        string doPattern = @"do\(\)";
        string dontPattern = @"don\'t\(\)";
        
        MatchCollection mulMatches = Regex.Matches(_lines, mulPattern);
        MatchCollection doMatches = Regex.Matches(_lines, doPattern);
        MatchCollection dontMatches = Regex.Matches(_lines, dontPattern);
        
        var controlList = new List<(int, bool)>();

        foreach(Match match in doMatches)
        {
            controlList.Add((match.Index, true));
        }

        foreach(Match match in dontMatches)
        {
            controlList.Add((match.Index, false));
        }

        Console.WriteLine($"Control list has {controlList.Count} entries");
        
        var sortedControlList  = controlList.OrderBy( x => x.Item1).ToList();

        int startIndex = 0;
        int endIndex = sortedControlList[0].Item1;
        bool enabled = true;


        for(int i=0 ; i<sortedControlList.Count ; i++)
        {
            endIndex = sortedControlList[i].Item1;
            var activeMatches = mulMatches.Where(match => match.Index >= startIndex && match.Index <= endIndex);
            if(enabled)
            {
                foreach(var match in activeMatches)
                {
                    if (match.Groups.Count == 3)
                    {
                        output += int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value);
                    }
                }
            }
            startIndex = endIndex;
            enabled = sortedControlList[i].Item2;
        }

        if(enabled)
        {
            var activeMatches = mulMatches.Where(match => match.Index >= startIndex);
            foreach(var match in activeMatches)
            {
                if(match.Groups.Count == 3)
                {
                    output += int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value);
                }
            }
        }
        return new(output.ToString());
    }

}
