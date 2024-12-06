using System.Runtime.InteropServices.Marshalling;
using AdventOfCode.Base.Grid8Way;

public class Day04 : BaseDay
{
    List<string> _lines = new();
    int _rows;
    int _cols;
    Grid8Way<char> _grid;

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
        _grid = new Grid8Way<char>(_rows, _cols);

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                _grid.AddNode(_lines[row][col], row, col);
            }
        }

        _grid.PrintGrid();
        _grid.PopulateEdges();

    }

    public override ValueTask<string> Solve_1()
    {
        long output = 0;
        int searchCount = 0;
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                if (_grid.Nodes[row, col] != null && _grid.Nodes[row, col].Value == 'X')
                {
                    searchCount++;
                    output += SearchResult(row, col);
                }
            }
        }

        Console.WriteLine($"Searched {searchCount} spots and found {output} xmas");
        return new(output.ToString());
    }

    private long SearchResult(int row, int col)
    {
        int output = 0;

        // Brute forcing the search, as it was only 8 directions, and 4 chars, so we didn't need to get recursive. Also, the linked matrix was totally unnecessary.
        // Left
        if (col >= 3 && _grid.Nodes[row, col - 1].Value == 'M' && _grid.Nodes[row, col - 2].Value == 'A' && _grid.Nodes[row, col - 3].Value == 'S') output++;
        // UpLeft
        if (col >= 3 && row >= 3 && _grid.Nodes[row - 1, col - 1].Value == 'M' && _grid.Nodes[row - 2, col - 2].Value == 'A' && _grid.Nodes[row - 3, col - 3].Value == 'S') output++;
        // DownLeft
        if (col >= 3 && row <= _rows - 4 && _grid.Nodes[row + 1, col - 1].Value == 'M' && _grid.Nodes[row + 2, col - 2].Value == 'A' && _grid.Nodes[row + 3, col - 3].Value == 'S') output++;

        // Right
        if (col <= _cols - 4 && _grid.Nodes[row, col + 1].Value == 'M' && _grid.Nodes[row, col + 2].Value == 'A' && _grid.Nodes[row, col + 3].Value == 'S') output++;
        // UpRight
        if (col <= _cols - 4 && row >= 3 && _grid.Nodes[row - 1, col + 1].Value == 'M' && _grid.Nodes[row - 2, col + 2].Value == 'A' && _grid.Nodes[row - 3, col + 3].Value == 'S') output++;
        // DownRight
        if (col <= _cols - 4 && row <= _rows - 4 && _grid.Nodes[row + 1, col + 1].Value == 'M' && _grid.Nodes[row + 2, col + 2].Value == 'A' && _grid.Nodes[row + 3, col + 3].Value == 'S') output++;

        // Up
        if (row >= 3 && _grid.Nodes[row - 1, col].Value == 'M' && _grid.Nodes[row - 2, col].Value == 'A' && _grid.Nodes[row - 3, col].Value == 'S') output++;

        // Down
        if (row <= _rows - 4 && _grid.Nodes[row + 1, col].Value == 'M' && _grid.Nodes[row + 2, col].Value == 'A' && _grid.Nodes[row + 3, col].Value == 'S') output++;

        return output;
    }

    public override ValueTask<string> Solve_2()
    {
        long output = 0;
        int searchCount = 0;
        for (int row = 1; row < _rows - 1; row++)
        {
            for (int col = 1; col < _cols - 1; col++)
            {
                if (_grid.Nodes[row, col] != null && _grid.Nodes[row, col].Value == 'A')
                {
                    searchCount++;
                    output += SearchResultXMAS(row, col);
                }
            }
        }

        Console.WriteLine($"Searched {searchCount} spots and found {output} xmas");
        return new(output.ToString());
    }

    private long SearchResultXMAS(int row, int col)
    {
        // Brute forcing the search, as it was only 8 directions, and 4 chars, so we didn't need to get recursive. Also, the linked matrix was totally unnecessary.

        // 2 M above, 2 S below
        if (_grid.Nodes[row - 1, col - 1].Value == 'M' && _grid.Nodes[row - 1, col + 1].Value == 'M' && _grid.Nodes[row + 1, col - 1].Value == 'S' && _grid.Nodes[row + 1, col + 1].Value == 'S') return 1;

        // 2 S above, 2 M below
        if (_grid.Nodes[row - 1, col - 1].Value == 'S' && _grid.Nodes[row - 1, col + 1].Value == 'S' && _grid.Nodes[row + 1, col - 1].Value == 'M' && _grid.Nodes[row + 1, col + 1].Value == 'M') return 1;

        // 2 M left, 2 S right
        if (_grid.Nodes[row - 1, col - 1].Value == 'M' && _grid.Nodes[row + 1, col - 1].Value == 'M' && _grid.Nodes[row - 1, col + 1].Value == 'S' && _grid.Nodes[row + 1, col + 1].Value == 'S') return 1;

        // 2 S left, 2 M right
        if (_grid.Nodes[row - 1, col - 1].Value == 'S' && _grid.Nodes[row + 1, col - 1].Value == 'S' && _grid.Nodes[row - 1, col + 1].Value == 'M' && _grid.Nodes[row + 1, col + 1].Value == 'M') return 1;

        return 0;
    }
}