using ServicesLayer.Helpers;
using Microsoft.AspNetCore.Mvc;
using ServicesLayer;
using SharedModels;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace CustomerMVC.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class CustomerController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;
        public CustomerController(HttpClient httpClient, IEmailService emailService, ILogger<AccountController> logger)
        {
            _httpClient = httpClient;
            _emailService = emailService;
            _httpClient.BaseAddress = new System.Uri("https://localhost:44354/");
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Customers");
                response.EnsureSuccessStatusCode();
                var customers = await response.Content.ReadAsAsync<IEnumerable<Customer>>();
                return View(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching customers.");
                TempData["ErrorMessage"] = "An unexpected error occurred while fetching customers. Please try again later.";
                return View();
            }
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            try
            {
                customer.Password = EncryptionHelper.Encrypt(customer.Password);
                var response = await _httpClient.PostAsJsonAsync("api/Customers/Register", customer);
                response.EnsureSuccessStatusCode();

                // Send registration email notification
                var subject = "Registration Confirmation";
                var message = $"Dear {customer.FirstName + " " + customer.LastName},<br/>Thank you for registering with us!";
                await _emailService.SendEmailAsync(customer.Email, subject, message);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating customer.");
                TempData["ErrorMessage"] = "An unexpected error occurred while creating customer. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Customers/{id}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound();
                }

                response.EnsureSuccessStatusCode();

                var customer = await response.Content.ReadAsAsync<Customer>();

                byte[] encryptedBytes = Convert.FromBase64String(customer.Password);
                customer.Password = EncryptionHelper.Decrypt(encryptedBytes);

                return View(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving customer for editing.");
                TempData["ErrorMessage"] = "An unexpected error occurred while retrieving customer for editing. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            try
            {
                if (id != customer.Id)
                {
                    return BadRequest();
                }

                var response = await _httpClient.PutAsJsonAsync($"api/Customers/{id}", customer);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogWarning($"Failed to update customer with ID {id}. HTTP status code: {response.StatusCode}");
                    TempData["ErrorMessage"] = "Failed to update customer. Please try again later.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating customer.");
                TempData["ErrorMessage"] = "An unexpected error occurred while updating customer. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Customers/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogWarning($"Failed to delete customer with ID {id}. HTTP status code: {response.StatusCode}");
                    TempData["ErrorMessage"] = "Failed to delete customer. Please try again later.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting customer.");
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting customer. Please try again later.";
                return RedirectToAction(nameof(Index));
            }

        }
    }
}
