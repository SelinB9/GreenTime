using GreenTime.DTOs;

namespace GreenTime.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> GoogleLoginAsync(GoogleLoginDto dto);
}


//! Token servise de veritabanıyla haberleşme yok authservice de var veritabanına gidip kullanıcı varmı yokmu bakıcak yoksa kaydedicek