namespace AdventOfCode.Base.Misc
{
    public class Point2D
    {
        public int X { get; set; }
        public int Y { get; set; }
        public override string ToString()
        {
            return $"X:{X} Y:{Y}";
        }
    }
}