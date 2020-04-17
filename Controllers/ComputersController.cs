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

    public class ComputersController : Controller

    {

        private readonly IConfiguration _config;

        public ComputersController(IConfiguration config)

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


        // GET: Computers
        public ActionResult Index(string searchString)

        {
            using(SqlConnection conn = Connection)

            {

                conn.Open();

                using(SqlCommand cmd = conn.CreateCommand())

                {

                    cmd.CommandText = "SELECT c.Id, c.PurchaseDate, c.DecomissionDate, c.Make, c.Model, e.FirstName, e.LastName, e.ComputerId FROM Computer c LEFT JOIN Employee e ON e.ComputerId = c.Id WHERE 1 = 1";

                    if (searchString != null)
                    {
                        cmd.CommandText += " AND Make LIKE @searchString OR Model Like @searchString";
                        cmd.Parameters.Add(new SqlParameter("@searchString", "%" + searchString + "%"));
                    }

                    var reader = cmd.ExecuteReader();

                    List<ComputerCreateViewModel> computers = new List<ComputerCreateViewModel>();

                    while(reader.Read())
                    {

                        ComputerCreateViewModel computer = new ComputerCreateViewModel()
                        {

                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Model = reader.GetString(reader.GetOrdinal("Model"))

                        };

                        
                        if (!reader.IsDBNull(reader.GetOrdinal("FirstName")))

                        {

                            computer.employee = new Employee

                            {

                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),

                                LastName = reader.GetString(reader.GetOrdinal("LastName"))



                            };

                        }

                        if(!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                            {

                            computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));

                         }

                        computers.Add(computer);

                     }
                
                    reader.Close();

                    return View(computers);

                }
            }
        }

         // GET: Computers/Create
        public ActionResult Create()
        {
            var employeeOptions = GetEmployeeOptions();
            var viewModel = new ComputerCreateViewModel()
            {
                EmployeeOptions = employeeOptions
            };
            return View(viewModel);

        }

        //POST: Computers/Create
       [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create(ComputerCreateViewModel computer)
        {
            try

            {

                using (SqlConnection conn = Connection)

                {

                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())

                    {

                        cmd.CommandText = @"INSERT INTO Computer (PurchaseDate, Make, Model)
                                            OUTPUT INSERTED.Id
                                            VALUES (@PurchaseDate, @Make, @Model)";

                        cmd.Parameters.Add(new SqlParameter("@purchaseDate", computer.PurchaseDate));
                        cmd.Parameters.Add(new SqlParameter("@make", computer.Make));
                        cmd.Parameters.Add(new SqlParameter("@model", computer.Model));

                        var id = (int)cmd.ExecuteScalar();

                        computer.Id = id;
   }

                    using (SqlCommand cmd = conn.CreateCommand())

                    {

                        cmd.CommandText = @"UPDATE Employee
                                            SET ComputerId = @computerId
                                            WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", computer.EmployeeId));
                    cmd.Parameters.Add(new SqlParameter("@computerId", computer.Id));

                    cmd.ExecuteNonQuery();

                    }

                        return RedirectToAction(nameof(Index));
                 
                }
            }

            catch (Exception ex)

            {

                return View();

            }

        }


        // GET: Computers/Details/1

        public ActionResult Details(int id)
        {

            var computer = GetComputerById(id);

            return View(computer);

        }

        // GET: Computers/Delete/5
        public ActionResult Delete(int id)

        {

            var computer = GetComputerById(id);

            return View(computer);

        }

        // POST: Computers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Delete([FromRoute] int id, Computer computer)

        {
            try
            {
                using (SqlConnection conn = Connection)

                {

                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())

                    {
                        cmd.CommandText = "DELETE Computer FROM Computer LEFT JOIN Employee e ON Computer.Id = e.ComputerId WHERE e.ComputerId IS NULL AND Computer.Id = @id;";

                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));

            }

            catch (Exception ex)

            {
                return View();
            }
        }

        
        private List<SelectListItem> GetEmployeeOptions()

        {

            using (SqlConnection conn = Connection)

            {

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())

                {

                    cmd.CommandText = "SELECT Id, FirstName, LastName FROM Employee";

                    var reader = cmd.ExecuteReader();

                    var options = new List<SelectListItem>();

                    while (reader.Read())

                    {

                        var option = new SelectListItem()

                        {

                            Text = reader.GetString(reader.GetOrdinal("FirstName")) + ' ' + reader.GetString(reader.GetOrdinal("LastName")),

                            Value = reader.GetInt32(reader.GetOrdinal("Id")).ToString()

                        };

                        options.Add(option);

                    }

                    reader.Close();

                    return options;

                }

            }

        }

        private Employee GetEmployeeByComputer(int id)
        {

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, FirstName, LastName FROM Employee WHERE ComputerId = @id";

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

        private List<Employee> GetEmployees()
        {

            using (SqlConnection conn = Connection)

            {

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())

                {

                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName
                                        FROM Employee e";

                    var reader = cmd.ExecuteReader();

                    var employees = new List<Employee>();

                    while (reader.Read())

                    {

                        employees.Add(new Employee()

                        {

                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))

                        });

                    }

                    reader.Close();

                    return employees;

                }

            }

        }

        //Get a computer by Id
        private ComputerDetailViewModel GetComputerById(int id)
        {

            using (SqlConnection conn = Connection)

            {

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())

                {

                    cmd.CommandText = "SELECT c.Id, c.PurchaseDate, c.DecomissionDate, c.Make, c.Model, e.FirstName, e.LastName FROM Computer c LEFT JOIN Employee e ON e.ComputerId = c.Id WHERE c.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();

                    ComputerDetailViewModel computer = null;

                    if (reader.Read())

                    {

                        computer = new ComputerDetailViewModel()

                        {

                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Model = reader.GetString(reader.GetOrdinal("Model")),
                            
                            employee = new Employee

                            {

                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),

                            }

                        };

                            if(!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                            {
                            computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));
                         }

                    }

                    reader.Close();

                    return computer;

                }
            }
        }
    }
}