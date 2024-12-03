using System.Text.RegularExpressions;

//  xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))
static void Solve(string filename)
{
    var input = File.ReadAllText(filename);
    var regex = new Regex(@"mul\((\d+),(\d+)\)|do\(\)|don't\(\)");
    var enabled = true;
    var sum = 0;

    foreach (Match match in regex.Matches(input))
    {
        switch (match.Value)
        {
            case var value when enabled && value.StartsWith("mul("):
                var a = int.Parse(match.Groups[1].Value);
                var b = int.Parse(match.Groups[2].Value);

                sum += a * b;
                break;

            case "do()":
                enabled = true;
                break;

            case "don't()":
                enabled = false;
                break;
        }
    }

    Console.WriteLine($"[{filename}]\t{sum}");
}

Solve("Input/example.txt");
Solve("Input/input.txt");
