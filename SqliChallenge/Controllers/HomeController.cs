using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SqliChallenge.Models;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace SqliChallenge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _clientFactory;

        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory, IConfiguration config)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _config = config;
        }

        public IActionResult Index()
        {
            var flag = _config.GetValue<string>("Flag", "DEFAULT_FAKE_FLAG");
            Response.Headers.Add("X-Flag", flag);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Query(string name)
        {
            if (String.IsNullOrEmpty(name)) {
                ViewData["Error"] = "Name can't be empty";
                return View();
            }

            ViewData["Query"] = name;

            List<Employee> employees = new List<Employee>();
            string queryString = "SELECT firstName, lastName, email FROM Employees WHERE firstName like @name or lastName like @name AND employeeNumber < 10000 LIMIT 5";
            SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite3");

            using (SqliteCommand cmd = new SqliteCommand(queryString, connection))
            {
                cmd.parameters.add("@name", name);
                connection.Open();

                using (SqliteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {                        
                        employees.Add(new Employee(dr.GetString(0)));
                    }
                }
            }   
            
            return View(employees);

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
