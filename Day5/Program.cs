using Common;

var logger = new LoggerSetup().Logger;

SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");
SolvePart2("Input/example.txt");
SolvePart2("Input/input.txt");

void SolvePart1(string filename)
{
    var content = File.ReadAllText(filename);

    var parts = content.Split("\n\n");
    var orderingRuleInput = parts[0];
    var updateInput = parts[1];

    Dictionary<int, HashSet<int>> rules = ParseRules(orderingRuleInput);
    List<List<int>> updates = ParseUpdates(updateInput);

    var sum = 0;
    foreach (var update in updates)
    {
        var isValid = IsValid(update, rules);

        if (isValid)
        {
            var middleIndex = update.Count / 2;

            sum += update[middleIndex];

            logger.Verbose($"{string.Join(", ", update)}: {update[middleIndex]}");
        }
    }

    logger.Information("Part1 {Filename}: {Sum}", filename, sum);
}

void SolvePart2(string filename)
{
    var content = File.ReadAllText(filename);

    var parts = content.Split("\n\n");
    var orderingRuleInput = parts[0];
    var updateInput = parts[1];

    Dictionary<int, HashSet<int>> rules = ParseRules(orderingRuleInput);
    List<List<int>> updates = ParseUpdates(updateInput);

    var sum = 0;
    foreach (var update in updates)
    {
        var isValid = IsValid(update, rules);

        if (!isValid)
        {
            update.Sort((current, next) =>
            {
                if (current == next)
                    return 0;

                rules.TryGetValue(current, out var numbersBefore);

                var currentIsHigher = numbersBefore?.Contains(next) ?? true;

                return currentIsHigher ? 1 : -1;
            });

            var middleIndex = update.Count / 2;

            sum += update[middleIndex];

            logger.Verbose($"{string.Join(", ", update)}: {update[middleIndex]}");
        }
    }

    logger.Information("Part2 {Filename}: {Sum}", filename, sum);

}

(int Key, int Before) ParseRule(string rule)
{
    var parts = rule.Split("|").Select(int.Parse).ToList();
    return (parts[0], parts[1]);
}

Dictionary<int, HashSet<int>> ParseRules(string orderingRuleInput)
{
    var rules = new Dictionary<int, HashSet<int>>();

    foreach (var rule in orderingRuleInput.Split("\n").Select(ParseRule))
    {
        if (!rules.ContainsKey(rule.Key))
            rules.Add(rule.Key, []);

        rules[rule.Key].Add(rule.Before);
    }

    return rules;
}

List<List<int>> ParseUpdates(string updateInput)
{
    var updates = new List<List<int>>();
    var updateLines = updateInput.Split("\n", StringSplitOptions.RemoveEmptyEntries);

    foreach (var line in updateLines)
    {
        var update = line
            .Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToList();

        updates.Add(update);
    }

    return updates;
}

bool IsValid(List<int> update, Dictionary<int, HashSet<int>> rules)
{
    for (int i = update.Count - 1; i >= 1; i--)
    {
        var currentNumber = update[i];

        for (int j = i - 1; j >= 0; j--)
        {
            var compareNumber = update[j];

            rules.TryGetValue(currentNumber, out var rule);
            if (rule?.Contains(compareNumber) == true)
            {
                return false;
            }
        }
    }

    return true;
}
