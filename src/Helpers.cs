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

    public static int CalculateOverlapDays(List<(DateTime Start, DateTime End)> list1, List<(DateTime Start, DateTime End)> list2)
    {
        int totalOverlap = 0;

        foreach (var (start1, end1) in list1)
        {
            foreach (var (start2, end2) in list2)
            {
                var overlapStart = start1 > start2 ? start1 : start2;
                var overlapEnd = end1 < end2 ? end1 : end2;
                if (overlapStart <= overlapEnd)
                {
                    totalOverlap += (overlapEnd - overlapStart).Days + 1;
                }
            }
        }

        return totalOverlap;
    }
}
