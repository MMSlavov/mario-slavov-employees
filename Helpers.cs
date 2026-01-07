public static class Helpers
{
    public static List<(DateTime Start, DateTime End)> MergePeriods(IEnumerable<(DateTime Start, DateTime End)> periods)
    {
        var sorted = periods.OrderBy(p => p.Start).ToList();
        var merged = new List<(DateTime, DateTime)>();
        if (!sorted.Any()) return merged;

        var current = sorted[0];
        for (int i = 1; i < sorted.Count; i++)
        {
            if (current.End >= sorted[i].Start)
            {
                current.End = DateTime.Compare(current.End, sorted[i].End) > 0 ? current.End : sorted[i].End;
            }
            else
            {
                merged.Add(current);
                current = sorted[i];
            }
        }
        merged.Add(current);
        return merged;
    }

}
