using System.Collections;

public class Day01 : BaseDay
{
    List<int> Left = new();
    List<int> Right = new();

    public Day01()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            var numbers = line.Split("   ");
            Left.Add(int.Parse(numbers[0]));
            Right.Add(int.Parse(numbers[1]));
        }
    }

    public override ValueTask<string> Solve_1()
    {
        var leftItr = Left.Order().GetEnumerator();
        var rightItr = Right.Order().GetEnumerator();
        long total = 0;
        do
        {
            total += Math.Abs(leftItr.Current - rightItr.Current);
        }
        while (leftItr.MoveNext() && rightItr.MoveNext());
        return new(total.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        Dictionary<int, int> _tracking = new();
        long total = 0;
        for (int i = 0; i < Left.Count; i++)
        {
            var j = Left[i];
            if (!_tracking.ContainsKey(j))  // this seems to make no speed diff, linq caching is nice
            {
                _tracking[j] = Right.Where(x => x == j).Count();
            }
            total += j * _tracking[j];
        }
        return new(total.ToString());
    }

}