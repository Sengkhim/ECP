using Microsoft.EntityFrameworkCore;

namespace UserService.Database;

public class UserContext(DbContextOptions<UserContext> options) : DbContext(options)
{
}