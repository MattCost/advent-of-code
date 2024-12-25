public class Day25 : BaseDay
{
    List<List<int>> _locks = new();
    List<List<int>> _keys = new();

    public Day25()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            var entryCount = new List<int> { 0, 0, 0, 0, 0 };
            var isLock = line == "#####"; 

            while (!string.IsNullOrEmpty(line = sr.ReadLine()))
            {
                for (int i = 0; i < line.Length; i++)
                    if (line[i] == '#') 
                        entryCount[i]++;
            }

            if (isLock)
            {
                var check = entryCount[0] * 10000 + entryCount[1] * 1000 + entryCount[2] * 100 + entryCount[3] * 10 + entryCount[4];
                _locks.Add(entryCount);
            }
            else
            {
                // Remove the wrong count from the bottom row  of ##### in the lock
                for (int i = 0; i < entryCount.Count; i++)
                    entryCount[i]--;
                
                var check = entryCount[0] * 10000 + entryCount[1] * 1000 + entryCount[2] * 100 + entryCount[3] * 10 + entryCount[4];
                _keys.Add(entryCount);
            }
        }

        Console.WriteLine($"There are {_locks.Count} locks and {_keys.Count} keys to try");
    }

    public override ValueTask<string> Solve_1()
    {
        int validCombos = 0;
        for (int l = 0; l < _locks.Count; l++)
        {
            for (int k = 0; k < _keys.Count; k++)
            {
                bool valid = true;
                for (int x = 0; x < 5; x++)
                {
                    if (_locks[l][x] + _keys[k][x] > 5)
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    // Console.WriteLine($"Lock {string.Join(",", _locks[l])} and key {string.Join(",", _keys[k])} fit");
                    validCombos++;
                } 

            }
        }
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1 {validCombos}");
    }

    public override ValueTask<string> Solve_2()
    {
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2");
    }

}