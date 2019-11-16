using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using JwtChallenge.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace JwtChallenge.Controllers
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
            Response.Cookies.Append("sessionId", GenerateJWT("guest"));

            return View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Admin()
        {
            var token = Request.Cookies["sessionId"];
            var user = ValidateJWT(token);
            if (String.IsNullOrEmpty(user))
            {
                return Unauthorized("Unauthorized");
            }
            else
            {
                if (user != "admin")
                {
                    return Unauthorized("Unauthorized");
                }
                var flag = _config.GetValue<string>("Flag", "DEFAULT_FAKE_FLAG");
                ViewData["Flag"] = flag;
                return View();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string GenerateJWT(string user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtPassword = _config.GetValue<string>("JWTPass", "DEFAULT_KEY_THAT_IS_REALLY_LONG");
            var key = Encoding.ASCII.GetBytes(jwtPassword);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, user)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private String ValidateJWT(string jwt)
        {
            var jwtPassword = _config.GetValue<string>("JWTPass");
            var key = Encoding.ASCII.GetBytes(jwtPassword);
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateAudience = false,
                RequireSignedTokens = true,
                ValidIssuer = "https://sso.binsec.cloud",
                ValidateIssuer = true
            };
            
            var validator = new JwtSecurityTokenHandler();
            
            if (validator.CanReadToken(jwt))
            {
                ClaimsPrincipal principal;
                SecurityToken validatedToken;
                JwtSecurityToken validJwt;
                
                principal = validator.ValidateToken(jwt, validationParameters, out validatedToken);
                validJwt = validatedToken as JwtSecurityToken;
                var alg = validJwt.Header.Alg.ToString();

                if (alg == "HS256" || alg == "RS256")
                {
                    return string.Empty;
                }

                if (string.IsNullOrEmpty(jwt.Split(".")[2]))
                {
                    return string.Empty;
                }

                return principal.Claims.First().Value;
            }

            return String.Empty;
        }
    }
}
