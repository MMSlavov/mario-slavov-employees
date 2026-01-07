using CsvHelper;
using SirmaTask.Models;
using System.Globalization;

var projToEmplProj = new Dictionary<int, List<EmployeeProject>>();

using (var reader = new StreamReader("TestData/data2.csv"))
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

foreach (var (projId, assignments) in projToEmplProj)
{
    var emplPeriods = assignments.GroupBy(a => a.EmpID)
        .ToDictionary(g => g.Key, g => Helpers.MergePeriods(g.Select(a => (a.DateFrom, a.DateTo))));

    foreach(var emp1 in emplPeriods)
    {
        Console.WriteLine($"Processing employee {emp1.Key} in project {projId}");
        foreach(var period in emp1.Value)
        {
            Console.WriteLine($"  Period: {period.Start.ToShortDateString()} - {period.End.ToShortDateString()}");
        }
    }
}