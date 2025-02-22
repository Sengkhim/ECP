using API_GateWay.core.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace API_GateWay.core.Controllers.OAuth2;

[Route("api/auth-github")]
[ApiController]
public class GitHubOAuthController : ControllerBase
{
    private const string Schema = CookieAuthenticationDefaults.AuthenticationScheme;

    [HttpGet("login")]
    public IActionResult Login()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = "/api/auth-github/profile"
        };
        return Challenge(properties, "GitHub");
    }
    
    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        var authResult = await HttpContext.AuthenticateAsync(Schema);
        if (!authResult.Succeeded)  
            return Unauthorized();
        
        var user = GitHubUserDto.GitHubResponse(authResult.Principal?.Claims);
        return Ok(user);
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(Schema);
        return Ok("Logged out");
    }
}