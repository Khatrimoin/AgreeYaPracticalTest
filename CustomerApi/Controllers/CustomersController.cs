using CustomerApi.Models;
using DataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServicesLayer;
using ServicesLayer.Helpers;
using SharedModels;

namespace CustomerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly AppDbContext _context;
        private readonly ILogger<CustomersController> _logger;
        public CustomersController(ICustomerService customerService, AppDbContext context, ILogger<CustomersController> logger)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all customers.
        /// </summary>
        /// <returns>A list of customers.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching customers.");
                return StatusCode(500, "Internal server error occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Retrieves a customer by their ID.
        /// </summary>
        /// <param name="id">The ID of the customer to retrieve.</param>
        /// <returns>The requested customer.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);

                if (customer == null)
                {
                    return NotFound();
                }

                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching customer with ID {id}.");
                return StatusCode(500, "Internal server error occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Logs in a customer with provided credentials.
        /// </summary>
        /// <param name="model">The login request containing email and password.</param>
        /// <returns>A message indicating success or failure of the login attempt.</returns>
        [HttpPost("Login")]
        public IActionResult Login(LoginRequest model)
        {
            try
            {
                var user = _context.Customers.FirstOrDefault(u => u.LoginUser == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    _logger.LogInformation("SucessFull Login.");
                    return Ok(new { message = "Login successful" });
                }
                else
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing login request.");
                return StatusCode(500, "Internal server error occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Registers a new customer.
        /// </summary>
        /// <param name="customer">The customer to be registered.</param>
        /// <returns>The newly registered customer.</returns>
        [HttpPost("Register")]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            try
            {
                await _customerService.AddCustomerAsync(customer);

                return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding customer.");
                return StatusCode(500, "Internal server error occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Updates an existing customer.
        /// </summary>
        /// <param name="id">The ID of the customer to be updated.</param>
        /// <param name="customer">The updated customer object.</param>
        /// <returns>An action result indicating success or failure.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            try
            {
                if (id != customer.Id)
                {
                    return BadRequest();
                }

                await _customerService.UpdateCustomerAsync(customer);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating customer with ID {id}.");
                return StatusCode(500, "Internal server error occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Deletes a customer by their ID.
        /// </summary>
        /// <param name="id">The ID of the customer to be deleted.</param>
        /// <returns>An action result indicating success or failure.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                await _customerService.DeleteCustomerAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting customer with ID {id}.");
                return StatusCode(500, "Internal server error occurred. Please try again later.");
            }
        }
    }
}
