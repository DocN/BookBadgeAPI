using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BadgeBookAPI.Data;
using BadgeBookAPI.Models;
using BadgeBookAPI.ViewModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BadgeBookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonalProfileController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext _context;
        private readonly string USER_NAME_IDENT = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        public PersonalProfileController(ApplicationDBContext context, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        [EnableCors("AllAccessCors")]
        [HttpPost("editUser")]
        public async Task<ActionResult<string>> EditUserData([FromBody] EditUserProfileViewModel model)
        {
            APIResponse response = new APIResponse();
            try
            {
                var currentUsername = User.Claims.Where(c => c.Type.Equals(USER_NAME_IDENT)).FirstOrDefault().Value;
                var currentUser = await _userManager.FindByNameAsync(currentUsername);
                if(currentUser != null)
                {
                    UserData currentUserData = _context.UserData.Where(c => c.UID.Equals(currentUser.Id)).FirstOrDefault();
                    if(currentUserData == null)
                    {
                        currentUserData = new UserData();
                    }
                    currentUserData.FirstName = model.FirstName;
                    currentUserData.LastName = model.LastName;
                    currentUserData.Country = model.Country;
                    DateTime newBirthday = new DateTime(model.BirthYear, model.BirthMonth, model.BirthDay);
                    currentUserData.Birthday = newBirthday;
                    _context.UserData.Update(currentUserData);
                    _context.SaveChanges();
                    response.Message = "Successfully updated user profile";
                    response.Success = true;
                    return JsonConvert.SerializeObject(response);
                }
            }catch(Exception e)
            {
                response.Message = "Internal Error " + e.Message;
                response.Success = false;
                return JsonConvert.SerializeObject(response);
            }
            response.Message = "Unknown error Unable to edit user data";
            response.Success = false;
            return JsonConvert.SerializeObject(response);
        }

        [EnableCors("AllAccessCors")]
        [HttpPost("editDescription")]
        public async Task<ActionResult<string>> EditProfileDescription([FromBody] EditProfileDescriptionViewModel model)
        {
            APIResponse response = new APIResponse();
            try
            {
                var currentUsername = User.Claims.Where(c => c.Type.Equals(USER_NAME_IDENT)).FirstOrDefault().Value;
                var currentUser = await _userManager.FindByNameAsync(currentUsername);
                if (currentUser != null)
                {
                    Profile currentProfile = _context.Profile.Where(c => c.UID.Equals(currentUser.Id)).FirstOrDefault();
                    if (currentProfile == null)
                    {
                        currentProfile = new Profile();
                    }
                    currentProfile.Description = model.Description;
                    _context.Profile.Update(currentProfile);
                    _context.SaveChanges();
                    response.Message = "Successfully updated user profile description";
                    response.Success = true;
                    return JsonConvert.SerializeObject(response);
                }
            }
            catch (Exception e)
            {
                response.Message = "Internal Error " + e.Message;
                response.Success = false;
                return JsonConvert.SerializeObject(response);
            }
            response.Message = "Unknown error Unable to edit profile description";
            response.Success = false;
            return JsonConvert.SerializeObject(response);
        }

        [EnableCors("AllAccessCors")]
        [HttpGet("claims")]
        public object Claims()
        {
            return User.Claims.Select(c =>
            new
            {
                Type = c.Type,
                Value = c.Value
            });
        }

    }
}