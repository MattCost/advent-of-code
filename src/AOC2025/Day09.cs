public class Day09 : BaseDay
{
    List<string> _lines = new();
    List<(long X, long Y)> points = new();
    public Day09()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            _lines.Add(line);
            var parts = line.Split(',');
            points.Add(new(long.Parse(parts[0]), long.Parse(parts[1])));
        }

    }

    public override ValueTask<string> Solve_1()
    {
        long output = 0;
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                var newArea = (1 + Math.Abs(points[i].X - points[j].X)) * (1 + Math.Abs(points[i].Y - points[j].Y));
                if (newArea > output)
                {
                    output = newArea;
                }
            }
        }
        return new($"{output}");
    }
    public override ValueTask<string> Solve_2()
    {
        long output = 0;

        return new($"{output}");
    }

}
//2147432496 too low