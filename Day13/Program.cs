using System.Diagnostics;
using System.Text.RegularExpressions;

var buttonRegex = new Regex(@"Button [A|B]: X\+(\d+), Y\+(\d+)");
var prizeRegex = new Regex(@"Prize: X=(\d+), Y=(\d+)");

SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");
SolvePart2("Input/example.txt");
SolvePart2("Input/input.txt");
SolvePart2Cramer("Input/example.txt");
SolvePart2Cramer("Input/input.txt");

void SolvePart1(string filepath)
{
    var games = ParseInput(filepath);

    var sw = Stopwatch.StartNew();
    long totalTokenCost = 0;
    foreach (var game in games)
    {
        totalTokenCost += SolveGame(game);
    }
    sw.Stop();

    Console.WriteLine($"[Part1:{filepath}] {totalTokenCost} {sw.Elapsed.TotalMilliseconds}ms");
}

void SolvePart2(string filepath)
{
    var games = ParseInput(filepath);

    var sw = Stopwatch.StartNew();
    long totalTokenCost = 0;
    foreach (var game in games)
    {
        game.PrizeX += 10000000000000;
        game.PrizeY += 10000000000000;
        totalTokenCost += SolveGame2(game);
    }
    sw.Stop();

    Console.WriteLine($"[Part2:{filepath}] {totalTokenCost} {sw.Elapsed.TotalMilliseconds}ms");
}

void SolvePart2Cramer(string filepath)
{
    var games = ParseInput(filepath);

    var sw = Stopwatch.StartNew();
    long totalTokenCost = 0;
    foreach (var game in games)
    {
        game.PrizeX += 10000000000000;
        game.PrizeY += 10000000000000;
        totalTokenCost += SolveGameCramer(game);
    }
    sw.Stop();

    Console.WriteLine($"[Part2Cramer:{filepath}] {totalTokenCost} {sw.Elapsed.TotalMilliseconds}ms");
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

// This one I kind of reached myself after understanding that this is a system of equations. I needed some help from ChatGPT though.
// Still struggled for a while, because I didn't realize that I forgot to check whether `a` was an integer.
// I also don't really understand if there was an easy way to see that all the systems of equations only have one solution...
// I'm not really satisfied with my understanding of this puzzle.
long SolveGame2(Game game)
{
    /* 
        Button A: X+94, Y+34
        Button B: X+22, Y+67
        Prize: X=8400, Y=5400

        -- Formulate as system of equations
        (I)  94*a + 22*b = 8400
        (II) 34*a + 67*b = 5400

        -- Align `a` -> multiply I) with 34 and II) with 94
        (I)  34*(94*a + 22*b) = 34*8400  =>  3196*a +  748*b = 285600
        (II) 94*(34*a + 67*b) = 94*5400  =>  3196*a + 6298*b = 507600

        -- Substract both equations
        3196*a + 748*b - 3196*a - 6298*b = 285600 - 507600
        748*b - 6298*b = 285600 - 507600                       // Ady*Bdx*b - Adx*Bdy*b = Ady*PrizeX - Adx*PriceY
        -5550*b = -222000
        b = -222000 / -5550
        b = 40
        
        -- Get `a` by inserting in (I)
        a = (8400 - 22*40) / 94
        a = 80
    */

    var numerator = game.PrizeX * game.Ady - game.PrizeY * game.Adx;
    var denominator = game.Ady * game.Bdx - game.Adx * game.Bdy;

    if (denominator == 0 || numerator % denominator != 0)
        return 0;

    var bCount = numerator / denominator;

    var aNumerator = game.PrizeX - bCount * game.Bdx;
    if (aNumerator % game.Adx != 0)
        return 0;

    var aCount = aNumerator / game.Adx;

    return 3 * aCount + bCount;
}

long SolveGameCramer(Game game)
{
    long a1 = game.Adx, b1 = game.Bdx, c1 = game.PrizeX;
    long a2 = game.Ady, b2 = game.Bdy, c2 = game.PrizeY;

    long det = a1 * b2 - b1 * a2;

    if (det == 0) 
        return 0; 

    long numeratorX = c1 * b2 - b1 * c2;
    long numeratorY = a1 * c2 - c1 * a2;

    if (numeratorX % det != 0 || numeratorY % det != 0)
        return 0;

    long x = numeratorX / det;
    long y = numeratorY / det;

    return 3 * x + y;
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

class Game(long Adx, long Ady, long Bdx, long Bdy, long PrizeX, long PrizeY)
{
    public long Adx { get; set; } = Adx;
    public long Ady { get; set; } = Ady;
    public long Bdx { get; set; } = Bdx;
    public long Bdy { get; set; } = Bdy;
    public long PrizeX { get; set; } = PrizeX;
    public long PrizeY { get; set; } = PrizeY;
}