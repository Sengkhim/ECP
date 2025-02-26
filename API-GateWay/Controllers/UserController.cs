using ECPLibrary.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_GateWay.Controllers;

[Route("users")]
[ApiController]
public class UserController(IEcpDatabase ecp) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await ecp.Set<IdentityUser>()
            .ToListAsync();

        return Ok(users);
    }
}