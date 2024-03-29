﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SsrfChallenge.Models;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using HtmlAgilityPack;
using System.Text.RegularExpressions;



namespace SsrfChallenge.Controllers
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Load(string url)
        {
            if (String.IsNullOrEmpty(url)) {
                ViewData["Error"] = "The URL isn't an HTML page.";
                return View();
            }
                
            if (!url.EndsWith(".html")) {
                ViewData["Error"] = "The URL isn't an HTML page.";
                return View();
            }

            ViewData["Url"] = url;

            var flag = _config.GetValue<string>("Flag", "DEFAULT_FAKE_FLAG");
            string flagEncoded = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(flag));

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "text/html");
            request.Headers.Add("User-Agent", "SINNER");
            request.Headers.Add("Authorization", $"Bearer {flagEncoded}"); 

            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                HtmlDocument pageDocument = new HtmlDocument();
                pageDocument.Load(responseStream); 

                var vgPattern = @"https?://vg.no.*";
                var dbPattern = @"https?://db.no.*";

                HtmlNodeCollection headers = null;

                // If VG.no
                if (Regex.Match(url, vgPattern, RegexOptions.IgnoreCase).Success) {
                    headers = pageDocument.DocumentNode.SelectNodes("//h3[@itemprop='headline']");
                } 
                // If db.no
                else if (Regex.Match(url, dbPattern, RegexOptions.IgnoreCase).Success){
                    headers = pageDocument.DocumentNode.SelectNodes("//h1[@class='headline']");
                }
                // Else grab all headers
                else {
                    headers = pageDocument.DocumentNode.SelectNodes("//h1");

                }
                return View(headers);

            } else {
                ViewData["Error"] = "Could not load news site";
                return View();
            }

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
