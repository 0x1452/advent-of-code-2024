// - Talked to ChatGPT to find out that this is called a "Clique Problem" in computer science
// - Used this to learn about Bron-Kerbosch: https://docs.google.com/presentation/d/16of18n90_mxCo-yToxuBkF3_ZlZpD4TJ7OmMy7PkRnM/edit

SolvePart1("Input/example.txt");
SolvePart1("Input/input.txt");

void SolvePart1(string filepath)
{
    var graph = ParseInput(filepath);
    var targetSize = 3;
    var cliques = BronKerbosch([], [.. graph.Keys], [], graph, targetSize);

    var relevant = cliques
        .Where(c => c.Any(computer => computer[0] == 't'));

    //foreach (var clique in relevant)
    //{
    //    Console.WriteLine(string.Join(" ", clique));
    //}

    Console.WriteLine($"[Part1:{filepath}] {relevant.Count()}");
}

List<HashSet<string>> BronKerbosch(
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
            cliques.AddRange(BronKerbosch(newClique, newCandidates, newExcluded, graph, targetSize));

		candidates.Remove(candidate);
		excluded.Add(candidate);
	}

	return cliques;
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

