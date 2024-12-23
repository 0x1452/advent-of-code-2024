// - Talked to ChatGPT to find out that this is called a "Clique Problem" in computer science
// - Used this to learn about Bron-Kerbosch: https://docs.google.com/presentation/d/16of18n90_mxCo-yToxuBkF3_ZlZpD4TJ7OmMy7PkRnM/edit

using System.Diagnostics;

SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");
SolvePart2("Input/example.txt");
SolvePart2("Input/input.txt");

void SolvePart1(string filepath)
{
    var graph = ParseInput(filepath);

    var sw = Stopwatch.StartNew();

    var targetSize = 3;
    var cliques = GetCliquesOfSize([], [.. graph.Keys], [], graph, targetSize);

    var relevant = cliques
        .Where(c => c.Any(computer => computer[0] == 't'));

    sw.Stop();

    Console.WriteLine($"[Part1:{filepath}] {relevant.Count()} {sw.Elapsed.TotalMilliseconds}ms");
}

void SolvePart2(string filepath)
{
    var graph = ParseInput(filepath);

    var sw = Stopwatch.StartNew();

    var cliques = new List<HashSet<string>>();
    GetCliques([], [.. graph.Keys], [], graph, cliques);

    var relevant = cliques
        .OrderByDescending(c => c.Count)
        .FirstOrDefault()?
        .Order();

    var password = relevant is not null
        ? string.Join(",", relevant)
        : "No password found";

    sw.Stop();

    Console.WriteLine($"[Part2:{filepath}] {password} {sw.Elapsed.TotalMilliseconds}ms");
}

// Bron Kerbosch with target size
List<HashSet<string>> GetCliquesOfSize(
    HashSet<string> currentClique,
    HashSet<string> candidates,
    HashSet<string> excluded,
    Dictionary<string, HashSet<string>> graph,
    int targetSize)
{
    if (currentClique.Count == targetSize)
        return [currentClique];

    if (candidates.Count == 0 && excluded.Count == 0)
        return [currentClique];

    var cliques = new List<HashSet<string>>();

    foreach (var candidate in candidates)
    {
        // R ∪ {v}
        var newClique = new HashSet<string>(currentClique) { candidate };

        // P ∩ N(v)
        var newCandidates = new HashSet<string>(candidates);
        newCandidates.IntersectWith(graph[candidate]);

        // X = X ∪ {v}
        var newExcluded = new HashSet<string>(excluded);
        newExcluded.IntersectWith(graph[candidate]);

        if (newClique.Count + newCandidates.Count >= targetSize)
            cliques.AddRange(GetCliquesOfSize(newClique, newCandidates, newExcluded, graph, targetSize));

        candidates.Remove(candidate);
        excluded.Add(candidate);
    }

    return cliques;
}

// Bron Kerbosch with pivot
void GetCliques(
    HashSet<string> currentClique,
    HashSet<string> candidates,
    HashSet<string> excluded,
    Dictionary<string, HashSet<string>> graph,
    List<HashSet<string>> foundCliques)
{
    if (candidates.Count == 0 && excluded.Count == 0)
        foundCliques.Add(currentClique);

    var cliques = new List<HashSet<string>>();

    var pivots = new HashSet<string>(candidates);
    pivots.UnionWith(excluded);
    var pivot = pivots.FirstOrDefault();

    // P \ N(u)
    var pivotNeighbors = pivot is not null
        ? graph[pivot]
        : [];
    var nonNeighbors = new HashSet<string>(candidates.Except(pivotNeighbors));

    foreach (var candidate in nonNeighbors)
    {
        // R ∪ {v}
        var newClique = new HashSet<string>(currentClique) { candidate };

        // P ∩ N(v)
        var newCandidates = new HashSet<string>(candidates);
        newCandidates.IntersectWith(graph[candidate]);

        // X = X ∪ {v}
        var newExcluded = new HashSet<string>(excluded);
        newExcluded.IntersectWith(graph[candidate]);

        GetCliques(newClique, newCandidates, newExcluded, graph, foundCliques);

        candidates.Remove(candidate);
        excluded.Add(candidate);
    }
}


// "Adjacency List"
Dictionary<string, HashSet<string>> ParseInput(string filepath)
{
    var graph = new Dictionary<string, HashSet<string>>();

    foreach (var line in File.ReadAllLines(filepath))
    {
        var parts = line.Split("-");

        if (!graph.ContainsKey(parts[0]))
            graph.Add(parts[0], []);

        if (!graph.ContainsKey(parts[1]))
            graph.Add(parts[1], []);

        graph[parts[0]].Add(parts[1]);
        graph[parts[1]].Add(parts[0]);
    }

    return graph;
}

