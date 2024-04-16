using AutoMapper;
using Inlämning_Bank.Core.Interfaces;
using Inlämning_Bank.Core.Services;
using Inlämning_Bank.Domain.DTO;
using Inlämning_Bank.Domain.Entities;
using Inlämning_Bank.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Inlämning_Bank.Api.Controllers
{
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;


        public AuthController(SignInManager<ApplicationUser> signInManager, ICustomerService customerService, IUserService userService, IMapper mapper)
        {
            _signInManager = signInManager;
            _userService = userService;
            _customerService = customerService;
            _mapper = mapper;
        }

        [Route("/api/signin")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserDTO user)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(user.Username, user.Password, false, false);

                if (result.Succeeded)
                {
                    var tokenString = await _userService.GenerateToken(user);

                    return Ok(tokenString);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex) 
            {
                return Conflict($"Det gick inte att logga in \n{ex.Message}");
            }

        }

        [Route("/api/customerprofile")]
        [HttpGet]
        [Authorize( Roles = "Customer" )]
        public async Task<IActionResult> ViewCustomerDetails()
        {
            try
            {
                var customerId = await _userService.RetrieveCustomerId(User);

                var customer = await _customerService.GetCustomerById(customerId);

                var customerDTO = _mapper.Map<CustomerProfileDTO>(customer);

                return Ok(customerDTO);
            }
            catch (Exception ex)
            {
                return Conflict($"Ett fel inträffade \n{ex.Message}");
            }
        }
    }
}
