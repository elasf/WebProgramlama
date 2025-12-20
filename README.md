
### Mimari Genel Bakış

- Yığın: ASP.NET Core 8 MVC + Identity + EF Core (PostgreSQL).

- Katmanlar:

- Controllers: HTTP isteklerini karşılar, servisleri çağırır, ViewModel üretir.

- Services: İş kuralları ve domain mantığı (randevu, eğitmen, AI önerileri).

- Data: ApplicationDbContext, EF Core konfigürasyonları ve migrasyonlar.

- Models / ViewModels: Veritabanı varlıkları ve UI veri taşıyıcıları.

- Views: Razor görünüm katmanı (MVC).

### Klasör Yapısı (seçilmiş)

- Controllers: AppointmentController, AIRecommendationController, AdminController, ...

- Services: AppointmentService, TrainerService, AIRecommendationService, ...

- Data: ApplicationDbContext, ApplicationDbContextFactory.

- Models: Trainer, Service, Expertise, Availability, Appointment, Member, ProgressEntry, AIRecommendation.

- Migrations: EF Core migration dosyaları ve ApplicationDbContextModelSnapshot.

- Views: Razor sayfaları (Home, Admin, Services, AIRecommendation vb).

### Başlatma ve Bağımlılık Enjeksiyonu

- PostgreSQL bağlantısı DefaultConnection üzerinden açılır.

- Identity + Roller (user, admin, trainer) etkin; uygulama start’ında yoksa oluşturulur.

- Servisler scoped olarak DI’a eklenir; HttpClientFactory AI servisi için kullanılır.

- MVC rotası: {controller=Home}/{action=Index}/{id?}.

### Veritabanı ve Modeller

- Temel tablolar: Trainers, Services, Expertises, TrainerExpertises (N–N), TrainerServices (N–N), Availabilities, Appointments, Members, ProgressEntries, AIRecommendations.

- AIRecommendation:

- Kullanıcı/Müşteri ile ilişkili; üç metin alanında öneriler: ExerciseRecommendations, DietRecommendations, GeneralAdvice.

- CreatedAt/UpdatedAt meta alanları.

---

## Service Katmanı Yapısı

Services
├── AppointmentService.cs 
├── TrainerService.cs      
├── AvailabilityService.cs
├── ExpertiseService.cs
├── GymService.cs (opsiyonel)
├── UserService.cs
└── Relations
    ├── TrainerExpertiseService.cs
    └── TrainerServiceService.cs
 Genel Mimari Yaklaşım
Controller → HTTP isteklerini alır, Service’e iletir

Service → İş kuralları (business logic) burada bulunur

DbContext / Identity → Veri erişimi

Controller içinde iş kuralı yazılmaz

Validation ve karar mekanizmaları service katmanındadır

Bu yaklaşım Spring Boot’taki:

nginx
Kodu kopyala
Controller → Service → Repository
yapısına birebir karşılık gelmektedir.

 AppointmentService
Sistemin en kritik servisidir.
Randevu oluşturma, güncelleme ve iptal işlemlerinin tamamı burada yönetilir.

Sorumluluklar
Eğitmenin müsaitliğini kontrol etmek

Randevu çakışmalarını engellemek

Randevu yaşam döngüsünü yönetmek

Metotlar

create(Appointment)
update(Appointment)
delete / cancel(int appointmentId)
canCreateAppointment(Appointment)
isTrainerAvailable(...)
isWithinTrainerAvailability(...)
hasAppointmentConflict(...)
Açıklamalar
create()
Yeni randevu oluşturur.
Trainer müsait değilse exception fırlatır.

update()
Mevcut randevuyu günceller, tekrar müsaitlik ve çakışma kontrolü yapar.

delete() / cancel()
Fiziksel silme yapılmaz.
Randevu Cancelled statüsüne çekilir (soft delete).

canCreateAppointment()
Randevu oluşturulabilir mi sorusuna true/false döner.

isTrainerAvailable()
Müsaitlik + çakışma kontrollerini birleştirir.

UserService
ASP.NET Identity ile entegre çalışır.
User CRUD işlemleri doğrudan DbContext ile yapılmaz.

Sorumluluklar
Kullanıcı bilgilerini okumak

Profil güncellemek

Kullanıcıyı pasif hale getirmek

Metotlar

GetByUsernameAsync()
GetByEmailAsync()
UpdateProfileAsync()
Açıklamalar
GetByUsernameAsync()
Identity üzerinden kullanıcıyı getirir.

GetByEmailAsync()
Email bazlı kullanıcı sorgusu yapar.

UpdateProfileAsync()
Kullanıcının ad, soyad gibi profil bilgilerini günceller.

Not:
Login, Register, Password işlemleri Identity tarafından yönetilir.

 TrainerService
 
Trainer merkezli tüm iş akışlarını yönetir.
Appointment ve Availability bu servise dolaylı olarak bağlıdır.

Sorumluluklar

Trainer oluşturmak

Trainer’a uzmanlık ve hizmet eklemek

Trainer detaylarını tek noktadan sunmak

Metotlar

createTrainer()
addExpertise(trainerId, expertiseId)
removeExpertise(trainerId, expertiseId)
addService(trainerId, serviceId)
removeService(trainerId, serviceId)
getTrainerDetails(trainerId)
Açıklamalar
createTrainer()
Yeni eğitmen oluşturur.

addExpertise() / removeExpertise()
Trainer ↔ Expertise many-to-many ilişkisini yönetir.

addService() / removeService()
Trainer ↔ Service ilişkisini yönetir.

getTrainerDetails()
Trainer + uzmanlıklar + hizmetler tek DTO olarak dönebilir.

AvailabilityService
Trainer’ın hangi gün ve saatlerde çalışabileceğini yönetir.

Sorumluluklar
Müsaitlik ekleme / silme

Trainer çalışma saatlerini belirleme

Örnek Metotlar

addAvailability()
removeAvailability()
getTrainerAvailabilities()
AppointmentService, müsaitlik kontrolünü buradaki veriler üzerinden yapar.

 ExpertiseService
Basit bir lookup/config servisidir.

Sorumluluklar
Uzmanlık tanımları

CRUD işlemleri

Örnek Metotlar

createExpertise()
getAll()
delete()

 GymService (Opsiyonel)
 
Birden fazla spor salonu senaryosu için kullanılabilir.

Sorumluluklar
Salon bilgileri

Salon çalışma saatleri

Salon → Trainer ilişkisi

Bu servis opsiyoneldir, tek salonlu projelerde zorunlu değildir.

 Relation Servisleri
 
Many-to-many tablolar için ayrı servisler tanımlanmıştır.

TrainerExpertiseService

add()
remove()
exists()
TrainerServiceService
text
Kodu kopyala
add()
remove()
exists()
Bu servisler:

Duplicate kayıtları engeller

İlişki yönetimini merkezileştirir

 GENEL İŞ AKIŞI (ÖZET)
 
Kullanıcı randevu oluşturmak ister

Controller → AppointmentService.create()

AppointmentService:

Trainer müsait mi?

Çakışan randevu var mı?

Uygunsa randevu Pending olarak kaydedilir

Trainer / Admin onaylayabilir

Kullanıcı iptal ederse status Cancelled olur

Veriler raporlama için sistemde kalır

 Sonuç
 
Bu mimari:

Test edilebilir

Okunabilir

Genişletilebilir

Spring Boot alışkanlıklarıyla uyumlu

Akademik ve gerçek dünya senaryolarına uygundur


### Kimlik ve Roller

- ASP.NET Core Identity ve rol yönetimi etkin.

- AIRecommendationController sadece user rolüne açık.

- Admin/eğitmen yönetimi diğer controller ve view’lar üzerinden.

### Ana İş Akışları

- AI Önerileri (Groq)

- GET AIRecommendation/Create: form gösterimi.

- POST AIRecommendation/Create: kullanıcı girdileri + opsiyonel foto; servis çağrısı; sonuç DB’ye kaydedilir; Details’a yönlendirilir.

- GET AIRecommendation/Details/{id}: tek öneri gösterimi.

- GET AIRecommendation/MyRecommendations: kullanıcının geçmiş önerileri listesi.

- Randevu (Appointment)

- api/appointment uçları:

- POST: randevu oluştur

- PUT {id}: güncelle

- DELETE {id}: iptal

- GET {id}: tek randevu getir

- Eğitmen/Servis/Uygunluk

- Eğitmen uzmanlıkları ve verdiği hizmetler (N–N), uygunluk saatleri, fiyat/süre gibi alanlar; admin arayüzleriyle yönetim.

### AI Servisi (Groq ile)

- Sağlayıcı: Groq (OpenAI uyumlu Chat Completions).

- Model önerisi: llama-3.1-70b-versatile (kalite) veya llama-3.1-8b-instant (hız/ücret).

- Rate-limit korumaları:

- Tek çağrıda üç bölüm (Exercise/Diet/General) üretilir.

- Bellek içi cache (10 dk), 2 eşzamanlı istek limiti, 429/503 için exponential backoff + Retry-After desteği.

### Yapılandırma (appsettings.json)

{

  "ConnectionStrings": {

    "DefaultConnection": "Host=localhost;Port=5432;Database=odev1db;Username=postgres;Password=***"

  },

  "AI": { "Provider": "Groq" },

  "Groq": {

    "ApiKey": "GROQ_API_KEYIN",

    "Model": "llama-3.1-70b-versatile",

    "MaxOutputTokens": "1024"

  },

  "Logging": {

    "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" }

  },

  "AllowedHosts": "*"

}

### Migrasyon ve Veritabanı Yönetimi (PMC)

- Veritabanını baştan kur:
    
    Drop-Database -Project odev1 -StartupProject odev1 -Context ApplicationDbContext -Force
    
    Remove-Item -Recurse -Force .\odev1\Migrations
    
    Add-Migration InitialClean -Project odev1 -StartupProject odev1 -Context ApplicationDbContext
    
    Update-Database -Project odev1 -StartupProject odev1 -Context ApplicationDbContext
    
