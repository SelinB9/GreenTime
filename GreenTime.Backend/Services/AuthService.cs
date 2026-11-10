using GreenTime.Context;
using GreenTime.Models;
using GreenTime.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GreenTime.Services;

public class AuthService : IAuthService
{
    private readonly GreenTimeDbContext _context;
    private readonly ITokenService _tokenService;
    public AuthService(GreenTimeDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto?> GoogleLoginAsync(GoogleLoginDto dto)
    {
        // 1. Adım: Google'dan gelen idToken'ı doğrula
        var payload = await _tokenService.VerifyGoogleTokenAsync(dto.IdToken);
        if (payload == null)
        {
            return null; // token geçersiz veya süresi dolmuş
        }

        // 2. Adım: Kullanıcı sistemimizde kayıtlı mı kontrol et
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);

        // 3. Adım: Kullanıcı ilk defa geliyorsa sisteme kaydet
        if (user == null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Email = payload.Email,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        // 4. Adım: Kullanıcı için Green Time JWT token'ı üret
        var jwtToken = _tokenService.CreateToken(user);

        // 5. Adım: Yanıt paketini hazırla ve teslim et
        return new AuthResponseDto
        {
            Token = jwtToken,
            Email = user.Email,
            UserId = user.Id
        };
    }
}