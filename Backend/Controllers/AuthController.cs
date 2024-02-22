using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Controllers
{
    [ApiController]
    [Route("loginAuth")]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login(LoginModel login)
        {
            if (login == null || string.IsNullOrEmpty(login.Document) || string.IsNullOrEmpty(login.Password))
            {
                return BadRequest("Documento ou senha não fornecidos.");
            }

            if (!IsValidUser(login.Document, login.Password))
            {
                return Unauthorized("Usuário ou senha inválidos.");
            }

            try
            {
                var token = GenerateJwtToken(login.Document);
                return Ok(new { token });
            }
            catch (Exception)
            {
                return StatusCode(500, "Houve um erro interno. Tente novamente mais tarde.");
            }
        }

        private bool IsValidUser(string document, string password)
        {
            return document == "30030030030" && password == "senha123";
        }

        private string GenerateJwtToken(string document)
        {
            var securityKey = GenerateRandomSecurityKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: new[] { new Claim(ClaimTypes.Name, document) },
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static SymmetricSecurityKey GenerateRandomSecurityKey() 
        {
            var keyBytes = new byte[32];
            new Random().NextBytes(keyBytes);
            return new SymmetricSecurityKey(keyBytes);
        }
    }
}
