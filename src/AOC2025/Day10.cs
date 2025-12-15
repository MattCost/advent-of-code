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

        // private bool IsValidJoltageState(List<int> buttonPressCounts)
        // {
        //     if (buttonPressCounts.Count != Buttons.Count) return false;

        //     var currentState = new List<int>();
        //     for (int i = 0; i < JoltageRequirements.Count; i++)
        //     {
        //         currentState.Add(0);
        //     }
        //     for (int i = 0; i < buttonPressCounts.Count; i++)
        //     {
        //         var buttons = Buttons[i];
        //         foreach (var button in buttons)
        //         {
        //             currentState[button] += buttonPressCounts[i];
        //         }
        //     }

        //     var output = true;
        //     for (int i = 0; i < currentState.Count; i++)
        //     {
        //         if (currentState[i] != JoltageRequirements[i])
        //             output = false;
        //     }
        //     return output;
        // }

        private int IsCurrentStateValid(int[] currentState)
        {
            if (currentState.Length != JoltageRequirements.Count) throw new ArgumentOutOfRangeException();
            var output = 0;
            for (int i = 0; i < currentState.Length; i++)
            {
                if (currentState[i] > JoltageRequirements[i])
                    return -1;
                if (currentState[i] < JoltageRequirements[i])
                    output = 1;
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
               a   b     c   d     e     f      0,1,2,3
        [.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}

        Button f - pushes 0 and 1.      Max of 3
        Button e - pushes 0 and 2.      Max of 3
        Button d - pushes 2 and 3.      Max of 4
        Button c - pushes 2.            Max of 4
        Button b - pushes 1 and 3.      Max of 5
        Button a - pushes 3.            Max of 7
        
        

        Figure out max button presses

        Figure out valid combinations

        put combos in order of least to most 

        19200 possible combinations

        This wont work for puzzle input



        Start at 0,0,0,0
        have 6 possible moves, so 6 new locations placed in priority heap
        repeat. 


        */
        public int CalculateButtonPressesJoltage()
        {
            Dictionary<int, int> maxPressesOfButton = Buttons.Select(buttons => buttons.Min(b => JoltageRequirements[b]) + 1).Select((x, i) => new { X = x, I = i }).ToDictionary(a => a.I, a => a.X);


            // Find most constrained output
            var mostConstrainedTargetCount = JoltageRequirements.Min();
            var mostConstrainedIndex = JoltageRequirements.IndexOf(mostConstrainedTargetCount);

            // Find any buttons that target this index
            var possibleButtons = Buttons.Select((l, i) => new { buttons = l, index = i }).Where(x => x.buttons.Contains(mostConstrainedIndex));

            // Generation combinations that add up to the Joltage requirement
            var combinations = GenerateCombinations2(JoltageRequirements, possibleButtons.Select(x=>x.index).ToList(), mostConstrainedIndex, mostConstrainedTargetCount);

            // process those combinations
            foreach (var combination in combinations)
            {
                // generate State entry
                var stateEntry = new StateMachineEntry(JoltageRequirements, this.Buttons.Count, null, combination);
                // add valid state entry to queue
            }

            // rinse and repeat until valid or invalid state is reached

            return 0;
        }

        private List<List<int>> GenerateCombinations2(List<int> joltageRequirements, List<int> possibleButtons, int index, int target, int depth = 0)
        {
            // var output = new List<Dictionary<int,int>>();
            var output = new List<List<int>>();
            if (depth < target)
            {
                for (int i = 0; i < possibleButtons.Count(); i++)
                {
                    var childLists = GenerateCombinations2(joltageRequirements, possibleButtons, index, target, depth + 1);
                    if (childLists.Count > 0)
                    {
                        for (int j = 0; j < childLists.Count; j++)
                        {
                            var newList = new List<int> { possibleButtons[i] };
                            newList.AddRange(childLists[j]);

                            // prevent adding a duplicate here?
                            newList.Sort();
                            if (!AlreadyContains(output, newList))
                                output.Add(newList);

                            //compare against each list in output, and 
                        }
                    }
                    else
                    {
                        output.Add([possibleButtons[i]]);
                    }
                }
                // I have 2 buttons to push, 3 times total.
                // 111
                // 112
                // 122
                // 222

            }
            return output;
        }

        private bool AlreadyContains(List<List<int>> output, List<int> newList)
        {
            foreach (var list in output)
            {
                if (list.Count == newList.Count)
                {
                    var equal = true;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] != newList[i])
                            equal = false;
                    }
                    if (equal) return true;
                }
            }
            return false;
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
                    output.Add(new List<int> { i });
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

        // private class QueueEntryJoltage
        // {
        //     public int Depth { get; set; }
        //     public List<int> CurrentState { get; set; }
        //     public int ButtonSelected { get; set; }
        //     public QueueEntryJoltage(List<int> currentState, int buttons, int depth)
        //     {
        //         CurrentState = new List<int>();
        //         for (int i = 0; i < currentState.Count; i++)
        //             CurrentState.Add(currentState[i]);
        //         ButtonSelected = buttons;
        //         Depth = depth;
        //     }
        // }

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
                    for(int i=0 ; i<current.Count ; i++)
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
                    for(int i=0 ; i< buttonPressCount.Count ; i++)
                    {
                        ButtonPressCount.Add(buttonPressCount[i]);
                    }
                }
                
                //Update Current State. But I need all button info from Machine

            }



            public List<int> JoltageRequirements { get; init; }
            public List<int> CurrentState { get; set; } = new();
            public List<int> ButtonPressCount { get; set; } = new();
            public int TotalButtonPresses => ButtonPressCount.Sum();
            public List<int> RemainingJoltageRequirements => CurrentState.Select((x, i) => JoltageRequirements[i] - x).ToList();
        }
    }

}

//187 too low