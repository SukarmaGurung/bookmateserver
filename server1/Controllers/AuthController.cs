// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using server1.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(Register request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest("User already exists.");

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "User"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok("User registered successfully.");
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(Login login)
    {
        // Email must be a valid Gmail address
        if (string.IsNullOrWhiteSpace(login.Email) || !login.Email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Email must be a valid Gmail address.");

        // Password must be at least 7 characters
        if (string.IsNullOrWhiteSpace(login.Password) || login.Password.Length < 7)
            return BadRequest("Password must be at least 7 characters long.");

        // Admin login
        var adminEmail = _config["AdminCredentials:Email"];
        var adminPassword = _config["AdminCredentials:Password"];

        if (login.Email == adminEmail && login.Password == adminPassword)
        {
            return Ok(new { token = GenerateJwtToken("Admin", adminEmail, "Admin") });
        }

        // Member login
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == login.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials.");

        return Ok(new { token = GenerateJwtToken(user.FullName, user.Email, user.Role) });
    }



    private string GenerateJwtToken(string name, string email, string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
