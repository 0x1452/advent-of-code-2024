using Day21;

SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");


void SolvePart1(string filepath)
{
    var codes = ParseInput(filepath);

    var numpadRobot = new NumpadRobot();
    var directionRobotA = new KeypadRobot();
    var directionRobotB = new KeypadRobot();

    var totalComplexity = 0;
    foreach (var code in codes)
    {
        var numpadMoves = numpadRobot.GetAllNumpadMoves(code.AsEnumerable());
        //Console.WriteLine($"[{code}:Numpad] \n{string.Join("\n", numpadMoves)}");

        var movesA = new List<string>();
        foreach (var moves in numpadMoves)
        {
            var directionMovesA = directionRobotA.GetAllKeypadMoves(moves);
            movesA.AddRange(directionMovesA);
        }
        //Console.WriteLine($"[{code}:A] \n{string.Join("\n", movesA)}");

        var movesB = new List<string>();
        foreach (var moves in movesA)
        {
            var directionMovesB = directionRobotB.GetAllKeypadMoves(moves);
            movesB.AddRange(directionMovesB);
        }
        //Console.WriteLine($"[{code}:B] \n{string.Join("\n", movesB)}");

        var lowest = movesB.MinBy(m => m.Length);

        totalComplexity += int.Parse(code[..3]) * lowest.Length;
    }

    Console.WriteLine($"[Part1:{filepath}] {totalComplexity}");
}

List<string> ParseInput(string filepath)
    => File.ReadAllLines(filepath).Select(l => l.Trim()).ToList();
