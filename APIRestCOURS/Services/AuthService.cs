using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using APIRestCOURS.Configuration;
using APIRestCOURS.DataAccess;
using APIRestCOURS.DataAccess.Models;
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
    private readonly BankDbContext _context;
    private readonly JwtSettings _jwtSettings;
    private const int MaxActiveTokensPerUser = 5; // Limite de tokens actifs par utilisateur

    public AuthService(BankDbContext context, IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<(string AccessToken, string RefreshToken)?> RegisterAsync(
        string email, string password, string nom, string prenom, DateTime dateNaissance)
    {
        if (await _context.Users.AnyAsync(u => u.Email == email))
        {
            return null;
        }
        
        var user = new User
        {
            Email = email,
            PasswordHash = HashPassword(password),
            Nom = nom,
            Prenom = prenom,
            DateNaissance = dateNaissance
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var accessToken = GenerateAccessToken(user);
        var refreshToken = await GenerateRefreshTokenAsync(user.Id);

        return (accessToken, refreshToken);
    }

    public async Task<(string AccessToken, string RefreshToken)?> LoginAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null || !VerifyPassword(password, user.PasswordHash))
        {
            return null;
        }

        var accessToken = GenerateAccessToken(user);
        var refreshToken = await GenerateRefreshTokenAsync(user.Id);

        return (accessToken, refreshToken);
    }

    public async Task<(string AccessToken, string RefreshToken)?> RefreshTokenAsync(string refreshToken)
    {
        // Hash the received token to compare with stored hash
        var tokenHash = HashToken(refreshToken);

        var token = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == tokenHash);

        if (token == null || token.IsRevoked || token.ExpiresAt < DateTime.UtcNow)
        {
            return null;
        }

        // Delete old refresh token instead of just revoking it
        _context.RefreshTokens.Remove(token);
        await _context.SaveChangesAsync();

        // Generate new tokens
        var accessToken = GenerateAccessToken(token.User);
        var newRefreshToken = await GenerateRefreshTokenAsync(token.UserId);

        return (accessToken, newRefreshToken);
    }

    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
    {
        // Hash the received token to compare with stored hash
        var tokenHash = HashToken(refreshToken);

        var token = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == tokenHash);

        if (token == null)
        {
            return false;
        }

        // Delete the token instead of revoking
        _context.RefreshTokens.Remove(token);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task CleanupExpiredTokensAsync()
    {
        // Delete all expired or revoked tokens
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
        // Enforce token limit per user: keep only the most recent tokens
        var userTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync();

        // If user has reached the limit, remove the oldest tokens
        if (userTokens.Count >= MaxActiveTokensPerUser)
        {
            var tokensToRemove = userTokens.Skip(MaxActiveTokensPerUser - 1).ToList();
            _context.RefreshTokens.RemoveRange(tokensToRemove);
        }

        // Generate random token
        var tokenValue = GenerateSecureToken();

        // Hash the token before storing
        var tokenHash = HashToken(tokenValue);

        var refreshToken = new RefreshToken
        {
            Token = tokenHash, // Store the hash, not the original token
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        // Return the original token to the client
        return tokenValue;
    }

    private static string GenerateSecureToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static bool VerifyPassword(string password, string passwordHash)
    {
        var hashedInput = HashPassword(password);
        return hashedInput == passwordHash;
    }

    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashedBytes);
    }
}
