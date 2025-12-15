using System.ComponentModel;
using System.Runtime.Intrinsics.Arm;
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


        public int CalculateButtonPressesJoltage()
        {
            Dictionary<int, int> maxPressesOfButton = Buttons.Select(buttons => buttons.Min(b => JoltageRequirements[b]) + 1).Select((x, i) => new { X = x, I = i }).ToDictionary(a => a.I, a => a.X);

            // Find most constrained output
            var mostConstrainedTargetCount = JoltageRequirements.Min();
            var mostConstrainedIndex = JoltageRequirements.IndexOf(mostConstrainedTargetCount);

            // Find any buttons that target this index
            var possibleButtons = Buttons.Select((l, i) => new { buttons = l, index = i }).Where(x => x.buttons.Contains(mostConstrainedIndex));


            Console.WriteLine($"Most ConstrainedIndex {mostConstrainedIndex} - Target Count {mostConstrainedTargetCount}. There are {possibleButtons.Count()} buttons we can hit");

            // Generation combinations that add up to the Joltage requirement
            var combinations = GenerateCombinations2(possibleButtons.Select(x => x.index).ToList(), mostConstrainedTargetCount);

            Console.WriteLine($"There are {combinations.Count()} combinations to review");
            PriorityQueue<StateMachineEntry, int> _queue = new();
            // process those combinations
            foreach (var combination in combinations)
            {
                // generate State entry
                var stateEntry = new StateMachineEntry(JoltageRequirements, this.Buttons.Count, null, combination);
                foreach (var buttonIndex in stateEntry.ButtonPressCount)
                {
                    var buttons = this.Buttons[buttonIndex];
                    foreach (var button in buttons)
                    {
                        stateEntry.CurrentState[button]++;
                    }
                }

                // add valid state entry to queue
                if (!stateEntry.IsInvalid)
                {
                    _queue.Enqueue(stateEntry, stateEntry.TotalButtonPresses);
                }
            }

            while (_queue.Count > 0)
            {
                Console.WriteLine($"Queue size {_queue.Count}");
                var currentEntry = _queue.Dequeue();
                if (currentEntry.IsSatisfied)
                    return currentEntry.TotalButtonPresses;
                mostConstrainedTargetCount = currentEntry.RemainingJoltageRequirements.Where(x => x > 0).Min();
                mostConstrainedIndex = currentEntry.RemainingJoltageRequirements.IndexOf(mostConstrainedTargetCount);

                // possibleButtons = Buttons.Select((l, i) => new { buttons = l, index = i }).Where(x => x.buttons.Contains(mostConstrainedIndex));

                var doNotUseIndex = currentEntry.RemainingJoltageRequirements.Select((x, i) => new { req = x, index = i }).Where(x => x.req == 0).Select(x => x.index).ToList();
                var doNotUseButtons = doNotUseIndex.SelectMany(doNotUse => Buttons.Select((l, i) => new { buttons = l, index = i }).Where(x => x.buttons.Contains(doNotUse)).Select(x => x.index));

                possibleButtons = Buttons.Select((l, i) => new { buttons = l, index = i }).Where(x => x.buttons.Contains(mostConstrainedIndex)).Where(x => !doNotUseButtons.Contains(x.index));

                if (!possibleButtons.Any())
                    continue;

                combinations = GenerateCombinations2(possibleButtons.Select(x => x.index).ToList(), mostConstrainedTargetCount);

                foreach (var combination in combinations)
                {
                    var stateEntry = new StateMachineEntry(JoltageRequirements, this.Buttons.Count, currentEntry.CurrentState, currentEntry.ButtonPressCount);
                    foreach (var buttonIndex in combination)
                    {
                        stateEntry.ButtonPressCount.Add(buttonIndex);
                        var buttons = this.Buttons[buttonIndex];
                        foreach (var button in buttons)
                        {
                            stateEntry.CurrentState[button]++;
                        }
                    }

                    if (!stateEntry.IsInvalid)
                    {
                        _queue.Enqueue(stateEntry, stateEntry.TotalButtonPresses);
                    }
                }
            }


            // rinse and repeat until valid or invalid state is reached
            throw new Exception("Bad code, how did you get here");
        }

        private List<List<int>> GenerateCombinations2(List<int> possibleButtons, int count, int currentButton = 0)
        {
            // Combinations where order does not matter, and repeats are allowed
            var output = new List<List<int>>();
            if (possibleButtons.Count == 1)
            {
                output.Add(Enumerable.Range(1, count).Select(x => possibleButtons[0]).ToList());
            }
            else
            {
                for (int i = 0; i <= count; i++)
                {
                    //remove possibleButtons[0] from copy of list and go recursive
                    var childCombinations = GenerateCombinations2(possibleButtons.Skip(1).ToList(), count - i);

                    foreach (var childCombination in childCombinations)
                    {
                        //take possibleButtons[0] i times
                        var newList = Enumerable.Range(1,i).Select(x=>possibleButtons[0]).ToList();
                        // var newList = new List<int>();

                        // for (int j = 0; j < i; j++)
                        // {
                        //     newList.Add(possibleButtons[0]);
                        // }
                        newList.AddRange(childCombination);
                        output.Add(newList);
                    }
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

        private class StateMachineEntry
        {
            public StateMachineEntry(List<int> requirements, int buttonCount, List<int>? current = null, List<int>? buttonPressCount = null)
            {
                JoltageRequirements = requirements;
                CurrentState = new List<int>();
                if (current == null)
                {
                    for (int i = 0; i < JoltageRequirements.Count; i++)
                    {
                        CurrentState.Add(0);
                    }
                }
                else
                {
                    for (int i = 0; i < current.Count; i++)
                    {
                        CurrentState.Add(current[i]);
                    }
                }

                ButtonPressCount = new List<int>();
                if (buttonPressCount == null)
                {
                    for (int i = 0; i < buttonCount; i++)
                    {
                        ButtonPressCount.Add(0);
                    }
                }
                else
                {
                    for (int i = 0; i < buttonPressCount.Count; i++)
                    {
                        ButtonPressCount.Add(buttonPressCount[i]);
                    }
                }

                //Update Current State. But I need all button info from Machine

            }

            public List<int> JoltageRequirements { get; init; }
            public List<int> CurrentState { get; set; } = new();
            public List<int> ButtonPressCount { get; set; } = new();
            public int TotalButtonPresses => ButtonPressCount.Count();
            public List<int> RemainingJoltageRequirements => CurrentState.Select((x, i) => JoltageRequirements[i] - x).ToList();
            public bool IsInvalid => RemainingJoltageRequirements.Any(x => x < 0);
            public bool IsSatisfied => RemainingJoltageRequirements.Where(x => x == 0).Count() == JoltageRequirements.Count;
        }
    }
}

//187 too low