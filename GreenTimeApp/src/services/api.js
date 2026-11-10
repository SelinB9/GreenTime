//kendi IPv4 adresimi ve ASP.NET Core backend portumu buraya yazıyorum.
const BASE_URL = 'http://192.168.1.198:5020/api';

export const authService = {
    // Google'dan gelen idToken'ı ASP.NET Core backend'e gönderen fonksiyon
    loginWithGoogle: async (idToken) => {
        try {
            const response = await fetch(`${BASE_URL}/Auth/google-login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ idToken }),
            });
            if (!response.ok) {
                throw new Error(`Sunucu hatası:${response.status}`)
            }
            const data = await response.json();
            return data; // Backend'den dönen JWT token ve kullanıcı bilgisi
        }
        catch (error) {
            console.error('Login API Hatası:', error);
            throw error;
        }
    },
};