# Dark-Age-3D

Unity 6 tabanli Phase 1 gameplay core iskeleti `Dark Age/Assets/_Project` altinda kuruldu.

- `DarkAge.Core`: domain modelleri ve servis arayuzleri
- `DarkAge.Gameplay`: use case ve oyun kurallari
- `DarkAge.Infrastructure.Firebase`: reflection tabanli Firebase adapterleri + local JSON fallback
- `DarkAge.Presentation`: runtime bootstrap, world scene, HUD ve input akisi
- `DarkAge.Tests.*`: edit/play mode test iskeleti

Firebase Unity SDK projeye eklendiginde adapterler otomatik olarak Firebase Auth ve Firestore kullanir. SDK yoksa proje local anonymous auth + JSON persistence ile calismaya devam eder.
