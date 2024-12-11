
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
        LinkedList<long> Stones = new();
        foreach (var stone in stones)
        {
            Stones.AddLast(stone);
        }
        PrintStones(Stones);
        for (int i = 0; i < 25; i++)
        {
            var stone = Stones.First;
            while (stone != null)
            {
                if (stone.Value == 0)
                {
                    stone.Value = 1;
                }
                else if (stone.Value.ToString().Length % 2 == 0)
                {
                    var prev = stone.Previous;
                    Stones.Remove(stone);
                    var len = stone.Value.ToString().Length;
                    var left = new LinkedListNode<long>(long.Parse(stone.Value.ToString().Substring(0, len / 2)));
                    var right = new LinkedListNode<long>(long.Parse(stone.Value.ToString().Substring(len / 2, len / 2)));
                    if (prev == null)
                    {
                        Stones.AddFirst(right);
                        Stones.AddFirst(left);
                    }
                    else
                    {
                        Stones.AddAfter(prev, right);
                        Stones.AddAfter(prev, left);
                    }
                    stone = right;
                }
                else
                {
                    stone.Value *= 2024;
                }
                stone = stone.Next;
            }
            // PrintStones();
        }
        return new($"{Stones.Count}");
    }

    private void PrintStones(LinkedList<long> stones)
    {
        foreach (var stone in stones)
        {
            Console.Write($"{stone} ");
        }
        Console.WriteLine();

    }

    public override ValueTask<string> Solve_2()
    {
        Console.WriteLine("Starting part 2");
        Console.WriteLine($"10 blink 1 {StoneCountRecursive(10, 1)}");
        Console.WriteLine($"10 blink 2 {StoneCountRecursive(10, 2)}");
        Console.WriteLine($"10 blink 3 {StoneCountRecursive(10, 3)}");
        Console.WriteLine($"10 blink 4 {StoneCountRecursive(10, 4)}");
        Console.WriteLine($"10 blink 5 {StoneCountRecursive(10, 5)}");
        var stones = line.Split(' ').Select(long.Parse);
        long output = 0;
        foreach (var stone in stones)
        {
            output += StoneCountRecursive(stone, 25);
        }
        return new($"{output}");
    }

    private long StoneCount(long stone, int blinkCount)
    {
        Console.WriteLine($"Working on Stone {stone}");
        var stones = new List<long> { stone };

        for (int i = 0; i < blinkCount; i++)
        {
            var newStones = new List<long>();
            for (int s = 0; s < stones.Count; s++)
            {
                if (stones[s] == 0)
                {
                    newStones.Add(1);
                }
                else if (stones[s].ToString().Length % 2 == 0)
                {
                    var len = stones[s].ToString().Length;
                    var left = long.Parse(stones[s].ToString().Substring(0, len / 2));
                    var right = long.Parse(stones[s].ToString().Substring(len / 2, len / 2));
                    newStones.Add(left);
                    newStones.Add(right);
                }
                else
                {
                    newStones.Add(stones[s] * 2024L);
                }
            }
            stones = newStones;
        }
        return stones.Count;
    }

    private long StoneCountRecursive(long stone, int blinkCount)
    {
        // if (blinkCount == 0) return 0; //doesn't get hit?
        var digitCount = $"{stone}".Length;

        if (digitCount == 1)
        {
            if (stone == 0)
            {
                // 0 -> 1 -> 2024
                if (blinkCount == 1)
                    return 1;
                else
                    return StoneCountRecursive(1, blinkCount - 1);
            }

            if (stone == 1)
            {
                // 1-> 2024 -> 20 24 -> 2 0 2 4
                if (blinkCount == 1) return 1; // 2024
                if (blinkCount == 2) return 2; // 20 24
                if (blinkCount == 3) return 4; // 2 0 2 4
                if (blinkCount > 3) return
                        StoneCountRecursive(2, blinkCount - 3) +
                        StoneCountRecursive(0, blinkCount - 3) +
                        StoneCountRecursive(2, blinkCount - 3) +
                        StoneCountRecursive(4, blinkCount - 3);
            }

            if (stone == 2)
            {
                //2 -> 4048 -> 40 48 -> 4 0 4 8
                if (blinkCount == 1) return 1; // 4048
                if (blinkCount == 2) return 2; // 40 48
                if (blinkCount == 3) return 4; // 4 0 4 8
                if (blinkCount > 3)
                    return StoneCountRecursive(4, blinkCount - 3) +
                        StoneCountRecursive(0, blinkCount - 3) +
                        StoneCountRecursive(4, blinkCount - 3) +
                        StoneCountRecursive(8, blinkCount - 3);
            }

            if (stone == 3)
            {
                //3 -> 6072 -> 60 72 -> 6 0 7 2
                if (blinkCount == 1) return 1; // 6072
                if (blinkCount == 2) return 2; // 60 72
                if (blinkCount == 3) return 4; // 6 0 7 2
                if (blinkCount > 3)
                    return StoneCountRecursive(6, blinkCount - 3) +
                        StoneCountRecursive(0, blinkCount - 3) +
                        StoneCountRecursive(7, blinkCount - 3) +
                        StoneCountRecursive(2, blinkCount - 3);
            }

            if (stone == 4)
            {
                //4 -> 8096 -> 80 96 -> 8 0 9 6
                if (blinkCount == 1) return 1;
                if (blinkCount == 2) return 2;
                if (blinkCount == 3) return 4;
                if (blinkCount > 3)
                    return StoneCountRecursive(8, blinkCount - 3) +
                        StoneCountRecursive(0, blinkCount - 3) +
                        StoneCountRecursive(9, blinkCount - 3) +
                        StoneCountRecursive(6, blinkCount - 3);
            }

            if (stone == 5)
            {                                  // 5
                if (blinkCount == 1) return 1; // 10120
                if (blinkCount == 2) return 1; // 20482880
                if (blinkCount == 3) return 2; // 2048 2880
                if (blinkCount == 4) return 4; // 20 48 28 80
                if (blinkCount == 5) return 8; // 2 0 4 8 2 8 8 0
                if (blinkCount > 5) return
                        StoneCountRecursive(2, blinkCount - 5) +
                        StoneCountRecursive(0, blinkCount - 5) +
                        StoneCountRecursive(4, blinkCount - 5) +
                        StoneCountRecursive(8, blinkCount - 5) +
                        StoneCountRecursive(2, blinkCount - 5) +
                        StoneCountRecursive(8, blinkCount - 5) +
                        StoneCountRecursive(8, blinkCount - 5) +
                        StoneCountRecursive(0, blinkCount - 5);
            }

            if (stone == 6)
            {
                // 6 -> 12144 -> 24579456 -> 2457 9456 -> 24 57 94 56
                if (blinkCount == 1) return 1; // 12144
                if (blinkCount == 2) return 1; // 24579456
                if (blinkCount == 3) return 2; // 2457 9456
                if (blinkCount == 4) return 4; // 20 48 28 80
                if (blinkCount == 5) return 8; // 2 0 4 8 2 8 8 0
                if (blinkCount > 5) return
                        StoneCountRecursive(2, blinkCount - 5) +
                        StoneCountRecursive(4, blinkCount - 5) +
                        StoneCountRecursive(5, blinkCount - 5) +
                        StoneCountRecursive(7, blinkCount - 5) +
                        StoneCountRecursive(9, blinkCount - 5) +
                        StoneCountRecursive(4, blinkCount - 5) +
                        StoneCountRecursive(5, blinkCount - 5) +
                        StoneCountRecursive(6, blinkCount - 5);
            }

            if (stone == 7)
            {
                // 7 -> 14168 -> 28676032 -> 2867 6032 -> 28 67 60 32 -> 2 8 6 7 6 0 3 2
                if (blinkCount == 1) return 1; // 14168
                if (blinkCount == 2) return 1; // 28676032
                if (blinkCount == 3) return 2; // 2867 6032
                if (blinkCount == 4) return 4; // 28 67 60 32
                if (blinkCount == 5) return 8; // 2 8 6 7 6 0 3 2
                if (blinkCount > 5) return
                        StoneCountRecursive(2, blinkCount - 5) +
                        StoneCountRecursive(8, blinkCount - 5) +
                        StoneCountRecursive(6, blinkCount - 5) +
                        StoneCountRecursive(7, blinkCount - 5) +
                        StoneCountRecursive(6, blinkCount - 5) +
                        StoneCountRecursive(0, blinkCount - 5) +
                        StoneCountRecursive(3, blinkCount - 5) +
                        StoneCountRecursive(2, blinkCount - 5);
            }

            if (stone == 8)
            {
                // 8 -> 16192 -> 32772608 -> 3277 2608 -> 32 77 26 08 -> 3 2 7 7 2 6 0 8
                if (blinkCount == 1) return 1; // 16192
                if (blinkCount == 2) return 1; // 32772608
                if (blinkCount == 3) return 2; // 3277 2608
                if (blinkCount == 4) return 4; // 32 77 26 08
                if (blinkCount == 5) return 8; // 3 2 7 7 2 6 0 8
                if (blinkCount > 5) return
                        StoneCountRecursive(3, blinkCount - 5) +
                        StoneCountRecursive(2, blinkCount - 5) +
                        StoneCountRecursive(7, blinkCount - 5) +
                        StoneCountRecursive(7, blinkCount - 5) +
                        StoneCountRecursive(2, blinkCount - 5) +
                        StoneCountRecursive(6, blinkCount - 5) +
                        StoneCountRecursive(0, blinkCount - 5) +
                        StoneCountRecursive(8, blinkCount - 5);
            }

            if (stone == 9)
            {
                // 9 -> 18216 -> 36869184 -> 3686 9184 -> 36 86 91 84 -> 3 6 8 6 9 1 8 4
                if (blinkCount == 1) return 1; // 18216
                if (blinkCount == 2) return 1; // 36869184
                if (blinkCount == 3) return 2; // 3686 9184
                if (blinkCount == 4) return 4; // 36 86 91 84
                if (blinkCount == 5) return 8; // 3 6 8 6 9 1 8 4
                if (blinkCount > 5) return
                        StoneCountRecursive(3, blinkCount - 5) +
                        StoneCountRecursive(6, blinkCount - 5) +
                        StoneCountRecursive(8, blinkCount - 5) +
                        StoneCountRecursive(6, blinkCount - 5) +
                        StoneCountRecursive(9, blinkCount - 5) +
                        StoneCountRecursive(1, blinkCount - 5) +
                        StoneCountRecursive(8, blinkCount - 5) +
                        StoneCountRecursive(4, blinkCount - 5);
            }

            return 0; //placeholder for compiler
        }
        else if (digitCount % 2 == 0)
        {
            if (blinkCount == 1) return 2;
            var left = long.Parse(stone.ToString().Substring(0, digitCount / 2));
            var right = long.Parse(stone.ToString().Substring(digitCount / 2, digitCount / 2));
            return StoneCountRecursive(left, blinkCount - 1) + StoneCountRecursive(right, blinkCount - 1);
        }
        else
        {
            if (blinkCount == 1) return 1;
            return StoneCountRecursive(stone * 2024, blinkCount - 1);
        }
    }
}