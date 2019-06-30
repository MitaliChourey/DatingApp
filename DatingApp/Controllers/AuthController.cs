using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Models;
using DatingApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatingApp.Dtos;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace DatingApp.Controllers
{  
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo ,IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register (UserForRegisterDto userDto)
        {

            //validate request
            userDto.UserName=userDto.UserName.ToLower();

            if(await _repo.UserExists(userDto.UserName))
            {
                return BadRequest("UserName already exists");
            }

            var userToCreate  = new  User {

                UserName = userDto.UserName
            };

            var createdUser = await _repo.Register(userToCreate,userDto.Password);
            return StatusCode(201);
        }
    


     [HttpPost("login")]
        public async Task<IActionResult> Login (UserForLoginDto userLoginDto)
        {

            //validate request
             var userFromRepo = await _repo.Login(userLoginDto.UserName.ToLower(),userLoginDto.Password);

             if(userFromRepo==null)
             {
                 return Unauthorized();
             }

             var claims = new [] {

                 new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                 new Claim(ClaimTypes.Name,userFromRepo.UserName)
             };

             var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
             var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
             var tokenDescriptor = new SecurityTokenDescriptor{
                 Subject = new ClaimsIdentity(claims),
                 Expires =DateTime.Now.AddDays(1),
                 SigningCredentials =creds                                               
             };

             var takenHandler = new JwtSecurityTokenHandler();
             var token  = takenHandler.CreateToken(tokenDescriptor);


            return Ok(new {
                token = takenHandler.WriteToken(token)  
            });
           
        }
    }
 }


