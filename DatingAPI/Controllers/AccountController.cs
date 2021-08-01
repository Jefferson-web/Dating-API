using DatingAPI.Data;
using DatingAPI.DTOs;
using DatingAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DatingAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {

        private readonly DataContext _context;

        public AccountController(DataContext context) {
            this._context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register([FromBody] RegisterDTO registerDto) {

            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                Password = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            this._context.Users.Add(user);
            await this._context.SaveChangesAsync();
            return user;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login([FromBody] LoginDTO loginDto) {

            var user = await this._context.Users
                                .SingleOrDefaultAsync(user => user.UserName == loginDto.Username.ToLower());

            if (user is null) return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.Password[i]) return Unauthorized("Invalid password");
            }

            return user;

        }

        private async Task<bool> UserExists(string username) {
            return await this._context.Users.AnyAsync(user => user.UserName == username.ToLower());
        }

    }
}
