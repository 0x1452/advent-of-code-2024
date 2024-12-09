using Common;
using Serilog;
using Serilog.Events;

ILogger logger = new LoggerSetup().Logger;

SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");
//SolvePart1("Input/debug.txt");

void SolvePart1(string filename)
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

    logger.Information("{Checksum}", checksum);
}
