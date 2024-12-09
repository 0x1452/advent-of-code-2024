using Common;
using Serilog;
using Serilog.Events;

ILogger logger = new LoggerSetup().Logger;

SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");
SolvePart2("Input/example.txt");
SolvePart2("Input/input.txt");

void SolvePart1(string filepath)
{
    long?[] storage = ParseInput(filepath);

    if (logger.IsEnabled(LogEventLevel.Debug))
        logger.Debug("{Storage}", string.Join(" ", storage.Select(i => i is not null ? i.Value.ToString() : ".")));

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

    logger.Information("[Part1:{Filepath}] {Checksum}", filepath, checksum);
}

void SolvePart2(string filepath)
{
    long?[] storage = ParseInput(filepath);

    if (logger.IsEnabled(LogEventLevel.Debug))
        logger.Debug("{Storage}", string.Join(" ", storage.Select(i => i is not null ? i.Value.ToString() : ".")));

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

        var storageBlock = GetBlock(storage[endIndex], endIndex, storage, ascending: false);
        var storageBlockSize = storageBlock.EndIndex - storageBlock.StartIndex + 1;

        Block? freeBlock = null;
        int freeBlockSize = 0;
        bool foundFittingBlock = false;

        while (!foundFittingBlock)
        {
            if (startIndex >= storageBlock.StartIndex)
                break;

            freeBlock = GetBlock(null, startIndex, storage, ascending: true);
            freeBlockSize = freeBlock.EndIndex - freeBlock.StartIndex + 1;

            if (freeBlock.StartIndex >= storageBlock.StartIndex)
                break;

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

    logger.Information("[Part2:{Filepath}] {Checksum}", filepath, checksum);
}

Block GetBlock(long? targetValue, int startIndex, long?[] storage, bool ascending)
{
    var difference = ascending ? 1 : -1;
        
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