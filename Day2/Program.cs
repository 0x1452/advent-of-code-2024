using Common;
using Serilog;

ILogger logger = new LoggerSetup().Logger;

logger.Information($"Debug: {GetSafeReportCount("Input/debug.txt", errorThreshold: 1)}");
logger.Information($"Example: {GetSafeReportCount("Input/example.txt", errorThreshold: 0)}");
logger.Information($"Part 1: {GetSafeReportCount("Input/input.txt", errorThreshold: 0)}");
logger.Information($"Part 2: {GetSafeReportCount("Input/input.txt", errorThreshold: 1)}");
logger.Information($"2 Errors allowed: {GetSafeReportCount("Input/input.txt", errorThreshold: 2)}");

int GetSafeReportCount(string filename, int errorThreshold)
{
    using var reader = new StreamReader(filename);

    var safeCount = 0;
    string? line;
    while ((line = reader.ReadLine()) is not null)
    {
        var report = line.Split().Select(int.Parse).ToList();

        var isSafe = IsReportSafe(report, errorThreshold);
        //var isSafe = IsReportSafeBruteforce(report);

        if (isSafe)
            safeCount++;

        var safeText = isSafe ? "SAFE" : "UNSAFE";
        logger.Debug($"{safeText}\t{line}");
    }

    return safeCount;
}

bool IsReportSafe(List<int> report, int errorThreshold)
    => IsReportSafeInOrder(report, isAscending: true, errorCount: 0, errorThreshold)
    || IsReportSafeInOrder(report, isAscending: false, errorCount: 0, errorThreshold);

bool IsReportSafeInOrder(List<int> report, bool isAscending, int errorCount, int errorThreshold)
{
    logger.Verbose($"Called IsReportSafe: [{string.Join(", ", report)}], isAscending: {isAscending}, errorCount: {errorCount}");
    for (int i = 0; i < report.Count - 1; i++)
    {
        var isMonotonic = IsMonotonic(isAscending, report[i], report[i + 1]);
        var isInSafeDistance = IsInSafeDistance(report[i], report[i + 1]);

        if (!isMonotonic || !isInSafeDistance)
        {
            errorCount++;

            if (errorCount > errorThreshold)
                return false;

            logger.Verbose(string.Join(", ", report));

            var withoutCurrent = WithoutIndex(report, i);
            if (IsReportSafeInOrder(withoutCurrent, isAscending, errorCount, errorThreshold))
                return true;

            var withoutNext = WithoutIndex(report, i + 1);
            if (IsReportSafeInOrder(withoutNext, isAscending, errorCount, errorThreshold))
                return true;

            return false;
        }
    }

    return true;
}

bool IsReportPerfectlySafe(List<int> report)
{
    var isAscending = report[0] < report[1];
    for (int i = 0; i < report.Count - 1; i++)
    {
        var isMonotonic = IsMonotonic(isAscending, report[i], report[i + 1]);
        var isInSafeDistance = IsInSafeDistance(report[i], report[i + 1]);

        if (!isMonotonic || !isInSafeDistance)
            return false;
    }

    return true;
}

bool IsReportSafeBruteforce(List<int> report)
{
    for (int i = 0; i < report.Count; i++)
    {
        var slicedReport = WithoutIndex(report, i);
        if (IsReportPerfectlySafe(slicedReport))
            return true;
    }

    return false;
}

List<int> WithoutIndex(List<int> list, int index)
    => list.Take(index).Concat(list.Skip(index + 1)).ToList();

bool IsMonotonic(bool isAscending, int first, int second)
    => isAscending
        ? first < second
        : first > second;

bool IsInSafeDistance(int first, int second)
    => Math.Abs(first - second) <= 3;
