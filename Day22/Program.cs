
SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");

void SolvePart1(string filepath)
{
    var numbers = File.ReadAllLines(filepath).Select(long.Parse);

    long sum = 0;
    foreach (var number in numbers)
    {
        var number2000 = CalculateNthNumber(number, 2000);
        sum += number2000;
        //Console.WriteLine($"{number, 10}: {number2000}");
    }

    Console.WriteLine($"[Part1:{filepath}] {sum}");
    Console.WriteLine();
}

long CalculateNthNumber(long number, int n)
{
    for (int i = 0; i < n; i++)
    {
        number ^= number * 64;
        number %= 16777216;

        number ^= number / 32;
        number %= 16777216;

        number ^= number * 2048;
        number %= 16777216;
    }
    return number;
}
