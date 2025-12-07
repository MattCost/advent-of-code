using Spectre.Console;

public class Day07 : BaseDay
{
    List<string> _lines = new();

    public Day07()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            _lines.Add(line);
        }

    }

    public override ValueTask<string> Solve_1()
    {
        long output = 0;
        var startingIndex = _lines[0].IndexOf('S');
        var length = _lines[0].Length;

        // Created a working copy for rendering but was too lazy to code it
        // var workingCopy = new char[_lines.Count, length];        
        // workingCopy[1, startingIndex] = '|';

        var containsBeam = new bool[length];
        containsBeam[startingIndex] = true;
        for (int i = 2; i < _lines.Count; i++)
        {
            var newContainsBeam = new bool[length];
            for (int j = 0; j < length; j++)
            {
                if (_lines[i][j] == '.')
                {
                    if (containsBeam[j])
                    {
                        newContainsBeam[j] = true;
                    }
                }
                else if (_lines[i][j] == '^')
                {
                    if (containsBeam[j])
                    {
                        output++;
                        if ((j - 1) >= 0)
                            newContainsBeam[j - 1] = true;
                        if ((j + 1) < length)
                            newContainsBeam[j + 1] = true;
                    }
                }
                else
                {
                    throw new Exception("invalid input");
                }
            }
            containsBeam = newContainsBeam;
        }

        return new($"{output}");
    }

    public override ValueTask<string> Solve_2()
    {
        var startingIndex = _lines[0].IndexOf('S');
        var length = _lines[0].Length;

        var containsBeam = new long[length];
        containsBeam[startingIndex] = 1;
        for (int i = 2; i < _lines.Count; i++)
        {
            var newContainsBeam = new long[length];
            for (int j = 0; j < length; j++)
            {
                if (_lines[i][j] == '.')
                {
                    newContainsBeam[j] += containsBeam[j];
                }
                else if (_lines[i][j] == '^')
                {
                    if ((j - 1) >= 0)
                        newContainsBeam[j - 1] += containsBeam[j];
                    if ((j + 1) < length)
                        newContainsBeam[j + 1] += containsBeam[j];
                }
                else
                {
                    throw new Exception("invalid input");
                }
            }
            containsBeam = newContainsBeam;
        }

        long output = containsBeam.Sum();

        return new($"{output}");
    }

}