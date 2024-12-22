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

        public static KeypadCommand FromChar(this char c)
        {
            return c switch
            {
                '^' => KeypadCommand.Up,
                '<' => KeypadCommand.Left,
                '>' => KeypadCommand.Right,
                'v' => KeypadCommand.Down,
                'A' => KeypadCommand.Activate,
                _ => throw new Exception("bad char")
            };
        }
    }
}