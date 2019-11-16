using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PathTraversalChallenge.Models;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace PathTraversalChallenge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            var files = Directory.GetFiles(@"./Downloads/");
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = files[i].Split('/')[2];
            }
            return View(files);
        }

        [HttpGet("download")]
        public IActionResult Download([FromQuery] string file)
        {
            var secureFile = Path.GetFileName(file);
            var net = new System.Net.WebClient();
            var data = net.DownloadData("./Downloads/"+ secureFile);
            var content = new System.IO.MemoryStream(data);
            var contentType = "APPLICATION/octet-stream";
            var fileName = secureFile;
            return File(content, contentType, fileName);
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
