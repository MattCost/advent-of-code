using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

public class Day13 : BaseDay
{

    class MachineDetail
    {
        public (int x, int y) ButtonA { get; set; }
        public (int x, int y) ButtonB { get; set; }
        public (int x, int y) PrizeLocation { get; set; }
    }

    List<MachineDetail> MachineDetails { get; init; } = [];
    const int ButtonACost = 3;
    const int ButtonBCost = 1;
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

            MachineDetails.Add(new MachineDetail
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
        for (int i = 0; i < MachineDetails.Count; i++)
        {
            if (FindWinningCombo(MachineDetails[i], 0, out var pressDetails))
            {
                if (pressDetails.ButtonACount <= 100 && pressDetails.ButtonBCount <= 100)
                {
                    output += ButtonACost * pressDetails.ButtonACount + ButtonBCost * pressDetails.ButtonBCount;
                }
            }
        }

        return new($"{output}");
    }

    public override ValueTask<string> Solve_2()
    {
        long output = 0;
        for (int i = 0; i < MachineDetails.Count; i++)
        {
            if (FindWinningCombo(MachineDetails[i], 10000000000000, out var pressDetails))
            {
                output += ButtonACost * pressDetails.ButtonACount + ButtonBCost * pressDetails.ButtonBCount;
            }
        }

        return new($"{output}");
    }

    private bool FindWinningCombo(MachineDetail machine, long offset, out (long ButtonACount, long ButtonBCount) pressDetails)
    {
        var Q = machine.ButtonA.x;
        var W = machine.ButtonB.x;
        var E = machine.ButtonA.y;
        var R = machine.ButtonB.y;

        var b = (E * (machine.PrizeLocation.x + offset) - Q * (machine.PrizeLocation.y + offset)) / ((E * W) - (Q * R));
        var a1 = (machine.PrizeLocation.x + offset - (b * W)) / Q;
        var a1d = (machine.PrizeLocation.x + offset - (b * W)) / Convert.ToDouble(Q);
        var a2 = (machine.PrizeLocation.y + offset - (b * R)) / E;


        if (b >= 0 && a1 >= 0 && a1 == a2 && a1 == a1d)
        {
            pressDetails = (a1, b);
            return true;
        }
        else
        {
            pressDetails = (0, 0);
            return false;
        }
    }
}