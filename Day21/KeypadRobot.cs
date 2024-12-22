using System.Text;

namespace Day21;
public class KeypadRobot
{
    /*
        +---+---+
        | ^ | A |
    +---+---+---+
    | < | v | > |
    +---+---+---+
    */
    public static readonly Dictionary<char, (int x, int y)> KeypadCoordinates = new()
    {
        { '^', (1, 0) },
        { 'A', (2, 0) },
        { '<', (0, 1) },
        { 'v', (1, 1) },
        { '>', (2, 1) },
    };

    public static readonly Dictionary<char, Dictionary<char, int>> KeypadDistances = new()
    {
        ['A'] = new() { { 'A', 0 }, { '^', 1 }, { '>', 1 }, { 'v', 2 }, { '<', 3 } },
        ['^'] = new() { { '^', 0 }, { 'A', 1 }, { 'v', 1 }, { '>', 2 }, { '<', 2 } },
        ['>'] = new() { { '>', 0 }, { 'A', 1 }, { 'v', 1 }, { '^', 2 }, { '<', 2 } },
        ['v'] = new() { { 'v', 0 }, { '<', 1 }, { '^', 1 }, { '>', 2 }, { 'A', 2 } },
        ['<'] = new() { { '<', 0 }, { 'v', 1 }, { '^', 2 }, { '>', 2 }, { 'A', 3 } },
    };

    public char CurrentCharacter { get; set; } = 'A';
    public (int x, int y) CurrentPosition => KeypadCoordinates[CurrentCharacter];

    public List<string> GetAllKeypadMoves(IEnumerable<char> targets)
    {
        var movesList = new List<List<string>>();

        foreach (var target in targets)
        {
            movesList.Add(GetAllKeypadMoves(target));
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

    public List<string> GetAllKeypadMoves(char target)
    {
        var (x1, y1) = KeypadCoordinates[CurrentCharacter];
        var (x2, y2) = KeypadCoordinates[target];
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

        if (horizontalDirection == ' ' && verticalDirection == ' ')
        {
            return ["A"];
        }

        var onlyHorizontalFirst = false;
        var onlyVerticalFirst = false;
        if (x1 == 0 && y1 + dy == 0)
        {
            onlyHorizontalFirst = true;
        }
        else if (y1 == 0 && x1 + dx == 0)
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
