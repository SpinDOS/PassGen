using System;
using Microsoft.AspNetCore.Mvc;
using PassGen.Lib;

namespace PassGen.Web.Controllers
{
    [ApiController]
    [Route("/api/v1/[action]")]
    public class ApiV1Controller : Controller
    {
        private const string RequestHeaderXPassgenSalt = "X-PASSGEN-SALT";
        private const string ResponseHeaderXPassgenGeneratedPassword = "X-PASSGEN-GENERATED-PASSWORD";
        private readonly PasswordGenerator _passwordGenerator = new PasswordGenerator();

        [HttpGet]
        public IActionResult Hello()
        {
            return Ok(new {Message = "Hello, world"});
        }

        [HttpPost]
        [RequireHttps]
        public IActionResult GeneratePassword([FromBody] GeneratePasswordRequest passwordRequest, [FromHeader(Name = RequestHeaderXPassgenSalt)] string salt)
        {
            if (passwordRequest == null)
                return BadRequest("password request not found");
            if (string.IsNullOrEmpty(passwordRequest.TargetSite))
                return BadRequest($"Empty {nameof(passwordRequest.TargetSite)}");
            if (string.IsNullOrEmpty(salt))
                return BadRequest($"Empty header {RequestHeaderXPassgenSalt}");

            Response.Headers[ResponseHeaderXPassgenGeneratedPassword] = _passwordGenerator.GeneratePassword(passwordRequest.TargetSite, salt);
            return Ok(new {Message = $"Password successfully generated. See header {ResponseHeaderXPassgenGeneratedPassword}"});
        }

        public sealed class GeneratePasswordRequest
        {
            public string TargetSite { get; set; }
        }
    }
}
