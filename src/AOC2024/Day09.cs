public class File
{
    public int Id { get; init; }
    public int FileSize { get; init; }
    public int FirstBlock { get; set; }
    // public int LastBlock => FirstBlock + (FileSize > 0 ? FileSize - 1 : 0);
    public int LastBlock => FirstBlock + FileSize - 1;
    public int NextBlock => LastBlock + 1;
    public override string ToString()
    {
        return new string($"{Id % 10}"[0], FileSize);
        // return FileSize > 0 ? new string($"{Id % 10}"[0], FileSize) : "!";
    }
    public long Checksum
    {
        get
        {
            if (FileSize == 0) return 0;
            var checksum = 0;
            for (int i = FirstBlock; i <= LastBlock; i++)
            {
                checksum += i * Id;
            }
            return checksum;
        }
    }
}

public class Day09 : BaseDay
{
    List<File> UnsortedFiles = new();
    List<int> UnsortedFileSystem = new();
    List<int> SortedFileSystem = new();
    SortedDictionary<int, File> OptimizedFileSystem = new();
    bool printDebug = true;
    public Day09()
    {
        StreamReader sr = new StreamReader(InputFilePath);

        string line = sr.ReadLine() ?? string.Empty;
        bool onFile = true;
        int currentBlock = 0;
        int currentFileId = 0;
        for (int i = 0; i < line.Length; i++)
        {
            var entrySize = int.Parse($"{line[i]}");
            if (onFile)
            {
                UnsortedFiles.Add(new File
                {
                    Id = currentFileId,
                    FileSize = entrySize,
                    FirstBlock = currentBlock
                });
                UnsortedFileSystem.AddRange(Enumerable.Repeat(currentFileId, entrySize));
                currentFileId++;
            }
            else
            {
                UnsortedFileSystem.AddRange(Enumerable.Repeat(-1, entrySize));
            }
            currentBlock += entrySize;
            onFile = !onFile;
        }
    }

    public override ValueTask<string> Solve_1()
    {
        long output = 0;
        PopulatedSortedFileSystem();
        for (int i = 0; i < SortedFileSystem.Count; i++)
        {
            output += SortedFileSystem[i] * i;
        }
        return new($"{output}");
    }

    private void PopulatedSortedFileSystem()
    {
        int start = 0;
        int end = UnsortedFileSystem.Count - 1;

        // Just in case we are pointing to an empty spot
        while (UnsortedFileSystem[start] == -1) start++;
        while (UnsortedFileSystem[end] == -1) end--;

        while (start <= end)
        {
            if (UnsortedFileSystem[start] != -1)
            {
                // If we are pointing to a file copy it over to the sorted filesystem
                SortedFileSystem.Add(UnsortedFileSystem[start]);
            }
            else
            {
                // But if we are point to a blank spot, copy over a block from the end of the unsorted filesystem
                SortedFileSystem.Add(UnsortedFileSystem[end--]);
                // And then advance the end pointer till we are pointing at the next block, for the next time
                while (UnsortedFileSystem[end] == -1) end--;
            }
            start++;
        }
    }

    public override ValueTask<string> Solve_2()
    {

        PopulateOptimizedFileSystem();
        long output = OptimizedFileSystem.Values.Select(file => file.Checksum).Sum();
        return new($"{output}");
    }

    private void PopulateOptimizedFileSystem()
    {
        // Make a copy of the files;
        foreach (var file in UnsortedFiles)
        {
            if (OptimizedFileSystem.ContainsKey(file.FirstBlock)) throw new Exception("dupe");
            OptimizedFileSystem.Add(file.FirstBlock, file);
        }
        Console.WriteLine($"There are {UnsortedFiles.Count} files to sort");

        if (printDebug)
        {
            Console.WriteLine("Initial File System");
            PrintDebug2();
            Console.WriteLine("Optimizing file system");
        }

        var filesToMove = OptimizedFileSystem.Values.OrderByDescending(file => file.Id);

        foreach (var fileToMove in filesToMove)
        {
            if (printDebug) Console.WriteLine($"Processing file id {fileToMove.Id} / {fileToMove}. Starting Block {fileToMove.FirstBlock}. Looking for a free space of {fileToMove.FileSize} blocks");

            var orderedFileKeys = OptimizedFileSystem.Keys;
            var currentFileKey = orderedFileKeys.GetEnumerator();
            var nextFileKey = orderedFileKeys.GetEnumerator();
            nextFileKey.MoveNext();

            int firstFreeSpaceIndex = -1;
            bool fileMoved = false;
            while (currentFileKey.MoveNext() && nextFileKey.MoveNext())
            {
                var currentFile = OptimizedFileSystem[currentFileKey.Current];
                var nextFile = OptimizedFileSystem[nextFileKey.Current];

                if (currentFile.NextBlock == nextFile.FirstBlock) continue; //move along to a free space

                var freeSpaceIndex = currentFile.NextBlock;

                if (firstFreeSpaceIndex == -1) firstFreeSpaceIndex = freeSpaceIndex;

                var freeSpaceSize = nextFile.FirstBlock - currentFile.NextBlock;

                if (fileToMove.FirstBlock <= firstFreeSpaceIndex)
                {
                    //if our first available free space is past the file we are trying to move, then we are done. Any other file will already be more left of this file.
                    Console.WriteLine("No free space to the left of the remaining files to move. Optimization Complete");
                    PrintDebug2();
                    return;
                };

                if (fileToMove.FirstBlock > freeSpaceIndex && fileToMove.FileSize <= freeSpaceSize)
                {
                    if (printDebug) Console.WriteLine($"Free Space of size {freeSpaceSize} found at {freeSpaceIndex}. Moving file id {fileToMove.Id} / {fileToMove} to {freeSpaceIndex}");
                    if (!OptimizedFileSystem.Remove(fileToMove.FirstBlock)) throw new Exception("WTF");
                    fileToMove.FirstBlock = freeSpaceIndex;
                    if (OptimizedFileSystem.ContainsKey(freeSpaceIndex)) throw new Exception("WTF2");
                    OptimizedFileSystem.Add(fileToMove.FirstBlock, fileToMove);
                    fileMoved = true;
                    PrintDebug2();
                    break; //get out of while loop and move to next file;
                }
            }

            if (firstFreeSpaceIndex == -1)
            {
                Console.WriteLine("No free space remaining. Optimization Complete");
                PrintDebug2();
                return;
            }
            if (!fileMoved)
            {
                if (printDebug) Console.WriteLine($"No free space large enough found for file Id {fileToMove.Id} / {fileToMove}");
            }
        }

        Console.WriteLine("Optimization Complete");
        PrintDebug2();

    }
    private void PrintDebug2()
    {
        if (!printDebug) return;
        int fileSystemIndex = 0;
        var currentFile = OptimizedFileSystem.Keys.Order().GetEnumerator();

        while (currentFile.MoveNext())
        {
            var file = OptimizedFileSystem[currentFile.Current];
            while (fileSystemIndex < file.FirstBlock)
            {
                Console.Write('.');
                fileSystemIndex++;
            }

            Console.Write(file.ToString());
            fileSystemIndex += file.FileSize;
        }
        Console.Write('\n');
    }


}