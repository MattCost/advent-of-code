using System.Text.RegularExpressions;
using AdventOfCode.Base.Misc;

public class Day14 : BaseDay
{
    List<Robot> robots = new();
    public Day14()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        string pattern = @"p=(-?\d+),(-?\d+) v=(-?\d+),(-?\d+)";
        int gridX = 101;
        int gridY = 103;
        while ((line = sr.ReadLine()) != null)
        {
            MatchCollection robotInput = Regex.Matches(line, pattern);

            robots.Add( new Robot( 
                new Point2d { 
                    X = int.Parse(robotInput.First().Groups[1].Value), 
                    Y = int.Parse(robotInput.First().Groups[2].Value)},
                int.Parse(robotInput.First().Groups[3].Value), 
                int.Parse(robotInput.First().Groups[4].Value),gridX, gridY));
        }
    }

    private Point2D AdvanceRobot(Point2D position, Point2D velocity, int steps, int rowCount, int colCount)
    {
        Point2D output = new();

        output.X = position.X + (velocity.X * steps);
        while (output.X >= colCount)
            output.X -= colCount;
        while (output.X < 0)
            output.X += colCount;


        output.Y = position.Y + (velocity.Y * steps);
        while (output.Y >= rowCount)
            output.Y -= rowCount;
        while (output.Y < 0)
            output.Y += rowCount;
        return output;
    }

    public override ValueTask<string> Solve_1()
    {
        List<int> quadCount = [0, 0, 0, 0];
        foreach(var robot in robots)
        {
            robot.Advance(100);
            if(robot.Quadrant != -1)
                quadCount[robot.Quadrant]++;
        }
        Console.WriteLine($" Quad 0 : {quadCount[0]} Quad 1 : {quadCount[1]} Quad 2 : {quadCount[2]} Quad 3 : {quadCount[3]}");
        long safetyFactor = quadCount[0] * quadCount[1] * quadCount[2] * quadCount[3];
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1 {safetyFactor}");
    }

    public override ValueTask<string> Solve_2()
    {
        int gridX = 101;
        int gridY = 103;
        foreach (var robot in robots)
        {
            robot.Advance(7900);
        }
        bool treeFound = false;
        int stepCount = 8000;
        while (!treeFound)
        {
            stepCount++;
            char[] currentGrid = new char[gridX * gridY];
            foreach (var robot in robots)
            {
                robot.Advance();
                currentGrid[robot.CurrentPosition.X + gridX * robot.CurrentPosition.Y] = '*';
            }

            var x = new string(currentGrid);
            if (x.Contains("*********"))
            {
                Console.Clear();
                Console.WriteLine($"After step {stepCount}");
                DrawGrid(currentGrid, gridX, gridY);
                Console.WriteLine();
                treeFound = true;
            }
        }
        return new($"{stepCount}");

    }

    private void DrawGrid(char[] grid, int gridX, int gridY)
    {
        for (int y = 0; y < gridY; y++)
        {
            for (int x = 0; x < gridX; x++)
            {
                var xxx = grid[x + y * gridX] == '*' ? '*' : '.';
                Console.Write($"{xxx}");
            }
            Console.Write("\n");
        }
    }

    public record Point2d
    {
        public int X { get; init; }
        public int Y { get; init; }
    }
    public class Robot
    {
        public Point2d CurrentPosition { get; private set; }
        int _deltaX;
        int _deltaY;
        int _gridX;
        int _gridY;
        public Robot(Point2d initialPosition, int deltaX, int deltaY, int gridX, int gridY)
        {
            CurrentPosition = initialPosition;
            _deltaX = deltaX;
            _deltaY = deltaY;
            _gridX = gridX;
            _gridY = gridY;
        }

        public void Advance(int steps = 1)
        {
            var newX = CurrentPosition.X + _deltaX * steps;
            while (newX >= _gridX)
                newX -= _gridX;
            while (newX < 0)
                newX += _gridX;

            var newY = CurrentPosition.Y + _deltaY * steps;
            while (newY > -_gridY)
                newY -= _gridY;
            while (newY < 0)
                newY += _gridY;

            CurrentPosition = new Point2d { X = newX, Y = newY };
        }

        public int Quadrant
        {
            get
            {
                var midX = _gridX / 2;
                var midY = _gridY / 2;
                if (CurrentPosition.X == midX || CurrentPosition.Y == midY) return -1;
                if (CurrentPosition.Y < midY)
                {
                    return (CurrentPosition.X > midX) ? 1 : 0;
                }
                else
                {
                    return (CurrentPosition.X > midX) ? 3 : 2;
                }
            }
        }

    }

}