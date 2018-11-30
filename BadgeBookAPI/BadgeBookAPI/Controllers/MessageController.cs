using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BadgeBookAPI.Data;
using BadgeBookAPI.Models;
using BadgeBookAPI.ResponseModels;
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

        [EnableCors("AllAccessCors")]
        [HttpPost("sendMsg")]
        public async Task<ActionResult<string>> sendMessage([FromBody] SendMessageViewModel model)
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
                //my user identity
                var userIdent = await _userManager.FindByNameAsync(username);
                if(userIdent != null)
                {
                    Message newMessage = new Message();
                    newMessage.Subject = model.Subject;
                    newMessage.Msg = model.Msg;
                    newMessage.SenderUID = userIdent.Id;
                    newMessage.ReceiverUID = model.MsgToUID;
                    newMessage.Read = false;
                    DateTime currentTime = DateTime.Now;
                    newMessage.SentTime = currentTime;
                    _context.Messages.Add(newMessage);
                    await _context.SaveChangesAsync();
                    response.Message = "Sucessfully sent message";
                    response.Success = true;
                }
            } catch(Exception e)
            {
                response.Message = "Failed to send message";
                response.Success = false;
            }
            return JsonConvert.SerializeObject(response);
        }


        [EnableCors("AllAccessCors")]
        [HttpGet("getmsgs")]
        public async Task<ActionResult<string>> getMyMessages()
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

                if (userIdent != null)
                {
                    var myMsgs = _context.Messages.Where(c => c.ReceiverUID.Equals(userIdent.Id)).OrderByDescending(c=>c.SentTime).ToList();
                    var msgContainerList = await convertToMsgContainer(myMsgs);
                    
                    response.Data = msgContainerList;
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

        public async Task<List<MsgContainer>> convertToMsgContainer(List<Message> messages)
        {
            List<EmailSearch> MappedEmails = new List<EmailSearch>();
            List<MsgContainer> MsgContainerList = new List<MsgContainer>();
            foreach (var msg in messages)
            {
                
                var currentUID = msg.SenderUID;
                string currentEmail = "";
                var currentMappedEmail = MappedEmails.Where(c => c.UID.Equals(currentUID)).FirstOrDefault();
                
                if (currentMappedEmail == null)
                {
                    var currentIdent = await _userManager.FindByIdAsync(currentUID);
                    EmailSearch currentESearch = new EmailSearch();
                    currentESearch.Email = currentIdent.Email;
                    currentESearch.UID = currentUID;
                    currentEmail = currentIdent.Email;
                    MappedEmails.Add(currentESearch);
                }
                else
                {
                    currentEmail = currentMappedEmail.Email;
                }
                MsgContainer currentContainer = new MsgContainer();
                currentContainer.Msg = msg;
                currentContainer.SenderEmail = currentEmail;
                MsgContainerList.Add(currentContainer);
            }
            return MsgContainerList;
        }

        [EnableCors("AllAccessCors")]
        [HttpPost("setReadStatus")]
        public async Task<ActionResult<string>> setMsgRead([FromBody] SetMsgReadViewModel model)
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
                if (userIdent != null)
                {
                    var currentMsg = _context.Messages.Where(c => c.MessageID.Equals(model.MsgID)).FirstOrDefault();
                    if (currentMsg != null)
                    {
                        if (currentMsg.ReceiverUID.Equals(userIdent.Id))
                        {
                            currentMsg.Read = true;
                            _context.Messages.Update(currentMsg);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                response.Message = "Successfully change read status on msg " + model.MsgID;
                response.Success = true;
            }
            catch (Exception e)
            {
                response.Message = "Failed to change read status " + model.MsgID;
                response.Success = false;
            }
            return JsonConvert.SerializeObject(response);
        }
    }
}