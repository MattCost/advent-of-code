// public class Day02 : BaseDay
// {
//     List<string> _lines = new();

//     public DayX()
//     {
//         StreamReader sr = new StreamReader(InputFilePath);

//         string? line;
//         while ((line = sr.ReadLine()) != null)
//         {
//             _lines.Add(line);
//         }

//     }

//     public override ValueTask<string> Solve_1()
//     {
//         return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1");
//     } 

//     public override ValueTask<string> Solve_2()
//     {
//         return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2");    
//     } 

// }