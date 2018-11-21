using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BadgeBookAPI.Data;
using BadgeBookAPI.ViewModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BadgeBookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppAuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext _context;
        private readonly string DEFAULT_ROLE = "App";

        public AppAuthController(ApplicationDBContext context, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        [EnableCors("AllAccessCors")]
        [HttpPost]
        public async Task<ActionResult<string>> UserLogin([FromBody] LoginViewModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                bool isAppRole = await checkIfAppRole(user);
                
                if(!isAppRole)
                {
                    return Unauthorized();
                }
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var claim = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claim, "Token");
                    var userRoles = await _userManager.GetRolesAsync(user);

                    foreach (var role in userRoles)
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                    }

                    var signinKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));

                    //one year expire time
                    int expiryInMinutes = 525600;

                    var token = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Site"],
                        audience: _configuration["Jwt:Site"],
                        claims: claimsIdentity.Claims,
                        expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
                        signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
                    );

                    return Ok(
                        new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        });
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return Unauthorized();
        }

        /* checkIfAppRole function is used for checking if a user is part of the app role */
        public async Task<bool> checkIfAppRole(IdentityUser user)
        {
            //check if user is an app
            if (user != null)
            {
                var uRole = await _userManager.GetRolesAsync(user);
                foreach (var role in uRole)
                {
                    if (role.Equals(DEFAULT_ROLE))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}