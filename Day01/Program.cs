var filename = "Input/input.txt";
//var filename = "Input/example.txt";

var list1 = new List<int>();
var list2 = new List<int>();

foreach (var line in File.ReadAllLines(filename))
{
    var numbers = line.Split("   ");
    list1.Add(int.Parse(numbers[0]));
    list2.Add(int.Parse(numbers[1]));
}

list1.Sort();
list2.Sort();

var sum = 0;
for (int i = 0; i < list1.Count; i++)
{
    sum += Math.Abs(list1[i] - list2[i]);
}

Console.WriteLine(sum);

var multipliers = list2
    .GroupBy(x => x)
    .ToDictionary(g => g.Key, g => g.Count());

var sum2 = list1
    .Select(n => n * multipliers.GetValueOrDefault(n))
    .Sum();

Console.WriteLine(sum2);
