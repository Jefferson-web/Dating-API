using DatingAPI.Data;
using DatingAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;

        public UsersController(DataContext context) {
            this._context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<AppUser>> GetUsers() {
            return await this._context.Users.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> FindUserById(int id) {
            var user = await this._context.Users.FindAsync(id);
            if (user is null) {
                return BadRequest("User Not Found");
            }
            return Ok(user);
        }

    }
}
