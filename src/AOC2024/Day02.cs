
public class Day02 : BaseDay
{
    List<string> _reports = new();

    public Day02()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            _reports.Add(line);
        }
    }

    public override ValueTask<string> Solve_1()
    {
        var safeCount = 0;
        foreach (var report in _reports)
        {
            var entries = report.Split(' ').Select(int.Parse).ToList();
            if (IsSafe(entries))
                safeCount++;
        }

        return new(safeCount.ToString());
    }

    private bool IsSafe(List<int> entries)
    {
        var deltas = new List<int>();
        for (int i = 0; i < entries.Count - 1; i++)
        {
            deltas.Add(entries[i] - entries[i + 1]);
        }

        var flatDeltas = deltas.Where(d => d == 0);
        var positiveDeltas = deltas.Where(d => d > 0);
        var negativeDeltas = deltas.Where(d => d < 0);

        if (flatDeltas.Any()) return false;

        if (positiveDeltas.Count() == negativeDeltas.Count()) return false;

        if (positiveDeltas.Any() && negativeDeltas.Any()) return false;

        if (deltas.Where(d => Math.Abs(d) > 3).Any()) return false;

        return true;
    }

    private bool IsSafeRemoveOne(List<int> entries)
    {
        var deltas = new List<int>();
        for (int i = 0; i < entries.Count - 1; i++)
        {
            deltas.Add(entries[i+1] - entries[i ]);
        }

        var positiveDeltas = deltas.Where(d => d > 0);
        var negativeDeltas = deltas.Where(d => d < 0);

        if (positiveDeltas.Count() == negativeDeltas.Count()) 
        {
            // Console.WriteLine($"Invalid due to flat trend. {string.Join(' ', entries)}");
            return false;
        }


        // Find the anomaly
        var positiveSlope = positiveDeltas.Count() > negativeDeltas.Count();

        int anomalyIndex;
        var anomalies = positiveSlope ? deltas.Where(d => d < 1 || d > 3) : deltas.Where(d => d < -3 || d > -1);
        switch (anomalies.Count())
        {
            case 0: return true;
            case 1: anomalyIndex = deltas.IndexOf(anomalies.First()); break;
            case 2: 
                var firstAnomaly = deltas.IndexOf(anomalies.First());
                var secondAnomaly = deltas.IndexOf(anomalies.Last());
                if(secondAnomaly == firstAnomaly+1)
                {
                    anomalyIndex = firstAnomaly;
                    break;
                }
                // Console.WriteLine($"Invalid due to multiple anomalies. {string.Join(' ', entries)} --- {string.Join(' ', anomalies)}");
                return false;

            default: 
                // Console.WriteLine($"Invalid due to multiple anomalies. {string.Join(' ', entries)} --- {string.Join(' ', anomalies)}");
                return false;
        }

        var test1 = entries.ToList();
        test1.RemoveAt(anomalyIndex);
        var test2 = entries.ToList();
        test2.RemoveAt(anomalyIndex + 1);

        var isSafe1 = IsSafe(test1);
        var isSafe2 = IsSafe(test2);

        return isSafe1 || isSafe2;
        // if(isSafe1 || isSafe2) return true;
        
        // Console.WriteLine($"Trimming: {string.Join(' ', entries)}");
        // if(!isSafe1)
        // {
        //     Console.WriteLine($"\tRemoved at {anomalyIndex} but still invalid. {string.Join(' ', test1)}");
        // }

        // if(!isSafe2)
        // {
        //     Console.WriteLine($"\tRemoved at {anomalyIndex + 1} but still invalid. {string.Join(' ', test2)}");
        // }

        // return false;
    }

    public override ValueTask<string> Solve_2()
    {
        var safeCount = 0;
        foreach (var report in _reports)
        {
            var entries = report.Split(' ').Select(int.Parse).ToList();
            if (IsSafeRemoveOne(entries))
                safeCount++;
        }

        return new(safeCount.ToString());
    }
}