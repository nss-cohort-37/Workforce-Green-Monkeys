# Bangazon Workforce side 
This is an MVC application to view and edit the database for the workforce side of the fictional Bangazon corp

## Database
To build the database you first need to run the `BangazonWorkforce.sql` query (*note we will add this file in later)

this will give you the following tables with some seed data:
 - Employees
 - Departments
 - Computers
 - TrainingPrograms
 - EmployeeTraining
 
 ## Cloning the Repo
 After building your database, use your command line to clone down this reop:
 `git clone git@github.com:nss-cohort-37/Workforce-Green-Monkeys.git`
 
 Run the project in Visual Studio
 
 ## Dependancies 
 You will need to install the following `NuGet` packages:
 
 - `Microsoft.Data.SQLclient`
 - `Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation`
 
 ## Running the Project
 
 Start your internal server and navagate to your browser. You should be seeing a dashboard with tabs to view the following endpoints:
 - Employees
 - Departments 
 - Computers 
 - TrainingPrograms
 
 These End points should have CRUD functionality and will update your local database
 
 
 
