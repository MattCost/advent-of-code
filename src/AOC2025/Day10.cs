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
            output += machine.CalculateButtonPressesIndicator();
        }


        return new($"{output}");
    }

    public override ValueTask<string> Solve_2()
    {
        long output = 0;

        foreach (var machine in machines)
        {
            output += machine.CalculateButtonPressesJoltage();
        }
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

            JoltageRequirements = parts.Last().TrimStart('{').TrimEnd('}').Split(',').Select(x => int.Parse(x)).ToList();
        }

        private bool IsValidJoltageState(List<int> input)
        {
            if (input.Count != Buttons.Count) return false;

            var currentState = new List<int>();
            for (int i = 0; i < JoltageRequirements.Count; i++)
            {
                currentState.Add(0);
            }
            for (int i = 0; i < input.Count; i++)
            {
                var buttons = Buttons[i];
                foreach (var button in buttons)
                {
                    currentState[button] += input[i];
                }
            }

            var output = true;
            for (int i = 0; i < currentState.Count; i++)
            {
                if (currentState[i] != JoltageRequirements[i])
                    output = false;
            }
            return output;
        }

        public int CalculateButtonPressesIndicator()
        {
            var _queue = new Queue<QueueEntryIndicator>();

            for (int i = 0; i < Buttons.Count; i++)
            {
                _queue.Enqueue(new QueueEntryIndicator(new bool[IndicatorLightSize], i, 1));
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
                    _queue.Enqueue(new QueueEntryIndicator(working.CurrentState, i, working.Depth + 1));
                }
            }

            return 0;
        }


        /*
               0   1     2   3     4     5      0,1,2,3
        [.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}


        Button 0 - pushes 3.            Max of 7
        Button 1 - pushes 1 and 3.      Max of 5
        Button 2 - pushes 2.            Max of 4
        Button 3 - pushes 2 and 3.      Max of 4
        Button 4 - pushes 0 and 2.      Max of 3
        Button 5 - pushes 0 and 1.      Max of 3

        Figure out max button presses

        Figure out valid combinations

        put combos in order of least to most 

        19200 possible combinations

        */
        public int CalculateButtonPressesJoltage()
        {
            Dictionary<int, int> maxPressesOfButton = Buttons.Select(buttons => buttons.Min(b => JoltageRequirements[b]) + 1).Select((x, i) => new { X = x, I = i }).ToDictionary(a => a.I, a => a.X);
            
            var combinations = GenerateCombinations(maxPressesOfButton);
            Console.WriteLine($"Generated {combinations.Count()} combinations");

            var validCombinations = combinations.Where(IsValidJoltageState);
            Console.WriteLine($"{validCombinations.Count()} are valid");

            var orderedValidCombinations = validCombinations.OrderBy(combination => combination.Sum());

            if (orderedValidCombinations.Any()) return orderedValidCombinations.First().Sum();

            return 0;
        }

        private bool SumCombination(List<int> combination)
        {
            throw new NotImplementedException();
        }

        private List<List<int>> GenerateCombinations(Dictionary<int, int> maxPressesOfButton)
        {
            var output = new List<List<int>>();
            var childLists = GenerateCombinationsRecursive(maxPressesOfButton, 1);
            for (int i = 0; i < maxPressesOfButton[0]; i++)
            {
                for (int j = 0; j < childLists.Count; j++)
                {
                    var newList = new List<int>
                    {
                        i
                    };
                    newList.AddRange(childLists[j]);
                    output.Add(newList);

                }
            }

            return output;
        }

        private List<List<int>> GenerateCombinationsRecursive(Dictionary<int, int> maxPressesOfButton, int button)
        {
            var output = new List<List<int>>();
            if (button >= Buttons.Count)
                return output;

            var childLists = GenerateCombinationsRecursive(maxPressesOfButton, button + 1);
            for (int i = 0; i < maxPressesOfButton[button]; i++)
            {
                if (childLists.Count > 0)
                {
                    for (int j = 0; j < childLists.Count; j++)
                    {
                        var newList = new List<int>
                        {
                            i
                        };

                        newList.AddRange(childLists[j]);
                        output.Add(newList);
                    }
                }
                else
                {
                    output.Add( new List<int> {i});
                }
            }
            return output;
        }

        private class QueueEntryIndicator
        {
            public int Depth { get; set; }
            public bool[] CurrentState { get; set; }
            public int ButtonSelected { get; set; }
            public QueueEntryIndicator(bool[] currentState, int buttons, int depth)
            {
                CurrentState = new bool[currentState.Length];
                for (int i = 0; i < currentState.Length; i++)
                    CurrentState[i] = currentState[i];
                ButtonSelected = buttons;
                Depth = depth;
            }
        }

        private class QueueEntryJoltage
        {
            public int Depth { get; set; }
            public List<int> CurrentState { get; set; }
            public int ButtonSelected { get; set; }
            public QueueEntryJoltage(List<int> currentState, int buttons, int depth)
            {
                CurrentState = new List<int>();
                for (int i = 0; i < currentState.Count; i++)
                    CurrentState.Add(currentState[i]);
                ButtonSelected = buttons;
                Depth = depth;
            }
        }
    }

}

//187 too low