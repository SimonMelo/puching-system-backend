using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
namespace Backend.Controllersusing Microsoft.IdentityModel.Tokens;


{
    [ApiController]
    [Route("loginAuth")]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        [ProducesResponseType(typeof(Login), StatusCodes.Status200Ok)]
        [ProducesResponseType(typeof(Login), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Login), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Login), StatusCodes.Status404NotFound)]
        public IActionResult Login(LoginModel login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (!IsValidUser(login.Document, login.Password))
                {
                    ModelState.AddModalError(string.Empty, "Usuário ou senha inválidos.")
                    return Unauthorized();
                }
                
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
