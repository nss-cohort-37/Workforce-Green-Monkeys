 SELECT e.Id, FirstName, LastName, e.DepartmentId, d.[Name]
                    FROM Employee e
                    LEFT JOIN Department d ON e.id = d.Id 
                    WHERE Name IS NOT NULL