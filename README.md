Edge cases(assumptions):
⦁	If employee is assigned to a project multiple times we should merge intervals if they overlap and take into account all the assignments for an employees to a single project if they don't overlap
⦁	Pair employees can have multiple overlapping periods of working together on different projects that should be merged
⦁	Employee can be assigned to only one project at a given time

Data assuptions:
⦁	EmpID and ProjectID are allways integers
⦁	Very large datasets are possible
⦁	Dates are inclusive on both ends
