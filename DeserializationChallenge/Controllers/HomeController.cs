using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DeserializationChallenge.Models;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DeserializationChallenge.Controllers
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

        public IActionResult Index() => View();

        public IActionResult Privacy() => View();

        [HttpPost]
        [Route("Home/Register")]
        public IActionResult Register([FromBody] dynamic visitor)
        {   
            string jsonString = visitor.ToString(Formatting.None);
            Visitor visitorObject = JsonConvert.DeserializeObject<Visitor>(jsonString, new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Auto});

            // TODO: Store the visitor in the Passive Directory database. Something is wrong with the types now...
            using (var writer = System.IO.File.AppendText("visitors.txt"))
                writer.WriteLine(jsonString);

            return Json(visitorObject);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
