import React, { useState, useEffect } from 'react';
import {
    StyleSheet,
    Text,
    View,
    TouchableOpacity,
    ActivityIndicator,
    Alert,
    Image,
    SafeAreaView,
} from 'react-native';

import * as Google from 'expo-auth-session/providers/google';//Google giriş akışını, buton basıldığında Google kimlik penceresinin açılmasını ve idToken alınmasını yöneten hazır Expo kütüphanesidir.
import { AntDesign } from '@expo/vector-icons';
import { COLORS } from '../constants/colors';
import { authService } from '../services/api';
import * as AuthSession from 'expo-auth-session';
import * as WebBrowser from 'expo-web-browser'; //Google ile giriş yaparken açılan oturum açma sayfasının, işlem bitince otomatik olarak düzgünce kapanmasını sağlar.

// Web tarayıcısı oturumunun düzgün kapanması için gerekli
WebBrowser.maybeCompleteAuthSession();

export default function LoginScreen() {
    const [loading, setLoading] = useState(false);

    // Google OAuth yapılandırması
    // NOT: Google Cloud Console'dan aldığın Client ID'leri buraya ekleyeceğiz
    const [request, response, promptAsync] = Google.useIdTokenAuthRequest({    //İçerideki 3 eleman, Google oturum açma sürecinin öncesini, sonrasını ve o anını temsil eden bir kumanda takımı gibidir (Expo’nun Google ile giriş sürecini yönetmek için sunduğu özel bir React kancasıdır (hook).)
        clientId: '1040215256941-a918tugml68ispb6r3863f42j0pia69v.apps.googleusercontent.com',
        //iosClientId: '1040215256941-laeaq4045imbivc8jf6hggjh5raocuoj.apps.googleusercontent.com',
        androidClientId: '1040215256941-qhjece2blu36kspgl8k54glditcmu889.apps.googleusercontent.com',
        // Expo Go üzerinde 400 hatasını önlemek için proxy yönlendirmesi:
        redirectUri: 'https://auth.expo.io/@selin09/GreenTimeApp',
        
    });

    // Google'dan yanıt geldiğinde çalışacak tetikleyici
    useEffect(() => {
        if (response?.type === 'success') {
            console.log('google tam yanıt:', response.params);
            const { id_token } = response.params;  //Google'ın oturum açma işlemi bittiğinde URL üzerinden bize geri gönderdiği tüm veri ve parametreleri içinde toplayan bir JavaScript nesnesidir (paketidir).
            console.log('Giden idToken:', id_token);
            handleBackendLogin(id_token);
            } else if (response?.type) {
    console.log('Farklı bir durum döndü:', response.type);
        }
    }, [response]);
    

    // HandleBacendLOgin Backend giriş sürecini ve ekrandaki yükleniyor/uyarı (UI) durumlarını yöneten fonksiyon
    const handleBackendLogin = async (idToken) => {
        setLoading(true);
        try {
            const data = await authService.loginWithGoogle(idToken);  //authService.loginWithGoogle(idToken): api.js dosyasının içine koyduğun loginWithGoogle fonksiyonunu çağırır ve Google'dan az önce aldığın idToken'ı ona teslim eder.
            console.log('Backend Giriş Başarılı:', data);
            Alert.alert('Giriş Başarılı', `Hoş geldin, ${data.email || 'Kullanıcı'}!`);
            // Bir sonraki adımda dönen data.token değerini SecureStore'a kaydedeceğiz
        } catch (error) {
            console.error('Giriş Hatası:', error);
            Alert.alert('Hata', 'Giriş yapılırken bir sorun oluştu.');
        } finally {
            setLoading(false);
        }
    }



return (
    <SafeAreaView style={styles.container}>
      <View style={styles.content}>
        {/* Başlık ve Karşılama Alanı */}
        <View style={styles.header}>
          <Text style={styles.title}>Green Time</Text>
          <Text style={styles.subtitle}>
            Dijital detoks ile odağını geri kazan.
          </Text>
        </View>

        {/* Buton ve Yükleniyor Durumu */}
        <View style={styles.actionContainer}>
          {loading ? (
            <ActivityIndicator size="large" color={COLORS.primary} />
          ) : (
            <TouchableOpacity
              style={styles.googleButton}
              disabled={!request}
              onPress={() => promptAsync()}
                        >
                  <Image
                  source={require('../../assets/google-logo.png')}
                  style={styles.googleIconImage}
                 resizeMode="contain"
  />          
              <Text style={styles.googleButtonText}>Google ile Devam Et</Text>
            </TouchableOpacity>
          )}
        </View>
      </View>
    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: COLORS.background,
  },
  content: {
    flex: 1,
    justifyContent: 'space-between',
   paddingHorizontal: 24,
    paddingTop: 80,                  // Başlığın yukarıdan rahat görünmesi için
    paddingBottom: 40,
  },
  header: {
    alignItems: 'center',
    marginBottom: 48,
  },
  title: {
    fontSize: 32,
    fontWeight: '700',
    color: COLORS.primary,
    marginBottom: 8,
  },
  subtitle: {
    fontSize: 16,
    color: COLORS.textSecondary,
    textAlign: 'center',
  },
  actionContainer: {
    width: '100%',
    alignItems: 'center',
  },
  googleButton: {
    width: '100%',
    height: 52,
    backgroundColor: COLORS.surface,
      borderRadius: 12,
    flexDirection: 'row',            // Logo ve yazıyı yan yana hizalar
    justifyContent: 'center',
    alignItems: 'center',
    borderWidth: 1,
    borderColor: COLORS.border,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 4,
    elevation: 2,
  },
  googleButtonText: {
    fontSize: 16,
    fontWeight: '600',
    color: COLORS.textPrimary,
    },
  googleIconImage: {
  width: 20,
  height: 20,
  marginRight: 12,
},
});
