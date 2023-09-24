using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CookieAuth.Controllers;

public class HomeController : Controller
{
    [HttpPost("/mvc/login")]
    public async Task<IActionResult> Login()
    {
        await HttpContext.SignInAsync("default", new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                }, 
                "default"
                )
            )
        );
        
        //HttpContext.ChallengeAsync()
        
        return Ok();
    }
}