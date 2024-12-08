using Common;
using Serilog;
using Serilog.Events;

ILogger logger = new LoggerSetup().Logger;

var filename = "Input/example.txt";

SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");


void SolvePart1(string filepath)
{
    var content = File.ReadAllText(filepath);
    var grid = GridUtils.ParseGrid(content, c => c);

    if (logger.IsEnabled(LogEventLevel.Debug))
    {
        logger.Debug("{Grid}", GridUtils.GridString(grid, c => c.ToString()));
    }

    var rows = grid.GetLength(0);
    var cols = grid.GetLength(1);

    var antinodeMask = new bool[rows, cols];

    var frequencies = GetFrequencies(grid);

    foreach (var (frequency, antennas) in frequencies)
    {
        var connections = GetConnections(antennas);

        foreach (var (antenna1, antenna2) in connections)
        {
            var dx = antenna2.X - antenna1.X;
            var dy = antenna2.Y - antenna1.Y;


            var antinode1 = new Coordinates(antenna1.X - dx, antenna1.Y - dy);
            var antinode2 = new Coordinates(antenna2.X + dx, antenna2.Y + dy);

            AddAntinode(antinodeMask, antinode1);
            AddAntinode(antinodeMask, antinode2);
        }
    }

    if (logger.IsEnabled(LogEventLevel.Debug))
    {
        logger.Debug("{Antinodes}", GridUtils.GridString(antinodeMask, b => b ? "1" : "0"));
    }

    var uniqueAntinodes = antinodeMask.Cast<bool>().Count(b => b);
    logger.Information("[Part1:{Filepath}] {UniqueAntinodes}", filename, uniqueAntinodes);
}

Dictionary<char, List<Coordinates>> GetFrequencies(char[,] grid)
{
    var antennas = new Dictionary<char, List<Coordinates>>();
    var rows = grid.GetLength(0);
    var cols = grid.GetLength(1);

    for (int y = 0; y < rows; y++)
    {
        for (int x = 0; x < cols; x++)
        {
            var currentFrequency = grid[y, x];
            if (currentFrequency == '.')
                continue;

            if (!antennas.ContainsKey(currentFrequency))
                antennas.Add(currentFrequency, new List<Coordinates>());

            antennas[currentFrequency].Add(new Coordinates(x, y));
        }
    }

    return antennas;
}

IEnumerable<(Coordinates, Coordinates)> GetConnections(List<Coordinates> antennas)
{
    for (int i = 0; i < antennas.Count; i++)
    {
        for (int j = i + 1; j < antennas.Count; j++)
        {
            yield return (antennas[i], antennas[j]);
        }
    }
}

void AddAntinode(bool[,] antinodeMask, Coordinates coordinates)
{
    var rows = antinodeMask.GetLength(0);
    var cols = antinodeMask.GetLength(1);
    var isInBounds =
           coordinates.X >= 0
        && coordinates.X < cols
        && coordinates.Y >= 0
        && coordinates.Y < rows;

    if (!isInBounds)
        return;

    antinodeMask[coordinates.Y, coordinates.X] = true;
}

record struct Coordinates(int X, int Y);
