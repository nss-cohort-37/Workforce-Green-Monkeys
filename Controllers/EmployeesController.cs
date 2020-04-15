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
                    LEFT JOIN Department d ON e.id = d.Id 
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


        // return the FORM
        //// GET: Employees/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    var student = GetStudentById(id);
        //    var cohortOptions = GetCohortOptions();
        //    var viewModel = new StudentEditViewModel()
        //    {
        //        StudentId = student.Id,
        //        FirstName = student.FirstName,
        //        LastName = student.LastName,
        //        CohortId = student.CohortId,
        //        SlackHandle = student.SlackHandle,
        //        CohortOptions = cohortOptions
        //        // use a helper method GetCohortOptions to get cohorts below


        //    };
        //    return View(viewModel);
        //}


        // runs the POST
        // POST: Employees/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, StudentEditViewModel student)
        //{
        //    try
        //    {

        //        using (SqlConnection conn = Connection)
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = @"UPDATE Student
        //                                 SET FirstName = @firstName,
        //                                     LastName = @lastName,
        //                                     SlackHandle =@slackhandle,
        //                                     CohortID = @cohortId
        //                                        WHERE Id = @id";


        //                cmd.Parameters.Add(new SqlParameter("@firstName", student.FirstName));
        //                cmd.Parameters.Add(new SqlParameter("@lastName", student.LastName));
        //                cmd.Parameters.Add(new SqlParameter("@slackHandle", student.SlackHandle));
        //                cmd.Parameters.Add(new SqlParameter("@cohortId", student.CohortId));
        //                cmd.Parameters.Add(new SqlParameter("@id", id));

        //                var rowsAffected = cmd.ExecuteNonQuery();


        //            }
        //        }

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

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
                    cmd.CommandText = @"SELECT Id, FirstName, LastName
                        FROM Employee
                        WHERE Id = @Id;";

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
    }
}