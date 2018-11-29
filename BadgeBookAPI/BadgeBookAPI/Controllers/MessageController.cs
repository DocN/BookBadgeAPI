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
    public class MessageController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext _context;
        private readonly string NAME_IDEN_TOKEN = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        public MessageController(ApplicationDBContext context, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        /*
        [EnableCors("AllAccessCors")]
        [HttpPost("RecMsg")]
        public async Task<ActionResult<string>> sendMessage()
        {

        }

         */

        [EnableCors("AllAccessCors")]
        [HttpGet("RecMsg")]
        public async Task<ActionResult<string>> getRecievedMessages()
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

                var username = tokenClaims.Where(c => c.Type.Equals(NAME_IDEN_TOKEN)).FirstOrDefault().Value;
                var userIdent = await _userManager.FindByNameAsync(username);
                if(userIdent != null)
                {
                    var recMsgs = _context.Messages.Where(c => c.ReceiverUID.Equals(userIdent.Id)).OrderBy(c => c.SentTime).ToList();
                    response.Data = recMsgs;
                }
                response.Message = "Successfully retrieved messages";
                response.Success = true;
            }
            catch (Exception e)
            {
                response.Message = "Error: " + e.Message;
                response.Success = false;
            }
            return JsonConvert.SerializeObject(response);
        }
    }
}