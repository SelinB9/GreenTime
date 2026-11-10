using Google.Apis.Auth;
using GreenTime.Models; //User modelinin bulunduğu namespace

namespace GreenTime.Services;

public interface ITokenService
{
    string CreateToken(User user); //Bana bir User ver, ben sana onun kimlik token'ını (string) döneyim.
    Task<GoogleJsonWebSignature.Payload?> VerifyGoogleTokenAsync(string idToken); //bu satır bir metod imzasıdır.mobil uygulamanın googledan alıp backendime gönderdiği İdtokenı string olarak alıp içeri vericem.Fonksiyonum bu girdi üzerinden doğrulama testleri yapacak
}