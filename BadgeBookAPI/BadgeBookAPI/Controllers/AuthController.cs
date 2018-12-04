using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BadgeBookAPI.Data;
using BadgeBookAPI.Models;
using BadgeBookAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
        private readonly string NAME_IDEN_TOKEN = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

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
                    Profile ProfileData = new Profile();
                    ProfileData.UID = newUser.Id;
                    ProfileData.Description = "";
                    newUserData.ProfileData = ProfileData;
                    DateTime newBirthday = new DateTime(model.BirthYear, model.BirthMonth, model.BirthDay);
                    newUserData.Birthday = newBirthday;
                    _context.Profile.Add(ProfileData);
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
            try
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                bool isApp = await checkIfAppRole(user);
                if(isApp)
                {
                    return Unauthorized();
                }
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var claim = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                        new Claim(JwtRegisteredClaimNames.NameId, user.Id),
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
            } catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return Unauthorized();
        }

        [EnableCors("AllAccessCors")]
        [Authorize(Roles = "User")]
        [HttpPost("changePassword")]
        public async Task<string> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            APIResponse response = new APIResponse();
            try
            {
                var tokenClaims = User.Claims.Select(c =>
                    new
                    {
                        Type = c.Type,
                        Value = c.Value
                    });

                var username = tokenClaims.Where(c=>c.Type.Equals(NAME_IDEN_TOKEN)).FirstOrDefault().Value;
                var currentUser = await _userManager.FindByNameAsync(username);
                var result = await _userManager.ChangePasswordAsync(currentUser, model.OldPassword, model.NewPassword);
                if(result.Succeeded)
                {
                    response.Message = "Successfully change password for user " + username;
                    response.Success = true;
                }
                else
                {
                    throw new Exception("Invalid old password");
                }
            } catch(Exception e)
            {
                response.Message = e.Message;
                response.Success = false;
            }
            return JsonConvert.SerializeObject(response);
        }

        [EnableCors("AllAccessCors")]
        [HttpPost("resetPassword")]
        public async Task<string> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            APIResponse response = new APIResponse();
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await SendAsync(model.Email, resetToken);
                }
            } catch(Exception e)
            {

            }
            response.Message = "If the account exists a confirmation email was sent";
            response.Success = true;
            return JsonConvert.SerializeObject(response);
        }
        public Task SendAsync(string email, string resetToken)
        {
            var client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                Credentials = new NetworkCredential("badgebookme@gmail.com", "passpassword123"),
                EnableSsl = true,
            };

            var @from = new MailAddress("badgebookme@gmail.com", "Badge Book Recovery email no reply");
            var to = new MailAddress(email);

            var mail = new MailMessage(@from, to)
            {
                Subject = "Your Badge Book Recovery Token",
                Body = "Paste this token into the recovery page to reset your password: <br> <br>" + resetToken,
                IsBodyHtml = true,
            };

            client.Send(mail);

            return Task.FromResult(0);
        }

        [EnableCors("AllAccessCors")]
        [HttpPost("resetPasswordwtoken")]
        public async Task<string> ResetPasswordWithToken([FromBody] ResetPasswordWTokenViewModel model)
        {
            APIResponse response = new APIResponse();
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                    if (result.Succeeded)
                    {
                        response.Message = "Password Sucessfully Reset";
                        response.Success = true;
                        return JsonConvert.SerializeObject(response);
                    }
                }
            } catch(Exception e)
            {

            }
            response.Message = "Failed to reset password";
            response.Success = false;
            return JsonConvert.SerializeObject(response);
        }

        /* checkIfAppRole function is used for checking if a user is part of the app role */
        public async Task<bool> checkIfAppRole(IdentityUser user)
        {
            //check if user is an app
            if(user != null)
            {
                var uRole = await _userManager.GetRolesAsync(user);
                foreach (var role in uRole)
                {
                    if (role.Equals("App"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


    }
}
 