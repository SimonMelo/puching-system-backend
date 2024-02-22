using Microsoft.AspNetCore.Http;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend.Controllers;

namespace Backend.Middleware
{
    public class TokenRefreshMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenRefreshMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                string? token = context.Request.Headers["Authorization"];
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                if (jwtToken.ValidTo.Subtract(DateTime.UtcNow).TotalMinutes < 5) 
                {
                    var newToken = GenerateJwtToken(jwtToken.Claims);
                    context.Response.Headers["Authorization"] = newToken;
                }
            }
            else 
            {
                context.Response.Headers.Remove("Authorization");
            }
            
            await _next(context);
        }

         private string GenerateJwtToken(System.Collections.Generic.IEnumerable<System.Security.Claims.Claim> claims)
        {
            if (claims == null || !claims.Any()) 
            {
                return string.Empty;
            }

            var securityKey = AuthController.GenerateRandomSecurityKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}