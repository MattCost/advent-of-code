using System.Text;

public class Day06 : BaseDay
{
    List<string> _lines = new();

    public Day06()
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

        List<List<long>> ProblemInputs = new();

        bool firstRun = true;
        for (int j = 0; j < _lines.Count - 1; j++)
        {

            var inputRow = _lines[j].Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            // Console.WriteLine($"inputRow contains {inputRow.Count()} elements");
            for (int i = 0; i < inputRow.Length; i++)
            {
                if (firstRun)
                    ProblemInputs.Add(new List<long>());
                ProblemInputs[i].Add(long.Parse(inputRow[i]));
            }
            firstRun = false;
        }


        var operations = _lines[_lines.Count - 1].Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        for (int i = 0; i < operations.Length; i++)
        {
            switch (operations[i])
            {
                case "+":
                    long tempOutput = ProblemInputs[i].Sum();
                    // Console.WriteLine($"{string.Join($" {operations[i]} ", ProblemInputs[i])} = {tempOutput}");
                    output += tempOutput;
                    break;

                case "*":
                    tempOutput = ProblemInputs[i][0];
                    for (int j = 1; j < ProblemInputs[i].Count; j++)
                    {
                        tempOutput *= ProblemInputs[i][j];
                    }
                    // Console.WriteLine($"{string.Join($" {operations[i]} ", ProblemInputs[i])} = {tempOutput}");
                    output += tempOutput;
                    break;
            }
        }


        return new($"{output}");
    }

    public override ValueTask<string> Solve_2()
    {
        long output = 0;
        int length = _lines[0].Count();

        List<List<int>> ProblemInputs = new();

        int problemIndex=0;
        ProblemInputs.Add(new());
        for(int i=0 ; i<length;i++)
        {
            var numberInput = GetColumn(i, _lines[..^1]);
            if(!string.IsNullOrWhiteSpace(numberInput))
            {
                ProblemInputs[problemIndex].Add(int.Parse(numberInput));            
            }
            else
            {
                ProblemInputs.Add(new());
                problemIndex++;
                
            }
        }

        var operations = _lines[_lines.Count - 1].Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        for (int i = 0; i < operations.Length; i++)
        {
            switch (operations[i])
            {
                case "+":
                    long tempOutput = ProblemInputs[i].Sum();
                    // Console.WriteLine($"{string.Join($" {operations[i]} ", ProblemInputs[i])} = {tempOutput}");
                    output += tempOutput;
                    break;

                case "*":
                    tempOutput = ProblemInputs[i][0];
                    for (int j = 1; j < ProblemInputs[i].Count; j++)
                    {
                        tempOutput *= ProblemInputs[i][j];
                    }
                    // Console.WriteLine($"{string.Join($" {operations[i]} ", ProblemInputs[i])} = {tempOutput}");
                    output += tempOutput;
                    break;
            }
        }


        return new($"{output}");
    }

    private string GetColumn(int i, List<string> lines)
    {
        var output = new StringBuilder();
        for(int j=0 ; j<lines.Count() ; j++)
        {
            output.Append(lines[j][i]);
        }
        return output.ToString();
        
    }
}