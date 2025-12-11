using System.ComponentModel;
using System.Text;

public class Day10 : BaseDay
{
    List<string> _lines = new();
    List<Machine> machines = new();

    public Day10()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            _lines.Add(line);
            machines.Add(new Machine(line));
        }

    }

    public override ValueTask<string> Solve_1()
    {
        long output = 0;

        foreach (var machine in machines)
        {
            output += machine.CalculateButtonPresses();
        }


        return new($"{output}");
    }

    public override ValueTask<string> Solve_2()
    {
        long output = 0;

        return new($"{output}");
    }

    private class Machine
    {
        public bool[] IndicatorLights { get; set; }
        public int IndicatorLightSize => IndicatorLights.Length;
        public List<List<int>> Buttons { get; set; } = new();
        public List<int> JoltageRequirements { get; set; } = new();

        public Machine(string input)
        {
            var parts = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var lightParts = parts.First();
            // var joltageParts = parts.Last();
            IndicatorLights = new bool[lightParts.Length - 2];
            for (int i = 1; i < lightParts.Length - 1; i++)
            {
                IndicatorLights[i - 1] = lightParts[i] == '#';
            }

            for (int i = 1; i < parts.Length - 1; i++)
            {
                var buttons = parts[i].TrimStart('(').TrimEnd(')').Split(',').Select(x => int.Parse(x)).ToList();
                Buttons.Add(buttons);
            }
        }

        // Ignore Joltage parts for now

        public int CalculateButtonPresses()
        {
            var _queue = new Queue<QueueEntry>();

            for (int i = 0; i < Buttons.Count; i++)
            {
                _queue.Enqueue(new QueueEntry(new bool[IndicatorLightSize], i, 1));
            }

            while (_queue.Count > 0)
            {
                var working = _queue.Dequeue();
                foreach (var button in Buttons[working.ButtonSelected])
                {
                    working.CurrentState[button] = !working.CurrentState[button];
                }
                bool match = true;
                for (int i = 0; i < IndicatorLightSize; i++)
                {
                    if (working.CurrentState[i] != IndicatorLights[i])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                    return working.Depth;

                for (int i = 0; i < Buttons.Count; i++)
                {
                    _queue.Enqueue(new QueueEntry(working.CurrentState, i, working.Depth + 1));
                }
            }

            return 0;
        }

        private class QueueEntry
        {
            public int Depth { get; set; }
            public bool[] CurrentState { get; set; }
            public int ButtonSelected { get; set; }
            public QueueEntry(bool[] currentState, int buttons, int depth)
            {
                CurrentState = new bool[currentState.Length];
                for (int i = 0; i < currentState.Length; i++)
                    CurrentState[i] = currentState[i];
                ButtonSelected = buttons;
                Depth = depth;
            }
        }
    }

}

