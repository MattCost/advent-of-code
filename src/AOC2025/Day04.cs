using AdventOfCode.Base.Grid8Way;

public class Day04 : BaseDay
{
    List<string> _lines = new();
    int _rows;
    int _cols;
    public Day04()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            _lines.Add(line);
        }

        _rows = _lines.Count();
        _cols = _lines[0].Length;
    }

    private class PaperLocation
    {
        public bool ContainsRoll { get; set; } = false;
        public int NeighboringRolls { get; set; } = 0;
    }

    public override ValueTask<string> Solve_1()
    {
        long output = 0;

        var grid = GenerateGrid();
        // grid.PrintGrid();

        var allNodes = grid.Nodes.Cast<Grid8WayNode<PaperLocation>>();
        output = allNodes.Where(node => node.Value.ContainsRoll && node.Value.NeighboringRolls < 4).Count();
        // for (int r = 0; r < grid.Rows; r++)
        // {
        //     for (int c = 0; c < grid.Cols; c++)
        //     {
        //         if (grid.Nodes[r, c].Value == '@')
        //         {
        //             var adjacentNodes = grid.GetAdjacentNodes(r, c);
        //             int adjacentPaperRollCount = 0;
        //             for (int i = 0; i < adjacentNodes.Count; i++)
        //             {
        //                 if (adjacentNodes[i].Visited == false)
        //                 {
        //                     adjacentNodes[i].Visited = true;
        //                     if (adjacentNodes[i].Value == '@')
        //                         adjacentPaperRollCount++;
        //                 }
        //             }
        //             // Console.WriteLine($"Row:{r} Col:{c} AdjacentCount:{adjacentPaperRollCount}");
        //             if (adjacentPaperRollCount < 4)
        //             {
        //                 output++;
        //             }
        //             grid.ResetVisited();
        //         }
        //     }
        // }
        return new($"{output}");
    }

    public override ValueTask<string> Solve_2()
    {
        return new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2");
    }

    private Grid8Way<PaperLocation> GenerateGrid()
    {
        var grid = new Grid8Way<PaperLocation>(_rows, _cols);
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                var current = _lines[row][col];

                grid.AddNode(new PaperLocation { ContainsRoll = current == '@' }, row, col);
            }
        }
        grid.PopulateEdges();

        //Traverse grid to Calculate Neighbor count
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                var adjacentNodes = grid.GetAdjacentNodes(row, col);
                grid.Nodes[row, col].Value.NeighboringRolls = adjacentNodes.Where(node => node.Value.ContainsRoll).Count();
            }
        }
        return grid;
    }
}

