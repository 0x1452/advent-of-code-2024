using Common;
using Serilog;
using Serilog.Events;
using System.Diagnostics;

ILogger logger = new LoggerSetup().Logger;

SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");
SolvePart2("Input/example.txt");
SolvePart2("Input/input.txt");
SolvePart2_2("Input/example.txt");
SolvePart2_2("Input/input.txt");

void SolvePart1(string filepath)
{
    long?[] storage = ParseInput(filepath);

    if (logger.IsEnabled(LogEventLevel.Debug))
        logger.Debug("{Storage}", string.Join(" ", storage.Select(i => i is not null ? i.Value.ToString() : ".")));

    var sw = Stopwatch.StartNew();
    var endIndex = storage.Length - 1;

    for (int i = 0; i < storage.Length; i++)
    {
        while (storage[endIndex] is null)
        {
            endIndex--;
        }

        if (storage[i] != null)
            continue;

        if (i >= endIndex)
            break;

        storage[i] = storage[endIndex];
        storage[endIndex] = null;


        if (logger.IsEnabled(LogEventLevel.Verbose))
            logger.Verbose("{Storage}", string.Join(" ", storage.Select(i => i is not null ? i.Value.ToString() : ".")));
    }

    if (logger.IsEnabled(LogEventLevel.Debug))
        logger.Debug("{Storage}", string.Join(" ", storage.Select(i => i is not null ? i.Value.ToString() : ".")));

    long checksum = 0;
    for (int i = 0; i < storage.Length; i++)
    {
        checksum += i * (storage[i] ?? 0);
    }

    sw.Stop();

    logger.Information("[Part1:{Filepath}] {Checksum} {ElapsedMs}ms", filepath, checksum, sw.Elapsed.TotalMilliseconds);
}

// Doing some unnecessary stuff here again: you don't need to recreate the actual storage, just keep track of indices/lengths of each block.
// I tried to properly plan ahead for other challenges but jumped into code too early for this one.
// -> see SolvePart2_2 for a better approach...
void SolvePart2(string filepath)
{
    long?[] storage = ParseInput(filepath);

    if (logger.IsEnabled(LogEventLevel.Debug))
        logger.Debug("{Storage}", string.Join(" ", storage.Select(i => i is not null ? i.Value.ToString() : ".")));

    var sw = Stopwatch.StartNew();
    var endIndex = storage.Length - 1;
    long? lowestBlockIdProcessed = int.MaxValue;

    while (true)
    {
        var startIndex = Array.IndexOf(storage, null);

        while (storage[endIndex] is null || storage[endIndex] >= lowestBlockIdProcessed)
        {
            endIndex--;
        }

        if (startIndex >= endIndex)
            break;

        var storageBlock = GetBlock(endIndex, storage, ascending: false);
        var storageBlockSize = storageBlock.EndIndex - storageBlock.StartIndex + 1;

        Block? freeBlock = null;
        int freeBlockSize = 0;
        bool foundFittingBlock = false;

        while (!foundFittingBlock)
        {
            if (startIndex >= storageBlock.StartIndex)
                break;

            freeBlock = GetBlock(startIndex, storage, ascending: true);
            freeBlockSize = freeBlock.EndIndex - freeBlock.StartIndex + 1;

            if (storageBlockSize <= freeBlockSize)
            {
                foundFittingBlock = true;
            }
            else
            {
                startIndex = Array.IndexOf(storage, null, freeBlock.EndIndex + 1);
            }
        }

        if (freeBlock is null || !foundFittingBlock)
        {
            endIndex = storageBlock.StartIndex - 1;
            continue;
        }

        lowestBlockIdProcessed = storage[storageBlock.StartIndex];

        Array.Fill(storage, storage[storageBlock.StartIndex], freeBlock.StartIndex, storageBlockSize);
        Array.Fill(storage, null, storageBlock.StartIndex, storageBlockSize);

        if (logger.IsEnabled(LogEventLevel.Verbose))
            logger.Verbose("{Storage}", string.Join(" ", storage.Select(i => i is not null ? i.Value.ToString() : ".")));
    }

    if (logger.IsEnabled(LogEventLevel.Debug))
        logger.Debug("{Storage}", string.Join(" ", storage.Select(i => i is not null ? i.Value.ToString() : ".")));

    long checksum = 0;
    for (int i = 0; i < storage.Length; i++)
    {
        checksum += i * (storage[i] ?? 0);
    }

    sw.Stop();

    logger.Information("[Part2:{Filepath}] {Checksum} {ElapsedMs}ms", filepath, checksum, sw.Elapsed.TotalMilliseconds);
}


void SolvePart2_2(string filepath)
{
    var totalLength = 0;

    var input = File.ReadAllText(filepath).Trim().Select(c => (int)char.GetNumericValue(c)).ToList();

    var sw = Stopwatch.StartNew();

    var fileBlocks = new List<FileBlock>();
    var emptyBlocks = new List<EmptyBlock>();

    var id = 0;
    for (int i = 0; i < input.Count; i++)
    {
        if (i % 2 == 0)
        {
            fileBlocks.Add(new FileBlock
            {
                Id = id++,
                StartIndex = totalLength,
                Length = input[i]
            });
        }
        else
        {
            emptyBlocks.Add(new EmptyBlock
            {
                StartIndex = totalLength,
                Length = input[i]
            });
        }

        totalLength += input[i];
    }

    for (int fileIndex = fileBlocks.Count - 1; fileIndex >= 0; fileIndex--)
    {
        var fileBlock = fileBlocks[fileIndex];
        var fileBlockStart = fileBlock.StartIndex;
        var fileBlockLength = fileBlock.Length;

        for (int emptyIndex = 0; emptyIndex < emptyBlocks.Count + 1; emptyIndex++)
        {
            var emptyBlock = emptyBlocks[emptyIndex];

            if (emptyBlock.StartIndex >= fileBlockStart)
                break;

            if (emptyBlock.Length >= fileBlock.Length)
            {
                fileBlock.StartIndex = emptyBlock.StartIndex;

                emptyBlock.StartIndex += fileBlockLength;
                emptyBlock.Length -= fileBlockLength;

                break;
            }
        }
    }

    long checksum = 0;
    foreach (var fileBlock in fileBlocks)
    {
        for (int i = 0; i < fileBlock.Length; i++)
        {
            checksum += fileBlock.Id * (fileBlock.StartIndex + i);
        }
    }

    sw.Stop();

    logger.Information("[Part2_2:{Filepath}] {Checksum} {ElapsedMs}ms", filepath, checksum, sw.Elapsed.TotalMilliseconds);
}

Block GetBlock(int startIndex, long?[] storage, bool ascending)
{
    var difference = ascending ? 1 : -1;
    var targetValue = storage[startIndex];
    var endIndex = startIndex;

    for (int i = startIndex; i >= 0 && i < storage.Length; i += difference)
    {
        if (storage[i] != targetValue)
        {
            endIndex = i - difference;
            break;
        }
    }

    return ascending
        ? new Block(startIndex, endIndex)
        : new Block(endIndex, startIndex);
}

static long?[] ParseInput(string filename)
{
    var input = File.ReadAllText(filename).Trim();

    var storageSize = (long)input.Select(char.GetNumericValue).Sum();

    var storage = new long?[storageSize + 1];

    var storageIndex = 0;
    var blockIndex = 0;
    var isFree = false;
    foreach (var block in input)
    {
        var size = int.Parse(block.ToString());

        if (isFree)
        {
            Array.Fill(storage, null, storageIndex, size);
        }
        else
        {
            Array.Fill(storage, blockIndex, storageIndex, size);
            blockIndex++;
        }

        storageIndex += size;
        isFree = !isFree;
    }

    return storage;
}

record Block(int StartIndex, int EndIndex);

class FileBlock
{
    public int Id { get; set; }
    public int StartIndex { get; set; }
    public int Length { get; set; }
}

class EmptyBlock
{
    public int StartIndex { get; set; }
    public int Length { get; set; }
}