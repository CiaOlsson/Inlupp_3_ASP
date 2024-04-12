using Inlämning_Bank.Core.Interfaces;
using Inlämning_Bank.Core.Services;
using Inlämning_Bank.Domain.DTO;
using Inlämning_Bank.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Inlämning_Bank.Api.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICustomerService _customerService;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ICustomerService customerService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _customerService = customerService;
        }



        [Route("/api/createaccount")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddNewCustomerAndAccount([FromBody] NewCustomerInfoDTO customerInfo)
        {
            if (customerInfo == null)
            {
                return BadRequest("Invalid user data");
            }

            try
            {
                await _customerService.AddCustomer(customerInfo);
                return Ok("Customer and account created successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error occurred {ex.Message}");
            }


        }

        [Route("/api/signin")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserDTO user)
        {
            var result = await _signInManager.PasswordSignInAsync(user.Username, user.Password, false, false);

            if (result.Succeeded)
            {
                //List<Claim> claims = new List<Claim>();
                //claims.Add(new Claim(ClaimTypes.Role, "Customer"));


                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mysecretKey12345!#123456789101112"));

                var signInCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var tokenOptions = new JwtSecurityToken(
                    issuer: "http://localhost:5166/",
                    audience: "http://localhost:5166/",
                    //claims: claims,
                    expires: DateTime.Now.AddMinutes(20),
                    signingCredentials: signInCredentials
                    );


                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                return Ok(tokenString);
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
