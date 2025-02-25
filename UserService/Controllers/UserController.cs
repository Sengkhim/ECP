using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private static readonly List<UserDto> Users =
    [
        new UserDto { Id = 1, Name = "Alice", Email = "alice@example.com" },
        new UserDto { Id = 2, Name = "Bob", Email = "bob@example.com" }
    ];

    [HttpGet]
    public IActionResult GetUsers()
    {
        return Ok(Users);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetUserById(int id)
    {
        var user = Users.Find(u => u.Id == id);
        if (user is null)
            return NotFound(new { message = "User not found" });

        return Ok(user);
    }
}

public class UserDto
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; }  = string.Empty;
}