using AdventOfCode.Base.Misc;

public class Day21 : BaseDay
{
    List<string> _lines = new();

    public Day21()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            _lines.Add(line);
        }
    }

    List<KeypadCommand> GenerateDoorRobotCommands(string code)
    {
        var output = GenerateDoorCommand('A', code[0]);
        output.Add(KeypadCommand.Activate);

        for (int i = 0; i < code.Length - 1; i++)
        {
            output.AddRange(GenerateDoorCommand(code[i], code[i + 1]));
            output.Add(KeypadCommand.Activate);
        }

        return output;
    }

    private List<KeypadCommand> GenerateDoorCommand(char v1, char v2)
    {
        var output = new List<KeypadCommand>();
        (int row, int col) start = FindDoorKeypadLocation(v1);
        (int row, int col) end = FindDoorKeypadLocation(v2);
        var rowDelta = end.row - start.row;
        var colDelta = end.col - start.col;

        // Left before Up, Down before Right, unless the gap is in the way
        bool leftBeforeUp = true;
        bool downBeforeRight = true;

        if (start.row == 4 && end.col == 1) leftBeforeUp = false;
        if (start.col == 1 && end.row == 4) downBeforeRight = false;

        if (leftBeforeUp)
        {
            if (colDelta < 0)
                output.AddRange(Enumerable.Range(1, -colDelta).Select(_ => KeypadCommand.Left).ToList());
        }

        if (rowDelta < 0)
            output.AddRange(Enumerable.Range(1, -rowDelta).Select(_ => KeypadCommand.Up).ToList());

        if (!leftBeforeUp)
        {
            if (colDelta < 0)
                output.AddRange(Enumerable.Range(1, -colDelta).Select(_ => KeypadCommand.Left).ToList());

        }

        if (downBeforeRight)
        {
            if (rowDelta > 0)
                output.AddRange(Enumerable.Range(1, rowDelta).Select(_ => KeypadCommand.Down).ToList());
        }

        if (colDelta > 0)
            output.AddRange(Enumerable.Range(1, colDelta).Select(_ => KeypadCommand.Right).ToList());


        if (!downBeforeRight)
        {
            if (rowDelta > 0)
                output.AddRange(Enumerable.Range(1, rowDelta).Select(_ => KeypadCommand.Down).ToList());
        }

        return output;
    }

    private List<KeypadCommand> GenerateKeypadRobotCommands(List<KeypadCommand> commands)
    {
        var output = GenerateKeypadRobotCommand(KeypadCommand.Activate, commands[0]);
        output.Add(KeypadCommand.Activate);

        for (int i = 0; i < commands.Count - 1; i++)
        {
            output.AddRange(GenerateKeypadRobotCommand(commands[i], commands[i + 1]));
            output.Add(KeypadCommand.Activate);
        }

        return output;

    }

    private List<KeypadCommand> GenerateKeypadRobotCommand(KeypadCommand c1, KeypadCommand c2)
    {
        var output = new List<KeypadCommand>();
        (int row, int col) start = FindRobotKeypadLocation(c1);
        (int row, int col) end = FindRobotKeypadLocation(c2);
        var rowDelta = end.row - start.row;
        var colDelta = end.col - start.col;


        // Left before Up, Down before Right, unless the gap is in the way
        bool leftArrowEnd = end.col == 1;
        bool leftArrowStart = start.col == 1;

        if (leftArrowEnd)
        {
            if (rowDelta > 0)
                output.AddRange(Enumerable.Range(1, rowDelta).Select(_ => KeypadCommand.Down).ToList());

            if (colDelta < 0)
                output.AddRange(Enumerable.Range(1, -colDelta).Select(_ => KeypadCommand.Left).ToList());
        }
        else if (leftArrowStart)
        {
            if (colDelta > 0)
                output.AddRange(Enumerable.Range(1, colDelta).Select(_ => KeypadCommand.Right).ToList());

            if (rowDelta < 0)
                output.AddRange(Enumerable.Range(1, -rowDelta).Select(_ => KeypadCommand.Up).ToList());

        }
        else
        {
            if (colDelta < 0)
                output.AddRange(Enumerable.Range(1, -colDelta).Select(_ => KeypadCommand.Left).ToList());
            if (rowDelta < 0)
                output.AddRange(Enumerable.Range(1, -rowDelta).Select(_ => KeypadCommand.Up).ToList());
            if (rowDelta > 0)
                output.AddRange(Enumerable.Range(1, rowDelta).Select(_ => KeypadCommand.Down).ToList());
            if (colDelta > 0)
                output.AddRange(Enumerable.Range(1, colDelta).Select(_ => KeypadCommand.Right).ToList());
        }

        return output;
    }

    private (int row, int col) FindRobotKeypadLocation(KeypadCommand c)
    {
        return c switch
        {
            KeypadCommand.Up => (1, 2),
            KeypadCommand.Activate => (1, 3),
            KeypadCommand.Left => (2, 1),
            KeypadCommand.Down => (2, 2),
            KeypadCommand.Right => (2, 3),
            _ => throw new Exception()
        };
    }

    private static (int row, int col) FindDoorKeypadLocation(char x)
    {
        return x switch
        {
            '7' => (1, 1),
            '8' => (1, 2),
            '9' => (1, 3),

            '4' => (2, 1),
            '5' => (2, 2),
            '6' => (2, 3),

            '1' => (3, 1),
            '2' => (3, 2),
            '3' => (3, 3),

            '0' => (4, 2),
            'A' => (4, 3),

            _ => throw new Exception()
        };
    }

    public override ValueTask<string> Solve_1()
    {
        long output = 0;
        checked
        {
            foreach (var line in _lines)
            {
                var doorRobotCommands = GenerateDoorRobotCommands(line);
                Console.WriteLine($"Code {line} Door Robot Commands");
                Console.WriteLine($"{string.Join(string.Empty, doorRobotCommands.Select(cmd => cmd.ToPrintString()))}");

                ValidateDoorRobotCommands(doorRobotCommands);

                List<KeypadCommand> Commands = doorRobotCommands;
                for (int i = 0; i < 2; i++)
                {
                    Commands = GenerateKeypadRobotCommands(Commands);
                    // Console.WriteLine($"Code {line} Keypad {i + 1} Commands");
                    // Console.WriteLine($"{string.Join(string.Empty, Commands.Select(cmd => cmd.ToPrintString()))}");
                    ValidateKeypadRobotCommands(Commands);
                }
                var code = int.Parse(line.Replace("A", string.Empty).Trim());
                long complexity = Commands.Count * code;
                Console.WriteLine($"Input {line} Code {code} Command Count {Commands.Count} Complexity {complexity}");
                // Console.WriteLine($"{line}: {string.Join(string.Empty, Commands.Select(c => c.ToPrintString()))}");
                output += complexity;

            }
        }
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1 {output}");
    }

    private void ValidateKeypadRobotCommands(List<KeypadCommand> commands)
    {
        int row = 1;
        int col = 3;

        for (int i = 0; i < commands.Count; i++)
        {
            switch (commands[i])
            {
                case KeypadCommand.Up:
                    row--;
                    break;
                case KeypadCommand.Down:
                    row++;
                    break;
                case KeypadCommand.Left:
                    col--;
                    break;
                case KeypadCommand.Right:
                    col++;
                    break;
            }
            if (row <= 0) throw new Exception("Went up off the grid");
            if (row > 2) throw new Exception("Went down off the grid");
            if (col <= 0) throw new Exception("Went left off the grid");
            if (col > 3) throw new Exception("Went right off the grid");
            if (row == 1 && col == 1) throw new Exception("Over the hole");
        }

    }

    private void ValidateDoorRobotCommands(List<KeypadCommand> doorRobotCommands)
    {
        int row = 4;
        int col = 3;
        for (int i = 0; i < doorRobotCommands.Count; i++)
        {
            switch (doorRobotCommands[i])
            {
                case KeypadCommand.Up:
                    row--;
                    break;
                case KeypadCommand.Down:
                    row++;
                    break;
                case KeypadCommand.Left:
                    col--;
                    break;
                case KeypadCommand.Right:
                    col++;
                    break;
            }
            if (row <= 0) throw new Exception("Went up off the grid");
            if (row > 4) throw new Exception("Went down off the grid");
            if (col <= 0) throw new Exception("Went left off the grid");
            if (col > 3) throw new Exception("Went right off the grid");
            if (row == 4 && col == 1) throw new Exception("Over the hole");
        }

    }

    public override ValueTask<string> Solve_2()
    {
        long output = 0;
        Dictionary<(KeypadCommand start, KeypadCommand end), List<KeypadCommand>> Expansion = GenerateExpansion();
        //needs to be Start,end, int
        Dictionary<(KeypadCommand start, KeypadCommand end), int> CommandCount = new();

        checked
        {
            foreach (var line in _lines)
            {
                var doorRobotCommands = GenerateDoorRobotCommands(line);
                Console.WriteLine($"Code {line} Door Robot Commands");
                Console.WriteLine($"{string.Join(string.Empty, doorRobotCommands.Select(cmd => cmd.ToPrintString()))}");

                var firstGenCommands = GenerateKeypadRobotCommands(doorRobotCommands);
                for (int i = 0; i < firstGenCommands.Count - 1; i++)
                {
                    var key = (firstGenCommands[i], firstGenCommands[i+1]);
                    if(!CommandCount.ContainsKey(key))
                    {
                        CommandCount[key]=1;
                    }
                    else
                    {
                        CommandCount[(firstGenCommands[i], firstGenCommands[i + 1])]++;
                    }
                }

                for (int i = 0; i < 25; i++)
                {
                    var currentCounts = CommandCount.ToArray();
                    CommandCount.Clear();
                    foreach (var ckvp in currentCounts)
                    {
                        var expandedCmd = Expansion[ckvp.Key];
                        for (int x = 0; x < expandedCmd.Count - 1; x++)
                        {
                            var cmd = (expandedCmd[x], expandedCmd[x+1]);
                            if(!CommandCount.ContainsKey(cmd))
                            {
                                CommandCount[cmd] = ckvp.Value;
                            }
                            else
                            {
                                CommandCount[cmd] += ckvp.Value;
                            }
                        }
                    }
                }
                 
                var totalCmdCount = CommandCount.Select(kvp => kvp.Value * Expansion[kvp.Key].Count).Sum();
                var code = int.Parse(line.Replace("A", string.Empty).Trim());
                long complexity = totalCmdCount * code;
                Console.WriteLine($"Input {line} Code {code} Command Count {totalCmdCount} Complexity {complexity}");
                // Console.WriteLine($"{line}: {string.Join(string.Empty, Commands.Select(c => c.ToPrintString()))}");
                output += complexity;

            }
        }
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2 {output}");

    }

    private Dictionary<(KeypadCommand start, KeypadCommand end), List<KeypadCommand>> GenerateExpansion()
    {
        return new Dictionary<(KeypadCommand start, KeypadCommand end), List<KeypadCommand>>
        {
            [(KeypadCommand.Activate, KeypadCommand.Left)] = GenerateKeypadRobotCommand(KeypadCommand.Activate, KeypadCommand.Left),
            [(KeypadCommand.Activate, KeypadCommand.Right)] = GenerateKeypadRobotCommand(KeypadCommand.Activate, KeypadCommand.Right),
            [(KeypadCommand.Activate, KeypadCommand.Up)] = GenerateKeypadRobotCommand(KeypadCommand.Activate, KeypadCommand.Up),
            [(KeypadCommand.Activate, KeypadCommand.Down)] = GenerateKeypadRobotCommand(KeypadCommand.Activate, KeypadCommand.Down),

            [(KeypadCommand.Up, KeypadCommand.Left)] = GenerateKeypadRobotCommand(KeypadCommand.Up, KeypadCommand.Left),
            [(KeypadCommand.Up, KeypadCommand.Right)] = GenerateKeypadRobotCommand(KeypadCommand.Up, KeypadCommand.Right),
            [(KeypadCommand.Up, KeypadCommand.Activate)] = GenerateKeypadRobotCommand(KeypadCommand.Up, KeypadCommand.Activate),
            [(KeypadCommand.Up, KeypadCommand.Down)] = GenerateKeypadRobotCommand(KeypadCommand.Up, KeypadCommand.Down),

            [(KeypadCommand.Down, KeypadCommand.Left)] = GenerateKeypadRobotCommand(KeypadCommand.Down, KeypadCommand.Left),
            [(KeypadCommand.Down, KeypadCommand.Right)] = GenerateKeypadRobotCommand(KeypadCommand.Down, KeypadCommand.Right),
            [(KeypadCommand.Down, KeypadCommand.Up)] = GenerateKeypadRobotCommand(KeypadCommand.Down, KeypadCommand.Up),
            [(KeypadCommand.Down, KeypadCommand.Activate)] = GenerateKeypadRobotCommand(KeypadCommand.Down, KeypadCommand.Activate),

            [(KeypadCommand.Left, KeypadCommand.Activate)] = GenerateKeypadRobotCommand(KeypadCommand.Left, KeypadCommand.Activate),
            [(KeypadCommand.Left, KeypadCommand.Right)] = GenerateKeypadRobotCommand(KeypadCommand.Left, KeypadCommand.Right),
            [(KeypadCommand.Left, KeypadCommand.Up)] = GenerateKeypadRobotCommand(KeypadCommand.Left, KeypadCommand.Up),
            [(KeypadCommand.Left, KeypadCommand.Down)] = GenerateKeypadRobotCommand(KeypadCommand.Left, KeypadCommand.Down),

            [(KeypadCommand.Right, KeypadCommand.Left)] = GenerateKeypadRobotCommand(KeypadCommand.Right, KeypadCommand.Left),
            [(KeypadCommand.Right, KeypadCommand.Activate)] = GenerateKeypadRobotCommand(KeypadCommand.Right, KeypadCommand.Activate),
            [(KeypadCommand.Right, KeypadCommand.Up)] = GenerateKeypadRobotCommand(KeypadCommand.Right, KeypadCommand.Up),
            [(KeypadCommand.Right, KeypadCommand.Down)] = GenerateKeypadRobotCommand(KeypadCommand.Right, KeypadCommand.Down),
        };

    }
}