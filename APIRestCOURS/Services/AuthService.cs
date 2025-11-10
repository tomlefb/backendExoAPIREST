using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using APIRestCOURS.Configuration;
using APIRestCOURS.DataAccess;
using APIRestCOURS.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace APIRestCOURS.Services;

public interface IAuthService
{
    Task<(string AccessToken, string RefreshToken)?> RegisterAsync(string email, string password, string nom, string prenom, DateTime dateNaissance);
    Task<(string AccessToken, string RefreshToken)?> LoginAsync(string email, string password);
    Task<(string AccessToken, string RefreshToken)?> RefreshTokenAsync(string refreshToken);
    Task<bool> RevokeRefreshTokenAsync(string refreshToken);
    Task CleanupExpiredTokensAsync();
}

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly BankDbContext _context;
    private readonly JwtSettings _jwtSettings;
    private const int MaxActiveTokensPerUser = 5;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        BankDbContext context,
        IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<(string AccessToken, string RefreshToken)?> RegisterAsync(
        string email, string password, string nom, string prenom, DateTime dateNaissance)
    {
        var user = new User
        {
            UserName = email,
            Email = email,
            Nom = nom,
            Prenom = prenom,
            DateNaissance = dateNaissance
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return null;
        }

        var accessToken = GenerateAccessToken(user);
        var refreshToken = await GenerateRefreshTokenAsync(user.Id);

        return (accessToken, refreshToken);
    }

    public async Task<(string AccessToken, string RefreshToken)?> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return null;
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            return null;
        }

        var accessToken = GenerateAccessToken(user);
        var refreshToken = await GenerateRefreshTokenAsync(user.Id);

        return (accessToken, refreshToken);
    }

    public async Task<(string AccessToken, string RefreshToken)?> RefreshTokenAsync(string refreshToken)
    {
        var tokenHash = HashToken(refreshToken);

        var token = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == tokenHash);

        if (token == null || token.IsRevoked || token.ExpiresAt < DateTime.UtcNow)
        {
            return null;
        }

        _context.RefreshTokens.Remove(token);
        await _context.SaveChangesAsync();

        var accessToken = GenerateAccessToken(token.User);
        var newRefreshToken = await GenerateRefreshTokenAsync(token.UserId);

        return (accessToken, newRefreshToken);
    }

    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
    {
        var tokenHash = HashToken(refreshToken);

        var token = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == tokenHash);

        if (token == null)
        {
            return false;
        }

        _context.RefreshTokens.Remove(token);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task CleanupExpiredTokensAsync()
    {
        var expiredTokens = await _context.RefreshTokens
            .Where(rt => rt.ExpiresAt < DateTime.UtcNow || rt.IsRevoked)
            .ToListAsync();

        if (expiredTokens.Any())
        {
            _context.RefreshTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
        }
    }

    private string GenerateAccessToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.Prenom} {user.Nom}"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> GenerateRefreshTokenAsync(int userId)
    {
        var userTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync();

        if (userTokens.Count >= MaxActiveTokensPerUser)
        {
            var tokensToRemove = userTokens.Skip(MaxActiveTokensPerUser - 1).ToList();
            _context.RefreshTokens.RemoveRange(tokensToRemove);
        }

        var tokenValue = GenerateSecureToken();
        var tokenHash = HashToken(tokenValue);

        var refreshToken = new RefreshToken
        {
            Token = tokenHash,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return tokenValue;
    }

    private static string GenerateSecureToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashedBytes);
    }
}
