using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odev1.Data;
using odev1.Models;
using odev1.Services;

namespace odev1.Controllers
{
    // Genel kullanıcı paneli
    [Authorize(Roles = "user")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserDetails> _userManager;
        private readonly IUserService _userService;

        public UserController(ApplicationDbContext context, UserManager<UserDetails> userManager, IUserService userService)
        {
            _context = context;
            _userManager = userManager;
            _userService = userService;
        }

        // Panel özeti
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var upcomingCount = await _context.Appointments
                .Where(a => a.userId == user.Id && a.AppointmentDate >= DateTime.UtcNow.Date)
                .CountAsync();

            ViewBag.ServiceCount = await _context.Services.CountAsync();
            ViewBag.TrainerCount = await _context.Trainers.CountAsync();
            ViewBag.UpcomingCount = upcomingCount;
            return View();
        }

        // Hizmetler listesi (salt okunur)
        public async Task<IActionResult> Services()
        {
            var list = await _context.Services
                .AsNoTracking()
                .OrderBy(s => s.name)
                .ToListAsync();
            return View(list);
        }

        // Eğitmenler listesi (salt okunur)
        public async Task<IActionResult> Trainers()
        {
            var list = await _context.Trainers
                .AsNoTracking()
                .OrderBy(t => t.fullName)
                .ToListAsync();
            return View(list);
        }

        // Hizmet detayı + tarihe göre o hizmeti verebilen eğitmenler
        [HttpGet]
        public async Task<IActionResult> ServiceDetails(int id, DateTime? date)
        {
            var svc = await _context.Services.AsNoTracking().FirstOrDefaultAsync(s => s.id == id);
            if (svc == null) return NotFound();

            var trainerIds = await _context.TrainerServices
                .Where(ts => ts.serviceId == id)
                .Select(ts => ts.trainerId)
                .ToListAsync();

            var trainersQuery = _context.Trainers.AsNoTracking().Where(t => trainerIds.Contains(t.id));
            if (date.HasValue)
            {
                var d = date.Value.Date;
                var ids = await _context.Availabilities
                    .Where(a => a.date.Date == d && trainerIds.Contains(a.trainerId))
                    .Select(a => a.trainerId)
                    .Distinct()
                    .ToListAsync();
                trainersQuery = trainersQuery.Where(t => ids.Contains(t.id));
            }

            ViewBag.Date = date?.ToString("yyyy-MM-dd");
            ViewBag.Service = svc;
            var trainers = await trainersQuery.OrderBy(t => t.fullName).ToListAsync();
            return View(trainers);
        }

        // Eğitmen detayı + isteğe bağlı hizmet ve tarihe göre uygun slotlar
        [HttpGet]
        public async Task<IActionResult> TrainerDetails(int id, int? serviceId, DateTime? date)
        {
            var trainer = await _context.Trainers.AsNoTracking().FirstOrDefaultAsync(t => t.id == id);
            if (trainer == null) return NotFound();

            var services = await _context.TrainerServices
                .Where(ts => ts.trainerId == id)
                .Select(ts => ts.serviceId)
                .ToListAsync();
            var serviceList = await _context.Services.Where(s => services.Contains(s.id)).AsNoTracking().ToListAsync();

            ViewBag.Trainer = trainer;
            ViewBag.Services = serviceList;
            ViewBag.SelectedServiceId = serviceId;
            ViewBag.Date = date?.ToString("yyyy-MM-dd");

            if (serviceId.HasValue && date.HasValue)
            {
                var slots = await GenerateAvailableSlotsAsync(id, serviceId.Value, date.Value.Date);
                ViewBag.Slots = slots;
            }

            return View();
        }

        // Profilimi düzenle (GET)
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
            var vm = new odev1.ViewModels.ProfileViewModel
            {
                FirstName = user.userAd,
                LastName = user.userSoyad,
                PhoneNumber = user.PhoneNumber
            };
            return View(vm);
        }

        // Profilimi düzenle (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(odev1.ViewModels.ProfileViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            await _userService.updateProfileAsync(user.Id, vm.FirstName, vm.LastName, vm.PhoneNumber ?? "");
            TempData["success"] = "Profiliniz güncellendi.";
            return RedirectToAction(nameof(Profile));
        }

        // Yardımcı: slot üretimi
        private async Task<List<(TimeSpan start, TimeSpan end)>> GenerateAvailableSlotsAsync(int trainerId, int serviceId, DateTime date)
        {
            var duration = await _context.Services.Where(s => s.id == serviceId).Select(s => s.duration).FirstOrDefaultAsync();
            if (duration <= 0) return new List<(TimeSpan, TimeSpan)>();

            var avails = await _context.Availabilities
                .Where(a => a.trainerId == trainerId && a.date.Date == date.Date)
                .ToListAsync();

            var taken = await _context.Appointments
                .Where(a => a.trainerId == trainerId && a.AppointmentDate.Date == date.Date && a.Status != odev1.Models.AppointmentStatus.Cancelled)
                .Select(a => new { a.StartTime, a.EndTime })
                .ToListAsync();

            var result = new List<(TimeSpan, TimeSpan)>();
            foreach (var a in avails)
            {
                var step = TimeSpan.FromMinutes(15); // 15 dk adım
                for (var t = a.startTime; t + TimeSpan.FromMinutes(duration) <= a.endTime; t += step)
                {
                    var s = t;
                    var e = t + TimeSpan.FromMinutes(duration);
                    var conflict = taken.Any(x => s < x.EndTime && e > x.StartTime);
                    if (!conflict) result.Add((s, e));
                }
            }
            return result.OrderBy(x => x.Item1).ToList();
        }

        // Kısayollar
        public IActionResult Book() => RedirectToAction("Create", "Appointments");
        public IActionResult MyAppointments() => RedirectToAction("My", "Appointments");

        // İlerleme (kilo/yağ) listesi ve grafik
        [HttpGet]
        public async Task<IActionResult> Progress()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var entries = await _context.ProgressEntries
                .Where(p => p.userId == user.Id)
                .OrderBy(p => p.date)
                .ToListAsync();

            return View(entries);
        }

        // İlerleme ekle (GET)
        [HttpGet]
        public IActionResult CreateProgress()
        {
            return View(new odev1.ViewModels.ProgressEntryViewModel
            {
                Date = DateTime.Today
            });
        }

        // İlerleme ekle (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProgress(odev1.ViewModels.ProgressEntryViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var entry = new ProgressEntry
            {
                userId = user.Id,
                date = vm.Date,
                weightKg = vm.WeightKg,
                bodyFatPercent = vm.BodyFatPercent
            };
            _context.ProgressEntries.Add(entry);
            await _context.SaveChangesAsync();

            TempData["success"] = "Kayıt eklendi.";
            return RedirectToAction(nameof(Progress));
        }

        // Yardım/SSS
        [HttpGet]
        public IActionResult Help()
        {
            return View();
        }
    }
}

