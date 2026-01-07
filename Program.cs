using CsvHelper;
using System.Globalization;

using (var reader = new StreamReader("data.csv"))
using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
{
    csv.Context.TypeConverterOptionsCache.GetOptions<string>().NullValues.Add("NULL");
    csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>().NullValues.Add("NULL");

    var projToEmplProj = new Dictionary<int, List<EmployeeProject>>();

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
            if(!string.IsNullOrEmpty(dateToField) && !dateToField.Equals("null", StringComparison.InvariantCultureIgnoreCase))
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

    foreach (var record in projToEmplProj)
    {
        Console.WriteLine($"ProjectID: {record.Key}");
        foreach(var empProj in record.Value)
        {
            Console.WriteLine($"\tEmpID: {empProj.EmpID}, DateFrom: {empProj.DateFrom.ToShortDateString()}, DateTo: {empProj.DateTo?.ToShortDateString()}");
        }
    }


}

public class EmployeeProject
{
    public int EmpID { get; set; }
    public int ProjectID { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}