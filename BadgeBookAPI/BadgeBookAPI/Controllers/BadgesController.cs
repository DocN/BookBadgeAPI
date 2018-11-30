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

namespace BadgeBookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BadgesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BadgesController(ApplicationDBContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: api/Badges
        [EnableCors("AllAccessCors")]
        [HttpGet]
        public IEnumerable<Badge> GetBadge()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name);
            return _context.Badge.Where(a => a.ApplicationId == (""+user.Id));
        }

        // GET: api/Badges/5
        [EnableCors("AllAccessCors")]
        [Authorize(Roles = "App")]
        [HttpGet("{id}")]
        public IEnumerable<Badge> GetBadge([FromRoute] string id)
        {
            return _context.Badge.Where(a => a.ProfileId == (id));
        }

        // PUT: api/Badges/5
        [HttpPut("{id}")]
        [Authorize(Roles = "App")]
        [EnableCors("AllAccessCors")]
        public async Task<IActionResult> PutBadge([FromRoute] string id, [FromBody] Badge badge)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != badge.BadgeID)
            {
                return BadRequest();
            }

            _context.Entry(badge).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BadgeExists(id))
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

        // POST: api/Badges
        [HttpPost]
        [Authorize(Roles = "App")]
        [EnableCors("AllAccessCors")]
        public async Task<IActionResult> PostBadge([FromBody] Badge badge)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Badge.Add(badge);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBadge", new { id = badge.BadgeID }, badge);
        }

        // DELETE: api/Badges/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "App")]
        [EnableCors("AllAccessCors")]
        public async Task<IActionResult> DeleteBadge([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var badge = await _context.Badge.FindAsync(id);
            if (badge == null)
            {
                return NotFound();
            }

            _context.Badge.Remove(badge);
            await _context.SaveChangesAsync();

            return Ok(badge);
        }

        private bool BadgeExists(string id)
        {
            return _context.Badge.Any(e => e.BadgeID == id);
        }
    }
}