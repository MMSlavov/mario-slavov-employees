# Employee Project Analyzer

A .NET 10 WinForms application that analyzes employee project assignments to identify pairs of employees who worked together the longest across projects.

## Edge Cases & Assumptions

- **Multiple assignments to same project**: If an employee is assigned to a project multiple times, intervals are merged if they overlap. Non-overlapping assignments are all taken into account.
- **Multiple project collaborations**: Employee pairs can have multiple overlapping periods of working together on different projects, which are merged.
- **Single project constraint**: An employee can be assigned to only one project at any given time.

## Data Assumptions

- **EmpID and ProjectID**: Always integers
- **Dataset size**: Very large datasets are possible and should be handled efficiently
- **Date ranges**: Dates are inclusive on both ends (start and end dates are counted)
- **NULL dates**: Treated as current date (`DateTime.Today`)

## CSV File Format

Expected CSV format:
```
EmpID,ProjectID,DateFrom,DateTo
```

Example:
```
143,12,2013-11-01,2014-01-05
218,10,2012-05-16,null
143,10,2009-01-01,2011-04-27
```
