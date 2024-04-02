using ServicesLayer.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedModels;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using CustomerMVC.Models;

namespace CustomerMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;

        private string _apiBaseUrl = string.Empty; // Update with your API base URL
        public AccountController(HttpClient httpClient, ILogger<AccountController> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
        }
        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _apiBaseUrl = "https://localhost:44354/api/Customers/Login";
                    model.Password = EncryptionHelper.Encrypt(model.Password);

                    var customerJson = JsonConvert.SerializeObject(model);
                    var content = new StringContent(customerJson, Encoding.UTF8, "application/json");

                    using (var response = await _httpClient.PostAsync(_apiBaseUrl, content))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string UserRole = "User";
                            if (model.Email == "gk@gmail.com")
                                UserRole = "Admin";
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, model.Email),
                                new Claim(ClaimTypes.Role, UserRole)
                            };

                            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                            var jwtToken = new JwtSecurityToken(
                                issuer: _configuration["Jwt:Issuer"],
                                audience: _configuration["Jwt:Issuer"],
                                claims: claims,
                                expires: DateTime.Now.AddMinutes(30),
                                signingCredentials: creds);
                            var encodedToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                            HttpContext.Session.SetString("JWTToken", encodedToken);
                            HttpContext.Session.SetString("IsLoggedIn", "true");
                            HttpContext.Session.SetString("Role", UserRole);

                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Login failed. Please try again.");
                        }
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing login.");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                return View(model);
            }
        }



        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var Password = EncryptionHelper.Encrypt(model.Password);

                    var customer = new Customer
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Password = Password,
                        LoginUser = model.LoginUser,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber
                    };
                    _apiBaseUrl = "https://localhost:44354/api/Customers/Register";

                    var customerJson = JsonConvert.SerializeObject(customer);
                    var content = new StringContent(customerJson, Encoding.UTF8, "application/json");

                    using (var response = await _httpClient.PostAsync(_apiBaseUrl, content))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return RedirectToAction("Login", "Account");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Registration failed. Please try again.");
                        }
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing registration.");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                return View(model);
            }
        }


    }
}
