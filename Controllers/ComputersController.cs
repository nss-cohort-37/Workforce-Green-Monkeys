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

                    cmd.CommandText = "SELECT Id, PurchaseDate, DecomissionDate, Make, Model FROM Computer WHERE 1 = 1";
                    if (searchString != null)
                    {
                        cmd.CommandText += " AND Make LIKE @searchString OR Model Like @searchString";
                        cmd.Parameters.Add(new SqlParameter("@searchString", "%" + searchString + "%"));
                    }
                    var reader = cmd.ExecuteReader();

                    var computers = new List<Computer>();

                    while(reader.Read())
                    {

                        var computer = new Computer()
                        {

                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Model = reader.GetString(reader.GetOrdinal("Model"))

                        };

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
            var viewModel = new ComputerCreateViewmodel()
            {
                EmployeeOptions = employeeOptions
            };
            return View(viewModel);

        }

        //POST: Computers/Create
       [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Create(Computer ComputerCreateViewModel)
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

                        cmd.Parameters.Add(new SqlParameter("@purchaseDate", ComputerCreateViewModel.PurchaseDate));
                        cmd.Parameters.Add(new SqlParameter("@make", ComputerCreateViewModel.Make));
                        cmd.Parameters.Add(new SqlParameter("@model", ComputerCreateViewModel.Model));

                        var id = (int)cmd.ExecuteScalar();

                        ComputerCreateViewModel.Id = id;

                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            catch (Exception ex)

            {

                return View(ComputerCreateViewModel);

            }

        }

        // GET: Computers/Details/1
        public ActionResult Details(int id)

        {

            var computer = GetComputerById(id);
            var EmployeesComputers = GetEmployeeByComputer(id);
            var viewModel = new ComputerDetailViewModel()
            {
                Id = id
            };
            return View(viewModel);

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

                            Text = reader.GetString(reader.GetOrdinal("FirstName")),
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
                    cmd.CommandText = "SELECT Id, FirstName FROM Employee WHERE ComputerId = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();
                    Employee employee = null;

                    if (reader.Read())
                    {
                        employee = new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        };

                    }
                    reader.Close();
                    return employee;
                }
            }
            
        }

        private Computer GetComputerById(int id)

        {

            using (SqlConnection conn = Connection)

            {

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())

                {

                    cmd.CommandText = "SELECT Id, PurchaseDate, DecomissionDate, Make, Model FROM Computer WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();

                    Computer computer = null;

                    if (reader.Read())

                    {

                        computer = new Computer()

                        {

                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Model = reader.GetString(reader.GetOrdinal("Model"))

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