using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

public class Day13 : BaseDay
{

    class MachineDeets
    {
        public (int x, int y) ButtonA { get; set; }
        public (int x, int y) ButtonB { get; set; }
        public (int x, int y) PrizeLocation { get; set; }
    }

    List<MachineDeets> MachineDetails { get; init; } = [];
    const int ButtonACost = 3;
    const int ButtonBCost = 1;
    const int MaxPresses = 100;
    public Day13()
    {
        StreamReader sr = new StreamReader(InputFilePath);
        string buttonPattern = @"X\+(\d+), Y\+(\d+)";
        string targetPattern = @"X\=(\d+), Y\=(\d+)";
        while (true)
        {
            var buttonAline = sr.ReadLine(); if (string.IsNullOrEmpty(buttonAline)) break;
            var buttonBline = sr.ReadLine(); if (string.IsNullOrEmpty(buttonBline)) break;
            var targetLine = sr.ReadLine(); if (string.IsNullOrEmpty(targetLine)) break;

            MatchCollection buttonAMatches = Regex.Matches(buttonAline, buttonPattern);
            MatchCollection buttonBMatches = Regex.Matches(buttonBline, buttonPattern);
            MatchCollection targetMatches = Regex.Matches(targetLine, targetPattern);

            MachineDetails.Add(new MachineDeets
            {
                ButtonA = (int.Parse(buttonAMatches.First().Groups[1].Value), int.Parse(buttonAMatches.First().Groups[2].Value)),
                ButtonB = (int.Parse(buttonBMatches.First().Groups[1].Value), int.Parse(buttonBMatches.First().Groups[2].Value)),
                PrizeLocation = (int.Parse(targetMatches.First().Groups[1].Value), int.Parse(targetMatches.First().Groups[2].Value))
            });

            if (sr.ReadLine() == null)
            {
                break;
            }
        }
    }

    public override ValueTask<string> Solve_1()
    {
        long output = 0;
        foreach (var machine in MachineDetails)
        {
            if (CanPlayToWin(machine, out var pressDetails))
            {
                output += ButtonACost * pressDetails.ButtonACount + ButtonBCost * pressDetails.ButtonBCount;
            }
        }

        return new($"{output}");
    }

    private bool CanPlayToWin(MachineDeets machine, out (int ButtonACount, int ButtonBCount) pressDetails)
    {
        var xCanWin = CanPlayToWinButton(machine.PrizeLocation.x, machine.ButtonA.x, machine.ButtonB.x, out var xPressDetails);
        var yCanWin = CanPlayToWinButton(machine.PrizeLocation.y, machine.ButtonA.y, machine.ButtonB.y, out var yPressDetails);
        if (xCanWin && yCanWin)
        {
            var validCombos = xPressDetails.Where(xPress => yPressDetails.Contains(xPress));
            if (validCombos.Any())
            {
                var ordered = validCombos.OrderBy(combo => ButtonACost * combo.ButtonACount + ButtonBCost * combo.ButtonBCount);
                pressDetails = (ordered.First().ButtonACount, ordered.First().ButtonBCount);
                return true;
            }
        }
        pressDetails = (0, 0);
        return false;
    }

    private bool CanPlayToWinButton(int target, int stepA, int stepB, out List<(int ButtonACount, int ButtonBCount)> PressDetails)
    {
        PressDetails = [];
        //Brute. Works for part 1, obviously not for part 2 :)
        for (int aPress = 1; aPress <= 100; aPress++)
        {
            if (aPress * stepA > target) break;
            for (int bPress = 1; bPress <= 100; bPress++)
            {
                var xx = aPress * stepA + bPress * stepB;
                if (xx > target) break;
                if (xx == target)
                {
                    PressDetails.Add(new(aPress, bPress));
                }
            }
        }
        return PressDetails.Any();

    }

    public override ValueTask<string> Solve_2()
    {
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2");
    }

}