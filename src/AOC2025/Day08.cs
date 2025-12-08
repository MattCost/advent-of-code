public class Day08 : BaseDay
{
    List<string> _lines = new();

    private class JunctionBox
    {
        public string Id { get; } = string.Empty;
        public int X { get; }
        public int Y { get; }
        public int Z { get; }
        public JunctionBox(string input)
        {
            Id = input;
            X = int.Parse(Id.Split(",")[0]);
            Y = int.Parse(Id.Split(",")[1]);
            Z = int.Parse(Id.Split(",")[2]);
        }

        public double DistanceTo(JunctionBox other)
        {
            return Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2) + Math.Pow(Z - other.Z, 2));
        }

        public override string ToString()
        {
            return Id;
        }
    }

    List<JunctionBox> junctionBoxes = new();
    public Day08()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            _lines.Add(line);
            junctionBoxes.Add(new JunctionBox(line));
        }

    }

    public override ValueTask<string> Solve_1()
    {
        long output = 0;
        /*
            while link counter < 10
                get closest 2 boxes
                if not in a network, link them, linkCounter++

            this means I need a distance Map, and a network tracker
        */

        List< (double Distance, JunctionBox Node1, JunctionBox Node2)> distanceTracker = new();
        for(int i = 0 ; i< junctionBoxes.Count-1 ; i++)
        {
            for(int j=i+1 ; j<junctionBoxes.Count; j++)
            {
                distanceTracker.Add( new(junctionBoxes[i].DistanceTo(junctionBoxes[j]), junctionBoxes[i], junctionBoxes[j]));
            }
        }
        Console.WriteLine("Distance Map Calculated");
        distanceTracker = distanceTracker.OrderBy(x=>x.Distance).ToList();


        return new($"{output}");
    }

    public override ValueTask<string> Solve_2()
    {
        long output = 0;

        return new($"{output}");
    }

}