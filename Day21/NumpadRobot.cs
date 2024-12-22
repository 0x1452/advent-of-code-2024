using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Day21;
public class NumpadRobot
{
    /*
    +---+---+---+
    | 7 | 8 | 9 |
    +---+---+---+
    | 4 | 5 | 6 |
    +---+---+---+
    | 1 | 2 | 3 |
    +---+---+---+
        | 0 | A |
        +---+---+
    */
    public static readonly Dictionary<char, (int x, int y)> NumpadCoordinates = new()
    {
        { '7', (0, 0) },
        { '8', (1, 0) },
        { '9', (2, 0) },
        { '4', (0, 1) },
        { '5', (1, 1) },
        { '6', (2, 1) },
        { '1', (0, 2) },
        { '2', (1, 2) },
        { '3', (2, 2) },
        { '0', (1, 3) },
        { 'A', (2, 3) },
    };

    public char CurrentCharacter { get; set; } = 'A';
    public (int x, int y) CurrentPosition => NumpadCoordinates[CurrentCharacter];

    public List<string> GetAllNumpadMoves(IEnumerable<char> targets)
    {
        var movesList = new List<List<string>>();

        foreach (var target in targets)
        {
            movesList.Add(GetAllNumpadMoves(target));
            CurrentCharacter = target;
        }

        return GeneratePermutations(movesList);
    }

    static List<string> GeneratePermutations(List<List<string>> lists)
    {
        var result = new List<string>();
        GeneratePermutationsRecursive(lists, 0, [], result);
        return result;
    }

    static void GeneratePermutationsRecursive(List<List<string>> lists, int depth, List<string> current, List<string> result)
    {
        if (depth == lists.Count)
        {
            result.Add(string.Join("", current));
            return;
        }

        foreach (var item in lists[depth])
        {
            current.Add(item);
            GeneratePermutationsRecursive(lists, depth + 1, current, result);
            current.RemoveAt(current.Count - 1);
        }
    }

    public List<string> GetAllNumpadMoves(char target)
    {
        var (x1, y1) = NumpadCoordinates[CurrentCharacter];
        var (x2, y2) = NumpadCoordinates[target];
        var (dx, dy) = (x2 - x1, y2 - y1);

        var directions = new List<string>();

        char horizontalDirection = dx switch
        {
            0 => ' ',
            < 0 => '<',
            > 0 => '>'
        };

        char verticalDirection = dy switch
        {
            0 => ' ',
            < 0 => '^',
            > 0 => 'v'
        };

        var onlyHorizontalFirst = false;
        var onlyVerticalFirst = false;
        if (x1 == 0 && y1 + dy == 3)
        {
            onlyHorizontalFirst = true;
        }
        else if (y1 == 3 && x1 + dx == 0)
        {
            onlyVerticalFirst = true;
        }

        if (!onlyVerticalFirst && horizontalDirection != ' ')
        {
            var horizontalFirstDirections = new StringBuilder();
            horizontalFirstDirections.Append(new string(horizontalDirection, Math.Abs(dx)));
            horizontalFirstDirections.Append(new string(verticalDirection, Math.Abs(dy)));
            horizontalFirstDirections.Append('A');

            directions.Add(horizontalFirstDirections.ToString());
        }

        if (!onlyHorizontalFirst && verticalDirection != ' ')
        {
            var verticalFirstDirections = new StringBuilder();
            verticalFirstDirections.Append(new string(verticalDirection, Math.Abs(dy)));
            verticalFirstDirections.Append(new string(horizontalDirection, Math.Abs(dx)));
            verticalFirstDirections.Append('A');

            directions.Add(verticalFirstDirections.ToString());
        }

        return directions;
    }
}
