using CsvHelper;
using System.Globalization;

using (var reader = new StreamReader("data.csv"))
using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
{
    csv.Context.TypeConverterOptionsCache.GetOptions<string>().NullValues.Add("NULL");
    csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>().NullValues.Add("NULL");

    var records = csv.GetRecords<EmployeeProject>().ToList();

    foreach (var record in records)
    {
        Console.WriteLine($"EmpID: {record.EmpID}, ProjectID: {record.ProjectID}, DateFrom: {record.DateFrom.ToShortDateString()}, DateTo: {(record.DateTo.HasValue ? record.DateTo.Value.ToShortDateString() : "NULL")}");
    }


}

public class EmployeeProject
{
    public int EmpID { get; set; }
    public string ProjectID { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}