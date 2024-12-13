using System.Text.RegularExpressions;

var buttonRegex = new Regex(@"Button [A|B]: X\+(\d+), Y\+(\d+)");
var prizeRegex = new Regex(@"Prize: X=(\d+), Y=(\d+)");

SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");

void SolvePart1(string filepath)
{
    var games = ParseInput(filepath);

    long totalTokenCost = 0;
    foreach (var game in games)
    {
        totalTokenCost += SolveGame(game);
    }

    Console.WriteLine($"[Part1:{filepath}] {totalTokenCost}");
}

long SolveGame(Game game)
{
    long aIndex = 0;
    long aX = 0;
    long aY = 0;

    long tokenCost = 0;
    while (aX <= game.PrizeX && aY <= game.PrizeY)
    {
        var complementX = game.PrizeX - aX;
        var complementY = game.PrizeY - aY;

        if (complementX % game.Bdx == 0 && complementY % game.Bdy == 0)
        {
            var bxIndex = complementX / game.Bdx;
            var byIndex = complementY / game.Bdy;

            if (bxIndex == byIndex)
            {
                tokenCost = aIndex * 3 + bxIndex;
                //Console.WriteLine($"Found match: A ({aX},{aY}), B ({complementX},{complementY}), Prize ({game.PrizeX}, {game.PrizeY}), Token Cost: {aIndex}*3 + {bxIndex} = {tokenCost}");
                return tokenCost;
            }
        }

        aIndex += 1;
        aX += game.Adx;
        aY += game.Ady;
    }

    return tokenCost;
}

List<Game> ParseInput(string filepath)
    => File.ReadAllText(filepath)
        .Split("\n\n")
        .Select(ParseGame)
        .ToList();

Game ParseGame(string game)
{
    var lines = game.Split("\n");

    var matchA = buttonRegex.Match(lines[0]);
    var adx = long.Parse(matchA.Groups[1].Value);
    var ady = long.Parse(matchA.Groups[2].Value);

    var matchB = buttonRegex.Match(lines[1]);
    var bdx = long.Parse(matchB.Groups[1].Value);
    var bdy = long.Parse(matchB.Groups[2].Value);

    var matchPrize = prizeRegex.Match(lines[2]);
    var prizeX = long.Parse(matchPrize.Groups[1].Value);
    var prizeY = long.Parse(matchPrize.Groups[2].Value);

    return new Game(adx, ady, bdx, bdy, prizeX, prizeY);
}

record Game(long Adx, long Ady, long Bdx, long Bdy, long PrizeX, long PrizeY);