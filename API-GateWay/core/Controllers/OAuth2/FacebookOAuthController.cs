using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_GateWay.Core.Controllers.OAuth2;

[Route("api/auth-facebook")] 
[ApiController]
public class FacebookOAuthController : ControllerBase
{
    private const string Schema = CookieAuthenticationDefaults.AuthenticationScheme;

        // Step 1: Redirect to Facebook for authentication
        [AllowAnonymous]
        [HttpGet("login")]
        public IActionResult Login()
        {
            var redirectUrl = Url.Action("FacebookLoginCallback", "FacebookOAuth");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        // Step 2: Handle the callback after Facebook authentication
        [AllowAnonymous]
        [HttpGet("callback")]
        public async Task<IActionResult> FacebookLoginCallback()
        {
            var result = await HttpContext.AuthenticateAsync(Schema);
            if (!result.Succeeded)
            {
                return Unauthorized("Authentication failed"); // Handle failure appropriately
            }

            // Extracting claims from the authenticated principal
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, result.Principal?.Identity?.Name ?? "Unknown"),
                new Claim(ClaimTypes.Email, result.Principal?.FindFirst(ClaimTypes.Email)?.Value ?? "No email")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Sign in the user and create an authentication cookie
            await HttpContext.SignInAsync(Schema, principal);

            // Return user info as a JSON response
            return Ok(new
            {
                UserName = result.Principal?.Identity?.Name,
                Email = result.Principal?.FindFirst(ClaimTypes.Email)?.Value
            });
        }

        // Step 3: Get profile information of the authenticated user
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var result = await HttpContext.AuthenticateAsync(Schema);
            if (!result.Succeeded)
            {
                return Unauthorized("You must be logged in to access this resource");
            }

            // Extract user info from the authenticated principal
            var userName = result.Principal?.Identity?.Name;
            var email = result.Principal?.FindFirst(ClaimTypes.Email)?.Value;

            return Ok(new { UserName = userName, Email = email });
        }

        // Step 4: Log out the user
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(Schema);
            return Ok("Logged out successfully");
        }
}