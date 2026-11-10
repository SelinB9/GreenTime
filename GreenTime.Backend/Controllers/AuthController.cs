using GreenTime.Models;
using GreenTime.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; //Web Apı olmanın gerektirdiği sınıfları (atribute leri) sağlar.[ControllerBase][ApiControlller][Route][HttpPost]
using GreenTime.DTOs;

namespace GreenTime.Controllers;

[ApiController]    //.NET'e bu sınıfın HTML web sayfası değil, JSON veri dönen saf bir REST API Controller'ı olduğunu bildirir.
[Route("api/[controller]")]   //Bu sınıfa hangi web adresiyle ulaşılacağını belirler.[controller] ifadesi sınıfın adındaki "Controller" kelimesini otomatik atar: AuthController -> auth.Yani bu dosyanın ana adresi: https://localhost:xxxx/api/auth olur.
public class AuthController : ControllerBase     //ControllerBase, ASP.NET Core'un API sınıfları için sunduğu temel sınıftır.Bu kalıtım sayesinde Ok(), BadRequest(), Unauthorized() gibi HTTP yanıt metotlarını doğrudan kullanabilirsin.
{
    private readonly ITokenService _tokenService;  //Burada Controller şunu söyler: "Benim token üretebilecek bir servise ihtiyacım var ama somut sınıfın detaylarıyla uğraşmak istemiyorum, bana ITokenService kuralına uyan bir araç verin."
    private readonly IAuthService _authService;
    public AuthController(ITokenService tokenService, IAuthService authService)
    {
        _tokenService = tokenService;
        _authService = authService;
    }

    // 1. Kapı: Halka açık, test için sahte kullanıcıya token üreten endpoint
    [HttpPost("test-token")]
    public IActionResult GenerateTestToken()
    {
        var fakeUser = new User
        {
            Id = Guid.NewGuid(),// 1 yerine bunu yazıyoruz
            Email = "selin@greentime.com"
        };
        var token = _tokenService.CreateToken(fakeUser);

        return Ok(new { token });  //new { token }: C#'ta "Anonim Nesne (Anonymous Object)"dir. JSON çıktısının { "token": "ey..." } şeklinde düzgün bir anahtar-değer nesnesi olmasını sağlar.
    }

    // 2. Kapı: [Authorize] ile kilitlenmiş özel oda
    [Authorize]     //Bu etiketi gören .NET, gelen isteğin HTTP başlığındaki Authorization: Bearer <token> değerini kontrol eder. Program.cs'te tanımladığımız TokenValidationParameters devreye girer: İmza doğru mu? Süresi dolmuş mu? Issuer ve Audience eşleşiyor mu? benim backendimin bizzat ürettiği jwt tokenı kontrol eder id tokenı değil dikkat.
    [HttpGet("secure-data")]
    public IActionResult GetSecureData()
    {
        return Ok(new { message = "Tebrikler! Geçerli bir JWT ile korumalı alana başarıyla eriştin." });
    }

    // 1 google ile giriş endpointi 
    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
    {
        // 1. AuthService'e Google'dan gelen token'ı verip kullanıcı kontrolünü ve JWT üretimini yaptırıyoruz
        var result = await _authService.GoogleLoginAsync(dto);

        // 2. Token geçersizse veya süresi dolmuşsa içeri almıyoruz
        if (result == null)
        {
            return Unauthorized(new { message = "Geçersiz veya süresi dolmuş Google token." });
        }
        // 3. Başarılıysa üretilen token ve kullanıcı verisini dönüyoruz (Eksik olan satır):
        return Ok(result);
    }
}
