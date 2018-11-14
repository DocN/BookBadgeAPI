﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BadgeBookAPI.Data;
using BadgeBookAPI.Models;
using BadgeBookAPI.ViewModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace BadgeBookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext _context;
        private readonly string DEFAULT_ROLE = "User";

        public AuthController(ApplicationDBContext context, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        [EnableCors("AllAccessCors")]
        [HttpPost("register")]
        public async Task<ActionResult<string>> UserRegister([FromBody] RegisterViewModel model)
        {
            APIResponse response = new APIResponse();

            try
            {
                var newUser = new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.Email
                };
                var result = await _userManager.CreateAsync(newUser, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, DEFAULT_ROLE);
                    UserData newUserData = new UserData();
                    newUserData.UID = newUser.Id;
                    newUserData.Country = model.Country;
                    newUserData.FirstName = model.FirstName;
                    newUserData.LastName = model.LastName;

                    DateTime newBirthday = new DateTime(model.BirthYear, model.BirthMonth, model.BirthDay);
                    newUserData.Birthday = newBirthday;
                    _context.UserData.Add(newUserData);
                    _context.SaveChanges();
                    response.Message = "Succesfully Registered " + newUser.UserName;
                    response.Success = true;
                    return JsonConvert.SerializeObject(response);
                }
            } catch(Exception e)
            {
                response.Message = "Failed to create user " + e.Message;
                response.Success = false;
                return JsonConvert.SerializeObject(response);
            }
            response.Message = "Failed to create user, unknown error";
            response.Success = false;
            return JsonConvert.SerializeObject(response);
        }

        [EnableCors("AllAccessCors")]
        [HttpPost("login")]
        public async Task<ActionResult<string>> UserLogin([FromBody] LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
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

                int expiryInMinutes = Convert.ToInt32(_configuration["Jwt:ExpiryInMinutes"]);

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
            return Unauthorized();
        }
    }
}