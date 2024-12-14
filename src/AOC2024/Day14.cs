using System.Text.RegularExpressions;
using AdventOfCode.Base.Misc;

public class Day14 : BaseDay
{
    List<(Point2D position, Point2D velocity)> RobotPositions = new();
    public Day14()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        string pattern = @"p=(-?\d+),(-?\d+) v=(-?\d+),(-?\d+)";
        while ((line = sr.ReadLine()) != null)
        {
            MatchCollection buttonAMatches = Regex.Matches(line, pattern);

            RobotPositions.Add(new(
                new Point2D { X = int.Parse(buttonAMatches.First().Groups[1].Value), Y = int.Parse(buttonAMatches.First().Groups[2].Value) },
                new Point2D { X = int.Parse(buttonAMatches.First().Groups[3].Value), Y = int.Parse(buttonAMatches.First().Groups[4].Value) }

            ));
        }

        // PrintPositions();
        // for(int i=1 ; i<=5 ; i++)
        // {
        //     var test = AdvanceRobot(RobotPositions.First().position, RobotPositions.First().velocity, i,7-1,11-1);
        //     Console.WriteLine($"Robot advanced to {test}");

        // }
    }

    private void PrintPositions()
    {
        foreach (var robotPosition in RobotPositions)
        {
            Console.WriteLine($"Robot Position {robotPosition.position} moving {robotPosition.velocity}");
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
        //sample
        int maxRow = 103; //7;
        int maxCol = 101; //11;
        int midRow = maxRow / 2;
        int midCol = maxCol / 2;
        int stepCount = 100;
        List<int> quadCount = [0, 0, 0, 0];
        foreach (var robotPosition in RobotPositions)
        {
            var newPosition = AdvanceRobot(robotPosition.position, robotPosition.velocity, stepCount, maxRow, maxCol);
            // Console.WriteLine($"Robot at Row {newPosition.Y}, Col {newPosition.X}");
            if (newPosition.X == midCol || newPosition.Y == midRow) continue;
            if (newPosition.Y < midRow) // 0 or 1
            {
                if (newPosition.X > midCol)
                {
                    quadCount[0]++;
                }
                else
                {
                    quadCount[1]++;
                }
            }
            else
            {
                if (newPosition.X > midCol)
                {
                    quadCount[3]++;
                }
                else
                {
                    quadCount[2]++;
                }
            }
        }

        Console.WriteLine($" Quad 0 : {quadCount[0]} Quad 1 : {quadCount[1]} Quad 2 : {quadCount[2]} Quad 3 : {quadCount[3]}");
        long safetyFactor = quadCount[0] * quadCount[1] * quadCount[2] * quadCount[3];
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1 {safetyFactor}");
    }

    public override ValueTask<string> Solve_2()
    {
        List<Robot> robots = new();
        int gridX = 101;
        int gridY = 103;
        List<int> quadCount = [0,0,0,0];
        foreach(var robotPosition in RobotPositions)
        {
            var pos = new Point2d { X = robotPosition.position.X, Y = robotPosition.position.Y};
            var robot = new Robot(pos, robotPosition.velocity.X, robotPosition.velocity.Y,gridX, gridY);
            robots.Add(robot);
        }
        for(int i = 0 ; i < 1000000 ; i++)
        {
            char[,] currentGrid = new char[gridX,gridY];
            foreach(var robot in robots)
            {
                robot.Advance();
                currentGrid[ robot.CurrentPosition.X, robot.CurrentPosition.Y] = '*';
            }

            Console.WriteLine($"After step {i}");
            DrawGrid(currentGrid, gridX, gridY);
            Console.WriteLine();
            Console.WriteLine();
        }
        return new($"{100}");

    }

    private void DrawGrid(char[,] grid, int gridX, int gridY)
    {
        for(int y=0 ; y<gridY ; y++)
        {
            for(int x=0 ; x<gridX ; x++)
            {
                var xxx = grid[x,y] == '*' ? '*' : '.';
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
        Point2d _initialPosition;
        public Point2d CurrentPosition { get; private set; }
        public bool LoopDetected { get; private set; } = false;
        int _deltaX;
        int _deltaY;
        int _gridX;
        int _gridY;
        List<Point2d> _history = new();
        public Robot(Point2d initialPosition, int deltaX, int deltaY, int gridX, int gridY)
        {
            _initialPosition = initialPosition;
            CurrentPosition = initialPosition;
            _deltaX = deltaX;
            _deltaY = deltaY;
            _gridX = gridX;
            _gridY = gridY;
        }

        public void Advance()
        {
            var newX = CurrentPosition.X + _deltaX;
            while (newX >= _gridX)
                newX -= _gridX;
            while (newX < 0)
                newX += _gridX;

            var newY = CurrentPosition.Y + _deltaY;
            while (newY > -_gridY)
                newY -= _gridY;
            while (newY < 0)
                newY += _gridY;

            CurrentPosition = new Point2d { X = newX, Y = newY };

            // if (_history.Contains(CurrentPosition))
            //     LoopDetected = true;
            if(_history.Where( point => point.X == CurrentPosition.X && point.Y == CurrentPosition.Y).Any())
                LoopDetected = true;

            _history.Add(CurrentPosition);
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