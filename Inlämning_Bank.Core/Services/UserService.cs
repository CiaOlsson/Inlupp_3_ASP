using Inlämning_Bank.Core.Interfaces;
using Inlämning_Bank.Domain.DTO;
using Inlämning_Bank.Domain.Entities;
using Inlämning_Bank.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Inlämning_Bank.Core.Services
{
    public class UserService:IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<int> RetrieveCustomerId(ClaimsPrincipal user)
        {
            var succeeded = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out int customerId);

            if (!succeeded)
            {
                throw new Exception("Kunde inte hitta användarens kund-id");
            }

            return customerId;
        }

        public async Task AddApplicationUser(ApplicationUser userModel, Customer returnedCustomer, string password)
        {
            userModel.Customer = returnedCustomer;

            //Detta sparar användaren i databasen. 
            var result = await _userManager.CreateAsync(userModel, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(userModel, "Customer");
            }
            else
            {
                throw new Exception("Error occurred");
            }
        }

        public async Task<string> GenerateToken(UserDTO user)
        {
            List<Claim> claims = new List<Claim>();

            // Hämta användarobjektet från databasen baserat på användarnamnet
            var loggedInUser = await _userManager.Users.Include(u => u.Customer).SingleOrDefaultAsync(u => u.UserName == user.Username);

            if (loggedInUser != null)
            {
                // Först måste jag hämta användarens roller
                var userRoles = await _userManager.GetRolesAsync(loggedInUser);

                // Lägga till varje roll som en claim i JWT-tokenet
                foreach (var role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));                   
                }

                //Om användaren har en customer kopplad till sig så ska customer id hamna som claim så får den bara se information den har tillgång till. 
                if (loggedInUser.Customer != null)
                {
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, loggedInUser.Customer.CustomerId.ToString()));
                }
            }


            string key = Environment.GetEnvironmentVariable("SecretKey");

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var signInCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: "http://localhost:5249/",
                audience: "http://localhost:5249/",
                claims: claims,
                expires: DateTime.Now.AddMinutes(20),
                signingCredentials: signInCredentials
                );


            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return tokenString;
        }
    }
}
