
public class Day11 : BaseDay
{
    string line;
    int blinkCount = 25;
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
        for (int i = 0; i < blinkCount; i++)
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
            output += StoneCountRecursive(stone, blinkCount);
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
        var digitCount = $"{stone}".Length;

        if (digitCount == 1)
        {

            switch (stone)
            {
                case 0:
                    return blinkCount switch
                    {
                        1 => 1,
                        _ => StoneCountRecursive(1, blinkCount - 1)
                    };

                case 1:
                    return blinkCount switch
                    {
                        1 => 1,
                        2 => 2,
                        3 => 4,
                        _ =>
                            StoneCountRecursive(2, blinkCount - 3) +
                            StoneCountRecursive(0, blinkCount - 3) +
                            StoneCountRecursive(2, blinkCount - 3) +
                            StoneCountRecursive(4, blinkCount - 3)
                    };

                case 2:
                    return blinkCount switch
                    {
                        1 => 1,
                        2 => 2,
                        3 => 4,
                        _ =>
                            StoneCountRecursive(4, blinkCount - 3) +
                            StoneCountRecursive(0, blinkCount - 3) +
                            StoneCountRecursive(4, blinkCount - 3) +
                            StoneCountRecursive(8, blinkCount - 3)
                    };

                case 3:
                    return blinkCount switch
                    {
                        1 => 1,
                        2 => 2,
                        3 => 4,
                        _ =>
                            StoneCountRecursive(6, blinkCount - 3) +
                            StoneCountRecursive(0, blinkCount - 3) +
                            StoneCountRecursive(7, blinkCount - 3) +
                            StoneCountRecursive(2, blinkCount - 3)
                    };

                case 4:
                    return blinkCount switch
                    {
                        1 => 1,
                        2 => 2,
                        3 => 4,
                        _ =>
                            StoneCountRecursive(8, blinkCount - 3) +
                            StoneCountRecursive(0, blinkCount - 3) +
                            StoneCountRecursive(9, blinkCount - 3) +
                            StoneCountRecursive(6, blinkCount - 3)
                    };

                case 5:
                    return blinkCount switch
                    {
                        1 => 1,
                        2 => 1,
                        3 => 2,
                        4 => 4,
                        5 => 8,
                        _ =>
                            StoneCountRecursive(2, blinkCount - 5) +
                            StoneCountRecursive(0, blinkCount - 5) +
                            StoneCountRecursive(4, blinkCount - 5) +
                            StoneCountRecursive(8, blinkCount - 5) +
                            StoneCountRecursive(2, blinkCount - 5) +
                            StoneCountRecursive(8, blinkCount - 5) +
                            StoneCountRecursive(8, blinkCount - 5) +
                            StoneCountRecursive(0, blinkCount - 5)
                    };
                case 6:
                    return blinkCount switch
                    {
                        1 => 1,
                        2 => 1,
                        3 => 2,
                        4 => 4,
                        5 => 8,
                        _ =>
                            StoneCountRecursive(2, blinkCount - 5) +
                            StoneCountRecursive(4, blinkCount - 5) +
                            StoneCountRecursive(5, blinkCount - 5) +
                            StoneCountRecursive(7, blinkCount - 5) +
                            StoneCountRecursive(9, blinkCount - 5) +
                            StoneCountRecursive(4, blinkCount - 5) +
                            StoneCountRecursive(5, blinkCount - 5) +
                            StoneCountRecursive(6, blinkCount - 5)
                    };         
                case 7:
                    return blinkCount switch
                    {
                        1 => 1,
                        2 => 1,
                        3 => 2,
                        4 => 4,
                        5 => 8,
                        _ =>
                            StoneCountRecursive(2, blinkCount - 5) +
                            StoneCountRecursive(8, blinkCount - 5) +
                            StoneCountRecursive(6, blinkCount - 5) +
                            StoneCountRecursive(7, blinkCount - 5) +
                            StoneCountRecursive(6, blinkCount - 5) +
                            StoneCountRecursive(0, blinkCount - 5) +
                            StoneCountRecursive(3, blinkCount - 5) +
                            StoneCountRecursive(2, blinkCount - 5)
                    };                                
                case 8:
                    return blinkCount switch
                    {
                        1 => 1,
                        2 => 1,
                        3 => 2,
                        4 => 4,
                        5 => 7,
                        _ =>
                            StoneCountRecursive(3, blinkCount - 5) +
                            StoneCountRecursive(2, blinkCount - 5) +
                            StoneCountRecursive(7, blinkCount - 5) +
                            StoneCountRecursive(7, blinkCount - 5) +
                            StoneCountRecursive(2, blinkCount - 5) +
                            StoneCountRecursive(6, blinkCount - 5) +
                            StoneCountRecursive(8*2024, blinkCount - 5)
                    };                
                case 9:
                    return blinkCount switch
                    {
                        1 => 1,
                        2 => 1,
                        3 => 2,
                        4 => 4,
                        5 => 8,
                        _ =>
                            StoneCountRecursive(3, blinkCount - 5) +
                            StoneCountRecursive(6, blinkCount - 5) +
                            StoneCountRecursive(8, blinkCount - 5) +
                            StoneCountRecursive(6, blinkCount - 5) +
                            StoneCountRecursive(9, blinkCount - 5) +
                            StoneCountRecursive(1, blinkCount - 5) +
                            StoneCountRecursive(8, blinkCount - 5) +
                            StoneCountRecursive(4, blinkCount - 5)
                    };   
                    
                default:
                    throw new Exception("missed a case");
            }
        }
        else if (digitCount % 2 == 0)
        {
            if (blinkCount == 1)
                return 2;

            var left = long.Parse(stone.ToString().Substring(0, digitCount / 2));
            var right = long.Parse(stone.ToString().Substring(digitCount / 2, digitCount / 2));
            return StoneCountRecursive(left, blinkCount - 1) + StoneCountRecursive(right, blinkCount - 1);
        }
        else
        {
            if (blinkCount == 1)
                return 1;
            return StoneCountRecursive(stone * 2024, blinkCount - 1);
        }
    }
}