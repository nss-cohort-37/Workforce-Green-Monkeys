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
        public ActionResult Index()

        {
            using(SqlConnection conn = Connection)

            {

                conn.Open();

                using(SqlCommand cmd = conn.CreateCommand())

                {

                    cmd.CommandText = "SELECT Id, PurchaseDate, DecomissionDate, Make, Model FROM Computer";

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



        // GET: Computers/Details/1

        public ActionResult Details(int id)

        {

            var computer = GetComputerById(id);

            return View(computer);

        }



        //// GET: Computers/Create

        //public ActionResult Create()

        //{

        //    var cohortOptions = GetCohortOptions();

        //    var viewModel = new ComputerEditViewmodel()

        //    {

        //        ComputerOptions = computerOptions

        //    };

        //    return View(viewModel);

        //}



        // POST: Computers/Create

        //[HttpPost]

        //[ValidateAntiForgeryToken]

        //public ActionResult Create(ComputerEditViewmodel computer)

        //{

        //    try

        //    {

        //        using (SqlConnection conn = Connection)

        //        {

        //            conn.Open();

        //            using (SqlCommand cmd = conn.CreateCommand())

        //            {

        //                cmd.CommandText = @"INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Model)

        //                                    OUTPUT INSERTED.Id

        //                                    VALUES (@firstName, @lastName, @slackHandle, @cohortId)";



        //                cmd.Parameters.Add(new SqlParameter("@purchaseDate", computer.PurchaseDate));

        //                cmd.Parameters.Add(new SqlParameter("@decomissionDate", computer.DecomissionDate));

        //                cmd.Parameters.Add(new SqlParameter("@make", computer.Make));

        //                cmd.Parameters.Add(new SqlParameter("@model", computer.Model));



        //                var id = (int)cmd.ExecuteScalar();

        //                computer.ComputerId = id;



        //                return RedirectToAction(nameof(Index));

        //            }

        //        }



                

        //    }

        //    catch (Exception ex)

        //    {

        //        return View();

        //    }

        //}






        // GET: Computers/Delete/5

        public ActionResult Delete(int id)

        {

            var computer = GetComputerById(id);

            return View(computer);

        }



        // POST: Computers/Delete/5

        [HttpPost]

        [ValidateAntiForgeryToken]

        public ActionResult DeleteComputer([FromRoute] int id)

        {

            try

            {

                using (SqlConnection conn = Connection)

                {

                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())

                    {

                        cmd.CommandText = "DELETE FROM Computer WHERE Id = @id";

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

                            DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),

                            Make = reader.GetString(reader.GetOrdinal("SlackHandle")),

                            Model = reader.GetString(reader.GetOrdinal("Model"))

                        };



                    }

                    reader.Close();

                    return computer;

                }

            }

        }

    }

}