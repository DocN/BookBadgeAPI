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
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace BadgeBookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public ApplicationsController(ApplicationDBContext context, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;

        }

        // GET: api/Applications
        [EnableCors("AllAccessCors")]
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public IEnumerable<Application> GetApplications()
        {
            return _context.Applications;
        }

        // GET: api/Applications/active
        [EnableCors("AllAccessCors")]
        [HttpGet("active")]
        public IEnumerable<Application> GetActiveApplications()
        {
            return _context.Applications.Where(a => a.Approved);
        }

        // GET: api/Applications/5
        [EnableCors("AllAccessCors")]
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetApplication([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var application = await _context.Applications.FindAsync(id);

            if (application == null)
            {
                return NotFound();
            }

            return Ok(application);
        }

        // PUT: api/Applications/5
        [EnableCors("AllAccessCors")]
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApplication([FromRoute] string id, [FromBody] Application application)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != application.Id)
            {
                return BadRequest();
            }

            _context.Entry(application).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Applications
        [EnableCors("AllAccessCors")]
        [HttpPost]
        public async Task<IActionResult> PostApplication([FromBody] Application application)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // update this shit later
            application.Approved = true;
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            if (await _userManager.FindByNameAsync(application.Name + "@gmail.com") == null)
            {
                IdentityUser newUser = new IdentityUser();
                newUser.Email = application.Name + "@gmail.com";
                newUser.UserName = application.Name + "@gmail.com";
                var result = await _userManager.CreateAsync(newUser, "P@$$w0rd");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, "App");
                    var claim = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, newUser.UserName),
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claim, "Token");

                    
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "App"));
                    

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
                            expiration = token.ValidTo,
                           id = application.Id,
                            application
                    
                        });
                }
            }


            return CreatedAtAction("GetApplication", new { id = application.Id }, application);
        }

        // DELETE: api/Applications/5
        [EnableCors("AllAccessCors")]
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplication([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var application = await _context.Applications.FindAsync(id);
            if (application == null)
            {
                return NotFound();
            }

            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();

            return Ok(application);
        }

        private bool ApplicationExists(string id)
        {
            return _context.Applications.Any(e => e.Id == id);
        }
    }
}