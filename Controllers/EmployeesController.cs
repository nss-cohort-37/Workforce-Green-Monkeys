using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using BangazonWorkforce.Models;
using BangazonWorkforce.Models.ViewModels;
using System.Data;

namespace BangazonWorkforce.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
        {
            _config = config;
        }


        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: Employees
        public ActionResult Index([FromQuery] string searchTerm)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT e.Id, FirstName, LastName, e.DepartmentId, d.[Name]
                    FROM Employee e
                    LEFT JOIN Department d ON  e.DepartmentId = d.Id 
                    WHERE Name IS NOT NULL";

                    var reader = cmd.ExecuteReader();
                    var employees = new List<Employee>();

                    while (reader.Read())
                    {
                        var employee = new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Department = new Department
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            }
                        };
                        employees.Add(employee);
                    }
                    reader.Close();
                    return View(employees);
                }
            }
        }

        //// GET: Employees/Details/1
        public ActionResult Details(int id)
        {
            var employee = GetEmployeeById(id);
            return View(employee);
        }

        //GET: Employees/Details/1
        //public ActionResult Details(int id)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, d.[Name] AS DeptName, c.Make, c.Model, e.Email, e.IsSupervisor, tp.Name, tp.StartDate,tp.EndDate
        //                             FROM Employee e
        //                             LEFT JOIN Department d ON d.Id = e.DepartmentId
        //                             LEFT JOIN Computer c ON c.Id = e.ComputerId
        //                             LEFT JOIN EmployeeTraining et ON et.Id = e.Id
        //                             LEFT JOIN TrainingProgram tp ON tp.Id = et.Id
        //                             WHERE e.Id = @Id";

        //            cmd.Parameters.Add(new SqlParameter("@id", id));

        //            var reader = cmd.ExecuteReader();
        //            Employee employee = null;

        //            while (reader.Read())
        //            {
        //                if (employee == null)
        //                {
        //                    employee = new Employee()
        //                    {
        //                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
        //                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
        //                        IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
        //                        Email = reader.GetString(reader.GetOrdinal("Email")),
        //                        Department = new Department
        //                        {
        //                            Name = reader.GetString(reader.GetOrdinal("DeptName"))
        //                        },
        //                        Computer = new Computer
        //                        {
        //                            Make = reader.GetString(reader.GetOrdinal("Make")),
        //                            Model = reader.GetString(reader.GetOrdinal("Model"))
        //                        },
        //                        TrainingProgram = new List<TrainingProgram>()

        //                    };
        //                }
        //                if (!reader.IsDBNull(reader.GetOrdinal("Name")))
        //                {
        //                    employee.TrainingProgram.Add(new TrainingProgram()
        //                    {
        //                        Name = reader.GetString(reader.GetOrdinal("Name")),
        //                        StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
        //                        EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate"))
        //                    });
        //                }
        //                else
        //                {
        //                    employee.TrainingProgram.Add(new TrainingProgram()
        //                    {
        //                        Name = null,
        //                        StartDate = null,
        //                        EndDate = null
        //                    });
        //                }
        //            }
        //            reader.Close();
        //            return View(employee);
        //        }
        //    }
        //}

        //this Get retrieves the FORM
        //GET: Employees/Create
        public ActionResult Create()
        {
            var departmentOptions = GetDepartmentOptions();
            var computerOptions = GetComputerOptions();
            var viewModel = new EmployeeEditViewModel()
            {
                DepartmentOptions = departmentOptions,
                ComputerOptions = computerOptions
            };
            return View(viewModel);
        }

        //this is the submit of the FORM
        //POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create(EmployeeEditViewModel employee)
             public ActionResult Create(EmployeeEditViewModel employee)
        {
            try
                //debug here
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Employee (FirstName, LastName, IsSupervisor, DepartmentId, ComputerId, Email)
                                            OUTPUT INSERTED.Id
                                            VALUES (@firstName, @lastName, @issupervisor, @departmentid, @computerid, @email)";

                        cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@issupervisor", employee.IsSupervisor));
                        cmd.Parameters.Add(new SqlParameter("@departmentid", employee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@computerid", employee.ComputerId));
                        cmd.Parameters.Add(new SqlParameter("@email", employee.Email));
                      

                        var id = (int)cmd.ExecuteScalar();
                        employee.EmployeeId = id;

                        // this sends you back to index after created
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception ex)
            {
                // debug here
                return View();
            }
        }

        //this is getting the info to build the form to assign the employee to a training program
        //GET: Employees/Assign
        public ActionResult Assign(int id)
        {
            var employee = GetEmployeeById(id);
            var programOptions = GetProgramOptions();
            var viewModel = new EmployeeTrainingAssignViewModel()
            {
                ProgramOptions = programOptions,
                Name = employee.FirstName + " " + employee.LastName,
                EmployeeId = id,
               
                
            };
            return View(viewModel);
        }

        //POST: Employees/Assign
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create(EmployeeEditViewModel employee)
        public ActionResult Assign(EmployeeTraining employeeTraining, EmployeeTrainingAssignViewModel employee)
        {
            try
            //debug here
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO EmployeeTraining (TrainingProgramId, EmployeeId)
                                            OUTPUT INSERTED.Id
                                            VALUES (@TrainingProgramId, @EmployeeId)";

                        cmd.Parameters.Add(new SqlParameter("@TrainingProgramId", employeeTraining.TrainingProgramId));
                        cmd.Parameters.Add(new SqlParameter("@EmployeeId", employee.Id));
                        

                        var id = (int)cmd.ExecuteScalar();
                        //employeeTraining.EmployeeId = id;

                        // this sends you back to index after created
                        return RedirectToAction("Details", new { employee.Id });
                    }
                }
            }
            catch (Exception ex)
            {
                // debug here
                return View();
            }
        }

        //return the FORM
        // GET: Employees/Edit/5
        public ActionResult Edit(int id)
        {
            var employee = GetEmployeeById(id);
            var DepartmentOptions = GetDepartmentOptions();
            var ComputerOptions = GetComputerOptions();
            var viewModel = new EmployeeEditViewModel()
            {

                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                DepartmentId = employee.DepartmentId,
                Email = employee.Email,
                IsSupervisor = employee.IsSupervisor,
                ComputerId = employee.ComputerId,
                DepartmentOptions = DepartmentOptions,
                ComputerOptions = ComputerOptions

                //FirstName = employee.FirstName,
                //LastName = employee.LastName,
                //DepartmentOptions = DepartmentOptions,
                //ComputerOptions = ComputerOptions
            };
            return View(viewModel);

        }




        //runs the POST
        //POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EmployeeEditViewModel employee)
              //public ActionResult Edit(int id, Employee employee)
        {
            try
            {

                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Employee
                                            SET FirstName = @firstName,
                                            LastName = @lastName, 
                                            DepartmentId = @departmentId,
                                            ComputerId = @computerId
                                            WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@departmentid", employee.DepartmentId));
                        //cmd.Parameters.Add(new SqlParameter("@name", employee.Name));
                        cmd.Parameters.Add(new SqlParameter("@computerid", employee.ComputerId));
                        //cmd.Parameters.Add(new SqlParameter("@model", employee.Model));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        var rowsAffected = cmd.ExecuteNonQuery();


                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: Employees/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    var student = GetStudentById(id);
        //    return View(student);
        //}

        // POST: Employees/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, Student student)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = Connection)
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = "DELETE FROM Student WHERE Id = @id";
        //                cmd.Parameters.Add(new SqlParameter("@id", id));

        //                cmd.ExecuteNonQuery();
        //            }
        //        }


        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (Exception ex)
        //    {
        //        return View();
        //    }
        //}


        private List<SelectListItem> GetDepartmentOptions()
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Department";

                    var reader = cmd.ExecuteReader();
                    var options = new List<SelectListItem>();

                    while (reader.Read())
                    {
                        var option = new SelectListItem()
                        {
                            Text = reader.GetString(reader.GetOrdinal("Name")),
                            Value = reader.GetInt32(reader.GetOrdinal("Id")).ToString(),
                        };

                        options.Add(option);

                    }
                    reader.Close();
                    return options;
                }
            }
        }


        private List<SelectListItem> GetComputerOptions()
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Model FROM Computer";

                    var reader = cmd.ExecuteReader();
                    var options = new List<SelectListItem>();

                    while (reader.Read())
                    {
                        var option = new SelectListItem()
                        {
                            Text = reader.GetString(reader.GetOrdinal("Model")),
                            Value = reader.GetInt32(reader.GetOrdinal("Id")).ToString(),
                        };

                        options.Add(option);

                    }
                    reader.Close();
                    return options;
                }
            }
        }

        private Employee GetEmployeeById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, FirstName, e.LastName, c.Make, c.Model, tp.Name
                                        FROM Employee e
                                        LEFT JOIN Computer c ON c.Id = e.ComputerId
                                        LEFT JOIN EmployeeTraining et ON et.EmployeeId = e.Id
                                        LEFT JOIN TrainingProgram tp ON et.TrainingProgramId = tp.Id
                                        WHERE e.Id =@id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();
                    Employee employee = null;

                    if (reader.Read())
                    {
                        employee = new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            

                        };

                    }
                    reader.Close();
                    return employee;
                }
            }





        }

        private Employee GetEmployeeById2(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id, FirstName, e.LastName, c.Make, c.Model, tp.Name. e.Computer
                                        FROM Employee e
                                        LEFT JOIN Computer c ON c.Id = e.ComputerId
                                        LEFT JOIN EmployeeTraining et ON et.EmployeeId = e.Id
                                        LEFT JOIN TrainingProgram tp ON et.TrainingProgramId = tp.Id
                                        WHERE e.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();
                    Employee employee = null;

                    if (reader.Read())
                    {
                        employee = new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            //Make = reader.GetString(reader.GetOrdinal("Make")),
                            //Model = reader.GetString(reader.GetOrdinal("Model")),
                            //Computer = Computer,
                            //Name = reader.GetString(reader.GetOrdinal("Name"))


                        };

                    }
                    reader.Close();
                    return employee;
                }
            }




        }
            private List<SelectListItem> GetProgramOptions()
            {

                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT tp.Id, tp.[Name], tp.MaxAttendees, et.TrainingProgramId, COUNT(et.EmployeeId) 
                                            FROM TrainingProgram tp
                                            LEFT JOIN EmployeeTraining et 
                                            ON tp.Id =  et.TrainingProgramId 
                                            WHERE tp.StartDate > GETDATE()
                                            AND tp.MaxAttendees > et.EmployeeId
                                            GROUP BY tp.[Name], et.TrainingProgramId, tp.Id,  tp.MaxAttendees";

                        var reader = cmd.ExecuteReader();
                        var options = new List<SelectListItem>();

                        while (reader.Read())
                        {
                            var option = new SelectListItem()
                            {
                                Text = reader.GetString(reader.GetOrdinal("Name")),
                                Value = reader.GetInt32(reader.GetOrdinal("Id")).ToString(),
                            };

                            options.Add(option);

                        }
                        reader.Close();
                        return options;
                    }
                }
            }
    }
}