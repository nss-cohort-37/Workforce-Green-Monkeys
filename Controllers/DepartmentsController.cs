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
    public class DepartmentsController : Controller

    {
        private readonly IConfiguration _config;

        public DepartmentsController(IConfiguration config)
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



        // GET: Departments
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT d.Id, d.[Name], d.Budget, Count(DepartmentId) as 'EmployeeCount'
                                      FROM Department d
                                      LEFT JOIN Employee e on d.Id = e.DepartmentId
                                      GROUP BY d.Id, d.[Name], d.Budget";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Department> departments = new List<Department>();
                    while (reader.Read())
                    {
                        Department department = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                            EmployeeCount = reader.GetInt32(reader.GetOrdinal("EmployeeCount"))
                        };

                        departments.Add(department);
                    }

                    reader.Close();

                    return View(departments);
                }
            }

        }



        // GET: Departments/Details/1
        public ActionResult Details(int id)
        {
            var departmentById = GetDepartmentById(id);
            return View(departmentById);
        }



        // GET: Departments/Create
        public ActionResult Create()
        {
            var viewModel = new DepartmentEditViewmodel()
            {
                
            };
            return View(viewModel);
        }


        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DepartmentEditViewmodel department)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Department (Name, Budget)
                                            OUTPUT INSERTED.Id
                                            VALUES (@name, @budget)";

                        cmd.Parameters.Add(new SqlParameter("@name", department.Name));
                        cmd.Parameters.Add(new SqlParameter("@budget", department.Budget));

                        var id = (int)cmd.ExecuteScalar();
                        department.Id = id;
                        // TODO: Add insert logic here

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch
            {
                return View();
            }
        }



        // GET: Departments/Edit/1
        public ActionResult Edit(int id)
        {
            var department = GetDepartmentById(id);
            var viewModel = new DepartmentEditViewmodel()
            {
                Id = department.Id,
                Name = department.Name,
                Budget = department.Budget
            };
            return View(viewModel);
        }


        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, DepartmentEditViewmodel department)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Department 
                                            SET Name = @name, 
                                                Budget = @budget 
                                            WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@name", department.Name));
                        cmd.Parameters.Add(new SqlParameter("@budget", department.Budget));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        var rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected < 1)
                        {
                            return NotFound();
                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }



        private Department GetDepartmentById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {                     
                                      //Accommodate Sql request for data you need in Details
                    cmd.CommandText = @"SELECT d.Id, d.[Name], d.Budget, e.FirstName, e.LastName 
                                      FROM Department d
                                      LEFT JOIN Employee e ON d.Id = e.DepartmentId
                                      WHERE d.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();
                    Department department = null;

                    while (reader.Read())
                    {
                        if (department == null)
                        {
                            department = new Department()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                                DepartmentEmployees = new List<Employee>()
                                //Employee List
                            };
                        }   
                        
                        //Incorporate Employee data to list them in Details View
                        if(!reader.IsDBNull(reader.GetOrdinal("FirstName")))
                        {
                            department.DepartmentEmployees.Add(new Employee()

                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))

                            });
                        }
                    }
                    reader.Close();
                    return department;
                }
            }
        }
    }
}