namespace AdventOfCode.Base.Misc
{
    public enum KeypadCommand
    {
        Up,
        Down,
        Left,
        Right,
        Activate
    }


    public static class KeypadCommandExtensions
    {
        public static string ToPrintString(this KeypadCommand command)
        {
            return command switch
            {
                KeypadCommand.Up => "^",
                KeypadCommand.Left => "<",
                KeypadCommand.Right => ">",
                KeypadCommand.Down => "v",
                KeypadCommand.Activate => "A",
                _ => string.Empty
            };

        }
    }
}