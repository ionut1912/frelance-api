using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Frelance.Contracts.Exceptions;
using Frelance.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Frelance.Infrastructure.Services;

public class TokenService
{
    private readonly UserManager<Users> _userManager;
    private readonly IConfiguration _configuration;
    public TokenService(IConfiguration configuration, UserManager<Users> userManager)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(userManager, nameof(userManager));
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<string> GenerateToken(Users user)
    {

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.UserName!),
        };

        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtTokenKey"]!));
        var creeds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var tokenOptions = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creeds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
}
