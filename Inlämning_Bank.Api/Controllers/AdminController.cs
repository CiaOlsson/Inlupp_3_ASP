using Inlämning_Bank.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inlämning_Bank.Core.Interfaces;
using Inlämning_Bank.Core.Services;

namespace Inlämning_Bank.Api.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ILoanService _loanService;
        private readonly ICustomerService _customerService;

        public AdminController(ILoanService service, ICustomerService customerService)
        {
            _loanService = service;
            _customerService = customerService;
        }

        [Route("/api/createaccount")]
        [HttpPost]
        public async Task<IActionResult> AddNewCustomerAndAccount([FromBody] NewCustomerDTO customerInfo)
        {
            if (customerInfo == null)
            {
                return BadRequest("Invalid user data");
            }

            try
            {
                await _customerService.AddCustomer(customerInfo);
                return Ok("Customer, account and login created successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error occurred {ex.Message}");
            }
        }

        [Route("/api/setuploan")]
        [HttpPost]
        public async Task<IActionResult> SetUpLoanForCustomer([FromBody] LoanDTO loan)
        {
            if (loan == null)
            {
                return BadRequest("Invalid user data");
            }
            try
            {
                await _loanService.NewLoan(loan);
            }
            catch (Exception ex)
            {
                return BadRequest($"Något gick fel!\n{ex.Message}");
            }

            return Ok("Kunden har ett nytt lån");
        }
    }
}
