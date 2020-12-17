using System;
using Microsoft.AspNetCore.Mvc;
using PassGen.Lib;

namespace PassGen.Web.Controllers
{
    [ApiController]
    [Route("/api/v1/[action]")]
    [RequireHttps]
    public class ApiV1Controller : Controller
    {
        private readonly PasswordGenerator _passwordGenerator = new PasswordGenerator();

        [HttpGet]
        public IActionResult Hello()
        {
            return Ok(new {Message = "Hello, world",});
        }
        
        [HttpPost]
        public IActionResult GeneratePassword([FromBody] GeneratePasswordRequest passwordRequest)
        {
            if (passwordRequest == null)
                return BadRequest("password request not found");
            if (string.IsNullOrEmpty(passwordRequest.TargetSite))
                return BadRequest($"Empty {nameof(passwordRequest.TargetSite)}");
            if (string.IsNullOrEmpty(passwordRequest.Salt))
                return BadRequest($"Empty {nameof(passwordRequest.Salt)}");
            
            var generatedPassword = _passwordGenerator.GeneratePassword(passwordRequest.TargetSite, passwordRequest.Salt);
            return Ok(new GeneratePasswordResponse {GeneratedPassword = generatedPassword,});
        }

        public sealed class GeneratePasswordRequest
        {
            public string TargetSite { get; set; }
            
            public string Salt { get; set; }
        }

        public sealed class GeneratePasswordResponse
        {
            public string GeneratedPassword { get; set; }
        }
    }
}