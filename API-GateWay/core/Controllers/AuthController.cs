using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using API_GateWay.core.DTOs;

namespace API_GateWay.core.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpGet("login-github")]
    public IActionResult Login()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = "/api/auth/github-callback"
        };
        return Challenge(properties, "GitHub");
    }

    [HttpGet("github-callback")]
    public async Task<IActionResult> LoginCallback()
    {
        var authResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!authResult.Succeeded)  return Unauthorized();
        var claims = authResult.Principal?.Claims;
        var user = GitHubUserDto.GitHubResponse(claims);
        return Ok(user);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok("Logged out");
    }
}
