using CsvHelper;
using SirmaTask.Models;
using System.Globalization;

var projToEmplProj = new Dictionary<int, List<EmployeeProject>>();

using (var reader = new StreamReader("TestData/data3.csv"))
using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
{
    csv.Context.TypeConverterOptionsCache.GetOptions<string>().NullValues.Add("NULL");
    csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>().NullValues.Add("NULL");


    while (csv.Read())
    {
        try
        {
            var dateFromField = csv.GetField<string>(2);
            if (!DateTime.TryParse(dateFromField, out var dateFrom))
            {
                throw new Exception("Invalid date format in DateFrom field.");
            }

            var dateToField = csv.GetField<string>(3);
            var dateTo = DateTime.Today;
            if (!string.IsNullOrEmpty(dateToField) && !dateToField.Equals("null", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!DateTime.TryParse(dateToField, out dateTo))
                {
                    throw new Exception("Invalid date format in DateTo field.");
                }
            }

            var record = new EmployeeProject
            {
                EmpID = csv.GetField<int>(0),
                ProjectID = csv.GetField<int>(1),
                DateFrom = dateFrom,
                DateTo = dateTo,
            };

            if (!projToEmplProj.ContainsKey(record.ProjectID))
            {
                projToEmplProj[record.ProjectID] = new List<EmployeeProject>();
            }

            projToEmplProj[record.ProjectID].Add(record);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing record: {ex.Message}");
        }
    }

}

var pairInfos = new Dictionary<(int, int), PairInfo>();

foreach (var (projId, assignments) in projToEmplProj)
{
    var emplProjPeriods = assignments.GroupBy(a => a.EmpID)
        .ToDictionary(g => g.Key, g => Helpers.MergePeriods(g.Select(a => (a.DateFrom, a.DateTo))));

    var emplIds = emplProjPeriods.Keys;
    foreach (var empId1 in emplIds)
    {
        foreach (var empId2 in emplIds)
        {
            if (empId1 >= empId2) continue;
            
            var overlapDays = Helpers.CalculateOverlapDays(emplProjPeriods[empId1], emplProjPeriods[empId2]);

            if (overlapDays > 0)
            {
                var pairKey = (empId1, empId2);
                if (!pairInfos.ContainsKey(pairKey))
                {
                    pairInfos[pairKey] = new PairInfo();
                }

                pairInfos[pairKey].TotalDays += overlapDays;
                pairInfos[pairKey].Projects.Add((projId, overlapDays));
            }
        }
    }
}

foreach (var pair in pairInfos.OrderBy(p => p.Key.Item1).ThenBy(p => p.Key.Item2))
{
    var (empId1, empId2) = pair.Key;
    var info = pair.Value;
    Console.WriteLine($"Employees {empId1} and {empId2} for {info.TotalDays}:");
    foreach (var (projId, days) in info.Projects)
    {
        Console.WriteLine($"  Project {projId}: {days}");
    }
}