using Google.Apis.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GreenTime.Models;
using Microsoft.IdentityModel.Tokens;

namespace GreenTime.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    // appsettings.dev.json'daki Jwt ayarlarını okuyabilmek için IConfiguration'ı enjekte ediyoruz
    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string CreateToken(User user)
    {
        var jwtSettings = _config.GetSection("Jwt");  //appsettings.Development.json içindeki "Jwt": { ... } bloğunu tutar.
        var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);  //JSON'daki gizli metin şifremizi bilgisayarın ve kripto motorunun işleyebileceği byte[] dizisine dönüştürür.

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        var key = new SymmetricSecurityKey(secretKey); //Az önce bayt dizisine çevirdiğimiz şifreyi (.NET dilinde) bir "güvenlik anahtarı" nesnesine sarar. Simetrik denmesinin sebebi: Token'ı üretirken de (imzalarken), gelen token'ı doğrularken de aynı tek anahtarın (SecretKey) kullanılmasıdır.
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // İmza mührünü hazır hale getirir. Sisteme şunu der: "Bu key'i al ve imzalama yaparken HMAC-SHA256 algoritmasını kullan." Bu algoritma token'ın yolda değiştirilmediğini (bütünlüğünü) garanti eden endüstri standardı bir matematiksel mühürdür.


        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = creds
        };
        // "Al bu tokenDescriptor reçetesini, içindeki kurallara göre bana gerçek bir JWT bas."

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }


    // Google id_token doğrulama metodu
    public async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleTokenAsync(string idToken)
    {
        try //güvenlik kontrollerini buraya yazıyorum
        {
            var googleClientId = _config["GoogleAuth:ClientId"];  //appsettings.Development.json dosyasındaki "GoogleAuth" başlığı altındaki "ClientId" metnini okur ve değişkene atar.

            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { googleClientId }   //İçinde yalnızca benim googleClientId değerimin bulunduğu 1 elemanlı bir metin dizisi oluştur ve doğrulama kuralı olarak Google kütüphanesine ver" demektir.
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            return payload;
        }
        catch
        {
            return null; // Token sahte, süresi bitmiş veya ClientId uyuşmuyorsa null döner
        }
    }
}