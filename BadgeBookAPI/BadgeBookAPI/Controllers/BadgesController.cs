using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BadgeBookAPI.Data;
using BadgeBookAPI.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BadgeBookAPI.ViewModels;
using Newtonsoft.Json;
using System.Diagnostics;

namespace BadgeBookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BadgesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly string NAME_IDEN_TOKEN = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        public BadgesController(ApplicationDBContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }
        [EnableCors("AllAccessCors")]
        [Authorize(Roles = "App")]
        [HttpPost]
        public async Task<string> assignBadge([FromBody] AssignBadgeViewModel model)
        {
            Badge newBadge = new Badge();
            APIResponse response = new APIResponse();
            try
            {
                var tokenClaims = User.Claims.Select(c =>
                    new
                    {
                        Type = c.Type,
                        Value = c.Value
                    });

                var username = tokenClaims.Where(c => c.Type.Equals(NAME_IDEN_TOKEN)).FirstOrDefault().Value;
                var userIdent = await _userManager.FindByNameAsync(username);
                if (userIdent != null)
                {
                    var currentApplication = _context.Applications.Where(c => c.UID.Equals(userIdent.Id)).FirstOrDefault();
                    //check exist
                    var currentbadge = _context.Badge.Where(c => c.ApplicationId.Equals(currentApplication.Id) && c.UID.Equals(model.UID)).FirstOrDefault();
                    if (currentApplication != null)
                    {
                        newBadge.ApplicationId = currentApplication.Id;
                        newBadge.BadgeName = model.BadgeName;
                        newBadge.BadgeDescription = model.BadgeDescription;
                        newBadge.ImageURL = model.ImageURL;
                        newBadge.UID = model.UID;
                        _context.Badge.Add(newBadge);
                        await _context.SaveChangesAsync();
                        response.Data = newBadge;
                        response.Message = "Successfully created badge";
                        response.Success = true;
                    }
                }
            } catch(Exception e)
            {
                response.Message = "Failed to create badge " + e.Message;
                response.Success = false;
            }

            return JsonConvert.SerializeObject(response);
        }

        [EnableCors("AllAccessCors")]
        [Authorize(Roles = "App")]
        [HttpPut]
        public async Task<string> updateBadge([FromBody] UpdateBadgeViewModel model)
        {
            Badge newBadge = new Badge();
            APIResponse response = new APIResponse();
            try
            {
                var tokenClaims = User.Claims.Select(c =>
                    new
                    {
                        Type = c.Type,
                        Value = c.Value
                    });

                var username = tokenClaims.Where(c => c.Type.Equals(NAME_IDEN_TOKEN)).FirstOrDefault().Value;
                var userIdent = await _userManager.FindByNameAsync(username);
                if (userIdent != null)
                {
                    var currentApplication = _context.Applications.Where(c => c.UID.Equals(userIdent.Id)).FirstOrDefault();
                    //check exist
                    var currentbadge = _context.Badge.Where(c => c.ApplicationId.Equals(currentApplication.Id) && c.BadgeID.Equals(model.BadgeID)).FirstOrDefault();
                    if (currentbadge != null)
                    {
                        currentbadge.BadgeName = model.BadgeName;
                        currentbadge.ImageURL = model.ImageURL;
                        currentbadge.BadgeDescription = model.BadgeDescription;
                        _context.Badge.Update(currentbadge);
                        await _context.SaveChangesAsync();
                        response.Message = "Successfully updated badge";
                        response.Success = true;
                    }
                    else
                    {
                        response.Message = "Failed to update badge ";
                        response.Success = false;
                    }
                }
            }
            catch (Exception e)
            {
                response.Message = "Failed to update badge " + e.Message;
                response.Success = false;
            }
            return JsonConvert.SerializeObject(response);
        }

        [EnableCors("AllAccessCors")]
        [HttpGet("{id}")]
        public async Task<string> getUserBadges(string id)
        {
            APIResponse response = new APIResponse();
            try
            {
                var userIdent = await _userManager.FindByIdAsync(id);
                if(userIdent != null)
                {
                    var badgeList = _context.Badge.Where(c => c.UID.Equals(userIdent.Id)).ToList();
                    response.Data = badgeList;
                    response.Message = "Successfully retrieved user " + id + " badges";
                    response.Success = true;
                    return JsonConvert.SerializeObject(response);
                }
            } catch(Exception e)
            {
                response.Message = "failed to retrieve badges" + e.Message;
                response.Success = false;
            }
            response.Message = "failed to retrieve badges";
            response.Success = false;
            return JsonConvert.SerializeObject(response);
        }
    }
}