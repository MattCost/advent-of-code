using System.Collections;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;

public class Day03 : BaseDay
{

    List<string> BatteryBanks = new();

    public Day03()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            BatteryBanks.Add(line);

        }
    }

    public override ValueTask<string> Solve_1()
    {
        long output = 0;
        var targets = Enumerable.Range(1, 9).OrderDescending().Select(x => x.ToString());
        foreach (var bank in BatteryBanks)
        {
            //find largest number. If at the end of line, find second largest to the left.
            var largestMatch = targets.Where(target => bank.IndexOf(target) != -1).First();
            //if not at end of line, find next largest number to the right
            if (bank.IndexOf(largestMatch) == bank.Length - 1)
            {
                Console.WriteLine("Largest match is at end");
                var secondMatch = targets.Where(target => bank.Substring(0, bank.Length - 1).IndexOf(target) != -1).First();
                Console.WriteLine($"Bank {bank} - SecondMatch {secondMatch} LargestMatch {largestMatch}");
                output += long.Parse($"{secondMatch}{largestMatch}");
            }
            else
            {
                var secondMatch = targets.Where(target => bank.Substring(bank.IndexOf(largestMatch) + 1).IndexOf(target) != -1).First();
                Console.WriteLine($"Bank {bank} -  LargestMatch {largestMatch} SecondMatch {secondMatch}");

                output += long.Parse($"{largestMatch}{secondMatch}");
            }
        }

        return new($"{output}");

    }



    public override ValueTask<string> Solve_2()
    {
        long output = 0;
        var targets = Enumerable.Range(1, 9).OrderDescending().Select(x => x.ToString());
        foreach (var bank in BatteryBanks)
        {
            var currentBank = new StringBuilder();
            var currentIndex = 0;
            for (int i = 11; i >= 0; i--)
            {
                var workingBank = bank.Substring(currentIndex, bank.Length - (i + currentIndex));
                var largestMatch = targets.Where(target => workingBank.IndexOf(target) != -1).First();
                currentBank.Append(largestMatch);
                currentIndex += workingBank.IndexOf(largestMatch) + 1;
            }
            Console.WriteLine($"Bank {bank} - Max Power {currentBank.ToString()}");
            output += long.Parse(currentBank.ToString());
        }


        return new($"{output}");

    }
}