using CsvHelper;
using SirmaTask.Models;
using System.Collections.Concurrent;
using System.Globalization;

namespace SirmaTask.Services
{
    public class EmployeeProjectService
    {
        public List<PairResult> ProcessCsvFile(string filePath)
        {
            var projToEmplProj = LoadEmployeeProjectsFromCsv(filePath);

            var pairInfos = CalculateEmployeePairInfos(projToEmplProj);

            var results = BuildTopPairResultsFromPairInfos(pairInfos);

            return results;
        }

        private static List<PairResult> BuildTopPairResultsFromPairInfos(Dictionary<(int, int), PairInfo> pairInfos)
        {
            var results = new List<PairResult>();

            if (pairInfos.Any())
            {
                var maxDays = pairInfos.Max(p => p.Value.TotalDays);
                var topPairs = pairInfos.Where(p => p.Value.TotalDays == maxDays).ToList();

                results = topPairs.SelectMany(pair =>
                {
                    var (empId1, empId2) = pair.Key;
                    return pair.Value.Projects.Select(proj => new PairResult
                    {
                        EmpID1 = empId1,
                        EmpID2 = empId2,
                        ProjectID = proj.ProjectID,
                        DaysWorked = proj.Days
                    });
                }).ToList();
            }

            return results;
        }

        private static Dictionary<(int, int), PairInfo> CalculateEmployeePairInfos(Dictionary<int, List<EmployeeProject>> projToEmplProj)
        {
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

            return pairInfos;
        }

        private static Dictionary<int, List<EmployeeProject>> LoadEmployeeProjectsFromCsv(string filePath)
        {
            var projToEmplProj = new Dictionary<int, List<EmployeeProject>>();

            using (var reader = new StreamReader(filePath))
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

            return projToEmplProj;
        }
    }
}
