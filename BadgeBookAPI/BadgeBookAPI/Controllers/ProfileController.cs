using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BadgeBookAPI.Data;
using BadgeBookAPI.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BadgeBookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext _context;

        public ProfileController(ApplicationDBContext context, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        [EnableCors("AllAccessCors")]
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> getUserProfile(string id)
        {
            APIResponse response = new APIResponse();

            try
            {
                IdentityUser currentUser = await _userManager.FindByIdAsync(id);
                if(currentUser == null)
                {
                    throw new Exception("Invalid user ID");
                }
                UserData currentUserData = this.getUserData(currentUser.Id);
                CompactIdentityUser compactDataUser = new CompactIdentityUser();
                compactDataUser.Username = currentUser.UserName;
                compactDataUser.Email = currentUser.Email;
                compactDataUser.UID = currentUser.Id;

                compactDataUser.UserData = currentUserData;
                response.Data = compactDataUser;
                return JsonConvert.SerializeObject(response);
            } catch(Exception e)
            {
                response.Message = "Error: " + e.Message;
                response.Success = false;
            }
            return JsonConvert.SerializeObject(response);
        }
        //incomplete function just a place holder...
        [EnableCors("AllAccessCors")]
        [HttpPut("setProfile/{id}")]
        public async Task<ActionResult<string>> setUserProfile(string id)
        {
            APIResponse response = new APIResponse();

            try
            {
                IdentityUser currentUser = await _userManager.FindByIdAsync(id);
                if (currentUser == null)
                {
                    throw new Exception("Invalid user ID");
                }
                UserData currentUserData = this.getUserData(currentUser.Id);
                CompactIdentityUser compactDataUser = new CompactIdentityUser();
                compactDataUser.Username = currentUser.UserName;
                compactDataUser.Email = currentUser.Email;
                compactDataUser.UID = currentUser.Id;

                compactDataUser.UserData = currentUserData;
                response.Data = compactDataUser;
                return JsonConvert.SerializeObject(response);
            }
            catch (Exception e)
            {
                response.Message = "Error: " + e.Message;
                response.Success = false;
            }
            return JsonConvert.SerializeObject(response);
        }

        /* UserData Function 
         * Input - User ID
         * Returns UserData Model of user ID
         */
        private UserData getUserData(string UID)
        {
            UserData currentUserData = _context.UserData.Where(c => c.UID.Equals(UID)).FirstOrDefault();
            Profile profileData = _context.Profile.Where(c => c.UID.Equals(UID)).FirstOrDefault();
            currentUserData.ProfileData = profileData;
            return currentUserData;
        }
    }
}