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

    public class SearchController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext _context;

        public SearchController(ApplicationDBContext context, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        [EnableCors("AllAccessCors")]
        [HttpPost]
        public async Task<ActionResult<string>> GetSearchQueueAsync([FromBody] SearchViewModel model)
        {
            APIResponse response = new APIResponse();
            if (model.Keywords != null)
            {
                string[] keywords = model.Keywords.Split(new char[0]);
                var listOfProfiles = _context.Profile.ToList();
                var validProfiles = new List<CompactIdentityUser>();
                bool foundNothing = true;
                //search profiles
                foreach(var profile in listOfProfiles)
                {
                    bool foundMatch = true;
                    foreach(var keyword in keywords)
                    {
                        UserData currentData = _context.UserData.Where(c => c.UID.Equals(profile.UID)).FirstOrDefault();
                        
                        if (currentData != null && currentData.FirstName.Contains(keyword) || currentData.LastName.Contains(keyword))
                        {
                            continue;
                        }
                        if (profile.Description.Contains(keyword) == false)
                        {
                            foundMatch = false;
                            break;
                        }

                    }
                    if(foundMatch)
                    {
                        CompactIdentityUser matchCompactUser = new CompactIdentityUser();
                        var matchedUser = await _userManager.FindByIdAsync(profile.UID);
                        matchCompactUser.Email = matchedUser.Email;
                        matchCompactUser.Username = matchedUser.UserName;
                        matchCompactUser.UID = profile.UID;
                        matchCompactUser.UserData = _context.UserData.Where(c => c.UID.Equals(profile.UID)).FirstOrDefault();
                        matchCompactUser.UserData.ProfileData = profile;
                        validProfiles.Add(matchCompactUser);
                        foundNothing = false;
                    }
                }
                response.Data = validProfiles;
                if (foundNothing)
                {
                    response.Message = "No Results found";
                    response.Success = false;
                }
            }

            return JsonConvert.SerializeObject(response);
        }
    }
}