using CsvHelper;
using SirmaTask.Models;
using System.Globalization;

var projToEmplProj = new Dictionary<int, List<EmployeeProject>>();

Console.WriteLine("Please enter the path to the CSV file:");
using (var reader = new StreamReader(Path.GetFullPath(Console.ReadLine())))
using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
{
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

if (pairInfos.Any())
{
    var maxDays = pairInfos.Max(p => p.Value.TotalDays);
    var topPairs = pairInfos.Where(p => p.Value.TotalDays == maxDays).ToList();

    foreach (var pair in topPairs)
    {
        var (empId1, empId2) = pair.Key;
        var pairInfo = pair.Value;

        foreach (var (projectId, days) in pairInfo.Projects)
        {
            Console.WriteLine($"Employee ID #1: {empId1}, Employee ID #2: {empId2}, Project ID: {projectId}, Days: {days}");
        }
        Console.WriteLine();
    }
}
else
{
    Console.WriteLine("No employee pairs found who worked together on projects.");
}