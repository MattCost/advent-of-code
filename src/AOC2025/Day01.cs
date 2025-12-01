using System.Collections;

public class Day01 : BaseDay
{
    List<int> operations = new();

    public Day01()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            var isPositive = line[0] == 'R';
            var number = int.Parse($"{(isPositive ? string.Empty : "-")}{line[1..]}");
            operations.Add(number);
        }
    }

    public override ValueTask<string> Solve_1()
    {
        int zeroCount = 0;
        int position = 50;
        foreach (var operation in operations)
        {
            position += operation;
            position %= 100;
            if (position == 0)
                zeroCount++;

        }
        return new($"{zeroCount}");
    }

    public override ValueTask<string> Solve_2()
    {
        int zeroCount = 0;
        int position = 50;
        foreach (var operation in operations)
        {
            // Console.WriteLine($"At Position {position} - Zero Count is {zeroCount} - Operation is {operation}");
            // Moves of more than 100 must cross zero, so we can figure out how many wraps will happen, and then come up with an adjusted operation that is less than 100
            var wraps = Math.Abs(operation / 100);
            var adjustedOperation = operation % 100;
            // Console.WriteLine($"\tOperation has {wraps} wraps");

            // If we are starting from 0, this is a fudge to make sign change detection easier. If we end up below 0 we assume a sign change, but starting _at_ zero moving left ends up below 0 but there was no sign change. Fudge by adding 100 to avoid having to do it later
            if (position == 0 && adjustedOperation < 0)
            {
                // Console.WriteLine("\tStarting position is 0, adjusting to 100 to avoid false positive on sign change");
                position += 100;
            }

            // Actually preform the operation
            position += adjustedOperation;
            // Console.WriteLine($"\tResult of Operation {operation} lands at Position {position}");

            // If the position went negative, we crossed zero at least once, plus any Wraps
            if (position < 0)
            {
                position += 100;
                // Console.WriteLine($"\tOperation caused a sign change, adjusting position to {position} and increasing zeroCount by 1");
                zeroCount++;
                zeroCount += wraps;

            }
            // If the position went over 99, then we crossed zero at least once, plus any wraps
            else if (position > 99)
            {
                position -= 100;
                // Console.WriteLine($"\tOperation went over 99, adjusting position to {position} and increasing zeroCount by 1");
                zeroCount++;
                zeroCount += wraps;

            }
            // If we ended up at 0, by moving a multiple of 100, then we only need to add in the wraps, but if we ended up at 0 by moving a multiple of 100 plus we started somewhere else, then add in wraps + 1
            else if (position == 0)
            {
                if(adjustedOperation == 0)
                {
                    zeroCount += wraps;
                }
                else
                {
                    zeroCount += wraps+1;
                }
            }
            // Finally if we ended up on a non-zero number between 1 and 99, then only need to increase the zero count by the # of wraps
            else
            {
                zeroCount += wraps;
            }

            // Console.WriteLine($"Operation complete. zeroCount now {zeroCount}\n");

        }
        return new($"{zeroCount}");
    }

}