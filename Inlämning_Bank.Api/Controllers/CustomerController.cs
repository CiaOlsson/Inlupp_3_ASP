using Inlämning_Bank.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inlämning_Bank.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomerController(ICustomerService customerService)
        {
            _service = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _service.GetCustomers();
            return Ok(result);
        }
    }
}
