

using System.Diagnostics.CodeAnalysis;

public class Antenna
{
    public int X { get; set; }
    public int Y { get; set; }
    public char Value { get; set; }

    public override string ToString()
    {
        return $"{Value} X:{X} Y:{Y}";
    }
}

public class Point
{
    public int X { get; set; }
    public int Y { get; set; }
    public override string ToString()
    {
        return $"X:{X} Y:{Y}";
    }
}

// Custom comparer for the Product class
class PointComparer : IEqualityComparer<Point>
{
    public bool Equals(Point? p1, Point? p2)
    {

        //Check whether the compared objects reference the same data.
        if (Object.ReferenceEquals(p1, p2)) return true;

        //Check whether any of the compared objects is null.
        if (Object.ReferenceEquals(p1, null) || Object.ReferenceEquals(p2, null))
            return false;

        //Check whether the products' properties are equal.
        return p1.X == p2.X && p1.Y == p2.Y;
    }

    public int GetHashCode([DisallowNull] Point obj)
    {
                //Check whether the object is null
        if (Object.ReferenceEquals(obj, null)) return 0;

        return obj.X.GetHashCode() ^ obj.Y.GetHashCode();
    }
}

public class Day08 : BaseDay
{
    List<string> _lines = new();
    List<Antenna> _antennas = new();
    int _maxX;
    int _maxY;
    public Day08()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        int y = 0;
        while ((line = sr.ReadLine()) != null)
        {
            _lines.Add(line);
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] != '.')
                {
                    _antennas.Add(new Antenna { X = i, Y = y, Value = line[i] });
                }
            }
            y++;
        }
        _maxX = _lines.First().Length;
        _maxY = y;
    }

    public override ValueTask<string> Solve_1()
    {
        var antinodeLocations = new List<Point>();
        var signalTypes = _antennas.Select(a => a.Value).Distinct();
        foreach (var signalType in signalTypes)
        {
            var antennas = _antennas.Where(a => a.Value == signalType).ToList();
            // Console.WriteLine($"Processing {antennas.Count()} antennas of type {signalType}");
            for (int i = 0; i < antennas.Count() - 1; i++)
            {
                for (int j = i + 1; j < antennas.Count(); j++)
                {
                    // Console.WriteLine($"Attempting to process pair {i}, {j}");
                    var nodes = GenerateAntinodes(antennas[i], antennas[j]);
                    var validNodes = nodes.Where(InBounds);
                    // Console.WriteLine($"\tFound {nodes.Count()} node locations, {validNodes.Count()} of which are in-bounds");
                    antinodeLocations.AddRange(validNodes);
                }
            }
        }
        var output = antinodeLocations.Distinct(new PointComparer()).Count().ToString();
        Console.WriteLine($"We have {antinodeLocations.Count()} nodes, {output} of which are at unique locations");
        PrintOut(antinodeLocations.Distinct());
        return new(output.ToString());
    }

    private void PrintOut(IEnumerable<Point> antinodeLocations)
    {
        // Print out the lines
        // but if we have a node print that out instead
        // but if we have an antenna print that out.
        for (int y = 0; y < _maxY; y++)
        {
            for (int x = 0; x < _maxX; x++)
            {
                var antenna = _antennas.Where( a => a.X == x && a.Y==y);
                if(antenna.Any())
                {
                    Console.Write($"{antenna.First().Value}");
                } else if(antinodeLocations.Where( a => a.X == x && a.Y==y).Any())
                {
                    Console.Write('#');
                }
                else
                {
                    Console.Write('.');
                }
            }
            Console.Write("\n");
        }
    }

    private IEnumerable<Point> GenerateAntinodes(Antenna antenna1, Antenna antenna2)
    {
        var deltaX = antenna2.X - antenna1.X;
        var deltaY = antenna2.Y - antenna1.Y;

        // Console.WriteLine($"Generate Antinodes. Antenna 1 {antenna1} Antenna 2 {antenna2}");
        // Console.WriteLine($"DeltaX {deltaX} DeltaY {deltaY}");

        var possible1 = new Point
        {
            X = antenna2.X + deltaX,
            Y = antenna2.Y + deltaY
        };
        var possible2 = new Point
        {
            X = antenna1.X - deltaX,
            Y = antenna1.Y - deltaY
        };

        // Console.WriteLine($"Possible Locations {possible1} and {possible2}");
        return new List<Point> { possible1, possible2 };
    }

    private bool InBounds(Point point)
    {
        if (point.X < 0) return false;
        if (point.X >= _maxX) return false;
        if (point.Y < 0) return false;
        if (point.Y >= _maxY) return false;
        return true;
    }

    public override ValueTask<string> Solve_2()
    {
        var antinodeLocations = new List<Point>();
        var signalTypes = _antennas.Select(a => a.Value).Distinct();
        foreach (var signalType in signalTypes)
        {
            var antennas = _antennas.Where(a => a.Value == signalType).ToList();
            // Console.WriteLine($"Processing {antennas.Count()} antennas of type {signalType}");
            for (int i = 0; i < antennas.Count() - 1; i++)
            {
                for (int j = i + 1; j < antennas.Count(); j++)
                {
                    // Console.WriteLine($"Attempting to process pair {i}, {j}");
                    var nodes = GenerateAntinodes2(antennas[i], antennas[j]);
                    var validNodes = nodes.Where(InBounds);
                    // Console.WriteLine($"\tFound {nodes.Count()} node locations, {validNodes.Count()} of which are in-bounds");
                    antinodeLocations.AddRange(validNodes);
                }
            }
        }
        var output = antinodeLocations.Distinct(new PointComparer()).Count().ToString();
        Console.WriteLine($"We have {antinodeLocations.Count()} nodes, {output} of which are at unique locations");
        PrintOut(antinodeLocations.Distinct());
        return new(output.ToString());

    }

    private IEnumerable<Point> GenerateAntinodes2(Antenna antenna1, Antenna antenna2)
    {

        var output = new List<Point>();
        int a = antenna2.Y - antenna1.Y;
        int b = antenna1.X - antenna2.X;
        double c = antenna1.Y * (antenna2.X-antenna1.X) - antenna1.X*(antenna2.Y-antenna1.Y);
        for(int x = 0 ; x<_maxX ; x++)
        {
            double y = (-c - (a*x)) / b;
            var yInt = Convert.ToInt32(y);
            if(yInt == y)
            {
                var point = new Point { X=x, Y=yInt};
                if(InBounds(point)) output.Add(point);
            }
        }
        return output;
    }


}