SELECT FirstName, LastName, d.[Name], e.DepartmentId
FROM Employee e
LEFT JOIN Department d ON e.DepartmentId = d.id