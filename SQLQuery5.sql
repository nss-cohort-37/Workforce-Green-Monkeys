SELECT e.Id, FirstName, e.LastName, c.Make, c.Model, tp.Name
FROM Employee e
LEFT JOIN Computer c ON c.Id = e.ComputerId
LEFT JOIN EmployeeTraining et ON et.EmployeeId = e.Id
LEFT JOIN TrainingProgram tp ON et.TrainingProgramId = tp.Id
WHERE e.Id = 11

-- this is the SQL for Ticket # 3 and it works
--1.	First name and last name
--2.	Department
--3.	Currently assigned computer
--4.	Training programs they have attended, or plan on attending
