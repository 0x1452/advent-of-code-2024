using System.Text.RegularExpressions;
using Day17;

var registerRegex = new Regex(@"Register [A|B|C]: (\d+)");

//SolvePart1("Input/example.txt");
//SolvePart1("Input/input.txt");
//SolvePart1("Input/example2.txt");

//SolvePart22("Input/input.txt");
SolvePart2_2("Input/input.txt");

void SolvePart1(string filepath)
{
    var computer = ParseInput(filepath);

    computer.Run();

    var result = string.Join(",", computer.Output);
    Console.WriteLine($"[Part1:{filepath}] {result}");
}

// This goes through 3bit chunks one-by-one -> finds a solution but it's not the lowest one
void SolvePart2(string filepath)
{
    var computer = ParseInput(filepath);

    var target = computer.Instructions.ToArray();

    // Minimum number required to get 16 numbers as result
    // -> A is processed in chunks of 3 bits, this one has 46 bit -> just enough for 15 full chunks and the least significant bit of the last chunk
    long start = 1 << 45;

    Console.WriteLine($"Start:    {Convert.ToString(start, 2).PadLeft(48, '0')}");

    // Idea now is to just bruteforce each 3 bit chunk individually

    var foundMatch = false;

    for (int c = 15; c >= 0; c--)
    {
        foundMatch = false;

        while (true)
        {
            var originalChunk = (start >> (c * 3)) & 0b111;

            // already reached max value of the chunk
            if (originalChunk == 0b111)
                break;

            // set the current chunk to 0
            start &= ~((long)0b111 << (c * 3));

            Console.WriteLine($"Cleared:  {Convert.ToString(start, 2).PadLeft(48, '0')}");

            var newChunk = originalChunk + 1;

            // replace with our new value
            start |= (newChunk << (c * 3));

            Console.WriteLine($"Replaced: {Convert.ToString(start, 2).PadLeft(48, '0')}");

            computer.Output.Clear();
            computer.A = start;
            computer.B = 0;
            computer.C = 0;
            computer.IP = 0;

            computer.Run();

            Console.WriteLine(string.Join(",", computer.Output));

            //if (computer.Output[0..(c + 1)].SequenceEqual(target[0..(c + 1)]))
            if (computer.Output.Skip(c).SequenceEqual(target.Skip(c)))
            {
                foundMatch = true;
                Console.WriteLine($"Chunk {c} is done");
                break;
            }
        }

        if (!foundMatch && c <= 14)
        {
            start &= ~(0b111 << (c * 3));
            c += 2;
        }
        else if (!foundMatch && c == 15)
        {
            Console.WriteLine("No match found");
            break;
        }
    }

    var result = string.Join(",", computer.Output);
    Console.WriteLine($"[Part2:{filepath}] {result} {start}");
}

// this bruteforces two chunks together and finds the lowest solution
void SolvePart2_2(string filepath)
{
    var computer = ParseInput(filepath);

    var target = computer.Instructions.ToArray();

    // Minimum number required to get 16 numbers as result
    // -> A is processed in chunks of 3 bits, this one has 46 bit -> just enough for 15 full chunks and the least significant bit of the last chunk
    long start = 35184372088832;

    Console.WriteLine($"Start:    {Convert.ToString(start, 2).PadLeft(48, '0')}");

    var foundMatch = false;

    for (int c = 7; c >= 0; c--)
    {
        foundMatch = false;

        while (true)
        {
            var originalChunk = (start >> (c * 6)) & 0b111111;

            // already reached max value of the chunk
            if (originalChunk == 0b111111)
                break;

            // set the current chunk to 0
            start &= ~((long)0b111111 << (c * 6));

            Console.WriteLine($"Cleared:  {Convert.ToString(start, 2).PadLeft(48, '0')}");

            var newChunk = originalChunk + 1;

            // replace with our new value
            start |= (newChunk << (c * 6));

            Console.WriteLine($"Replaced: {Convert.ToString(start, 2).PadLeft(48, '0')}");

            computer.Output.Clear();
            computer.A = start;
            computer.B = 0;
            computer.C = 0;
            computer.IP = 0;

            computer.Run();

            Console.WriteLine(string.Join(",", computer.Output));

            //if (computer.Output[0..(c + 1)].SequenceEqual(target[0..(c + 1)]))
            if (computer.Output.Skip(c*2).SequenceEqual(target.Skip(c*2)))
            {
                foundMatch = true;
                Console.WriteLine($"Chunk {c} is done");
                break;
            }
        }

        if (!foundMatch && c < 7)
        {
            start &= ~(0b111111 << (c * 6));
            c += 2;
        }
        else if (!foundMatch && c == 7)
        {
            Console.WriteLine("No match found");
            break;
        }
    }

    var result = string.Join(",", computer.Output);
    Console.WriteLine($"[Part2:{filepath}] {result} {start}");
}


ChronospatialComputer ParseInput(string filepath)
{
    var lines = File.ReadAllLines(filepath);

    var a = int.Parse(registerRegex.Match(lines[0]).Groups[1].Value);
    var b = int.Parse(registerRegex.Match(lines[1]).Groups[1].Value);
    var c = int.Parse(registerRegex.Match(lines[2]).Groups[1].Value);

    var program = lines[4].Split()[1].Split(",").Select(byte.Parse).ToList();

    return new ChronospatialComputer(a, b, c, program);
}