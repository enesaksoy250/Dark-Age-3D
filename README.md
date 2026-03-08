# Dark Age 3D

Dark Age 3D, mobil odaklı bir 3D sosyal strateji oyunudur. Oyuncu kendi ussunu kurar, haritada varlik gosterir, klanlara katilir, diger oyuncularla mesajlasir ve kaynaklarini yoneterek uzun vadeli bir guc alani insa eder.

Bu repo, oyunun Unity tabanli gelisim surecini, mimari altyapisini ve erken prototipten urune dogru evrimini sergilemek icin tutulmaktadir.

## Oyun Vizyonu

Dark Age 3D'nin temel vaadi:

- Haritada kendi yerini al
- Ussunu kur ve gelistir
- Klan kur veya bir klana katil
- Klan ici iletisimi ve koordinasyonu yonet
- Kaynak ekonomisini ve sosyal gucu ayni anda buyut

Oyun, klasik strateji loop'unu sosyal sistemlerle birlestiren bir yapi hedefler. Sadece bina kurma degil, ayni zamanda klan organizasyonu, sohbet, kaynak paylasimi ve uzun vadeli oyuncu bagliligi uzerine kurulur.

## Ekran Göruntüleri

<p align="center">
  <img src="Dark%20Age/Assets/Screenshots/3.jpg" alt="World map and base view" width="220" />
  <img src="Dark%20Age/Assets/Screenshots/4.jpg" alt="Clan panel" width="220" />
  <img src="Dark%20Age/Assets/Screenshots/1.jpg" alt="Clan chat" width="220" />
</p>

<p align="center">
  <img src="Dark%20Age/Assets/Screenshots/5.jpg" alt="Clan resource transfer" width="220" />
  <img src="Dark%20Age/Assets/Screenshots/2.jpg" alt="Market screen" width="220" />
</p>

## One Cikan Sistemler

- 3D world map uzerinde us ve yapi odakli oynanis
- Klan sistemi, uye yonetimi ve klan ici sosyal organizasyon
- Klan sohbeti ve oyuncular arasi iletisim altyapisi
- Kaynak ekonomisi ve kaynak transferi
- Mobil odakli arayuz ve dikey ekran prototipleme
- Market ve premium odakli monetizasyon zemini

## Teknik Altyapı

- Unity 6
- Unity Input System
- uGUI tabanli arayuz
- Firebase Authentication
- Cloud Firestore
- Firebase Storage
- Firebase Cloud Messaging (FCM)
- SOLID prensiplerine uygun katmanli proje yapisi

Mevcut kod tabani `Assets/_Project` altinda su sekilde ayrilmistir:

- `DarkAge.Core`: domain modelleri ve temel servis arayuzleri
- `DarkAge.Gameplay`: use case'ler, oyun kurallari ve uygulama akislari
- `DarkAge.Infrastructure.Firebase`: Firebase adaptasyon katmani ve veri erisim yapisi
- `DarkAge.Presentation`: sahne bootstrap, HUD, input ve world sunum katmani
- `DarkAge.Tests.*`: EditMode ve PlayMode test iskeleti


## Hedeflenen Oynanış Döngüsü

1. Oyuncu oyuna girer ve hesabini baslatir.
2. Haritada kendi HQ/us konumunu belirler.
3. Kaynak biriktirir ve ilerleme saglar.
4. Klan sistemine katilarak sosyal baglarini guclendirir.
5. Kaynak, iletisim ve strateji uzerinden haritadaki etkisini buyutur.

## Yol Haritasi

- HQ odakli world placement sisteminin genisletilmesi
- daha net ekonomi ve uretim kurallari
- gorev ve ilerleme sisteminin derinlestirilmesi
- klan ici roller ve gelismis sosyal paneller
- PvP/PvE strateji katmaninin eklenmesi
- daha guclu canli operasyon ve monetizasyon yapilari

## Repository Amacı

Bu repository, sadece bir Unity projesi degil; ayni zamanda Dark Age 3D'nin teknik mimarisini, tasarim yonunu ve gelisim surecini belgeleyen bir vitrin reposudur.
