using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using odev1.Data;
using odev1.Models;
using odev1.Services;
using odev1.ViewModels;

namespace odev1.Controllers
{
    // Üye (user) rolündeki kullanıcıların randevu işlemleri
    [Authorize(Roles = "user")]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAppointmentService _appointmentService;
        private readonly UserManager<UserDetails> _userManager;

        public AppointmentsController(
            ApplicationDbContext context,
            IAppointmentService appointmentService,
            UserManager<UserDetails> userManager)
        {
            _context = context;
            _appointmentService = appointmentService;
            _userManager = userManager;
        }

        // Randevu formu (GET)
        [HttpGet]
        public async Task<IActionResult> Create(int? serviceId = null)
        {
            var vm = new AppointmentCreateViewModel
            {
                // Hizmet listesi (dropdown)
                Services = await _context.Services
                    .Select(s => new SelectListItem
                    {
                        Value = s.id.ToString(),
                        Text = $"{s.name} ({s.duration} dk - {s.price:C})"
                    })
                    .ToListAsync()
            };

            // Hizmet seçilerek gelinmişse eğitmenleri doldur
            if (serviceId.HasValue)
            {
                vm.ServiceId = serviceId;
                vm.Trainers = await TrainersSelectListForService(serviceId.Value);
                var service = await _context.Services.FindAsync(serviceId.Value);
                vm.Price = service?.price;
            }

            return View(vm);
        }

        // Randevu oluşturma (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Services = await _context.Services
                    .Select(s => new SelectListItem
                    {
                        Value = s.id.ToString(),
                        Text = $"{s.name} ({s.duration} dk - {s.price:C})"
                    })
                    .ToListAsync();

                if (vm.ServiceId.HasValue)
                    vm.Trainers = await TrainersSelectListForService(vm.ServiceId.Value);

                return View(vm);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var service = await _context.Services.FirstOrDefaultAsync(s => s.id == vm.ServiceId);
            if (service == null)
            {
                ModelState.AddModelError(string.Empty, "Hizmet bulunamadı.");
                vm.Services = await _context.Services
                    .Select(s => new SelectListItem
                    {
                        Value = s.id.ToString(),
                        Text = $"{s.name} ({s.duration} dk - {s.price:C})"
                    })
                    .ToListAsync();
                return View(vm);
            }

            // Bitiş saatini hizmet süresinden hesapla
            var start = vm.StartTime!.Value;
            var end = start.Add(TimeSpan.FromMinutes(service.duration));

            var appointment = new Appointment
            {
                userId = user.Id,
                trainerId = vm.TrainerId!.Value,
                serviceId = vm.ServiceId!.Value,
                AppointmentDate = vm.AppointmentDate!.Value.Date,
                StartTime = start,
                EndTime = end,
                Price = service.price
            };

            // Müsaitlik / çakışma kontrolü servis içinde
            if (!_appointmentService.canCreateAppointment(appointment))
            {
                ModelState.AddModelError(string.Empty, "Seçilen tarih ve saat dilimi uygun değil. Lütfen başka bir saat deneyin.");
                vm.Services = await _context.Services
                    .Select(s => new SelectListItem
                    {
                        Value = s.id.ToString(),
                        Text = $"{s.name} ({s.duration} dk - {s.price:C})"
                    })
                    .ToListAsync();
                vm.Trainers = await TrainersSelectListForService(vm.ServiceId.Value);
                vm.Price = service.price;
                return View(vm);
            }

            // Uygun → Kaydet
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(My));
        }

        // Kullanıcının randevuları
        [HttpGet]
        public async Task<IActionResult> My()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var items = await _context.Appointments
                .AsNoTracking()
                .Include(a => a.service)
                .Include(a => a.trainer)
                .Where(a => a.userId == user.Id)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.StartTime)
                .ToListAsync();

            return View(items);
        }

        // Randevu iptal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var ap = await _context.Appointments
                .FirstOrDefaultAsync(a => a.id == id && a.userId == user.Id);
            if (ap == null) return NotFound();

            if (ap.Status != odev1.Models.AppointmentStatus.Cancelled)
            {
                ap.Status = odev1.Models.AppointmentStatus.Cancelled;
                await _context.SaveChangesAsync();
                TempData["success"] = "Randevu iptal edildi.";
            }

            return RedirectToAction(nameof(My));
        }

        // Randevu yeniden planla (GET) - aynı servis ve eğitmenle tarih/saat değiştir
        [HttpGet]
        public async Task<IActionResult> Reschedule(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var ap = await _context.Appointments
                .Include(a => a.service)
                .Include(a => a.trainer)
                .FirstOrDefaultAsync(a => a.id == id && a.userId == user.Id);
            if (ap == null) return NotFound();

            var vm = new odev1.ViewModels.AppointmentRescheduleViewModel
            {
                AppointmentId = ap.id,
                AppointmentDate = ap.AppointmentDate,
                StartTime = ap.StartTime,
                ServiceName = ap.service?.name ?? "",
                ServiceDuration = ap.service?.duration ?? 0,
                TrainerName = ap.trainer?.fullName ?? ""
            };

            return View(vm);
        }

        // Randevu yeniden planla (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reschedule(odev1.ViewModels.AppointmentRescheduleViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var ap = await _context.Appointments
                .Include(a => a.service)
                .FirstOrDefaultAsync(a => a.id == vm.AppointmentId && a.userId == user.Id);
            if (ap == null) return NotFound();

            var service = ap.service;
            if (service == null) return BadRequest();

            var start = vm.StartTime!.Value;
            var end = start.Add(TimeSpan.FromMinutes(service.duration));

            // uygunluk kontrolü
            var candidate = new Appointment
            {
                id = ap.id,
                userId = ap.userId,
                trainerId = ap.trainerId,
                serviceId = ap.serviceId,
                AppointmentDate = vm.AppointmentDate!.Value.Date,
                StartTime = start,
                EndTime = end
            };

            if (!_appointmentService.canCreateAppointment(candidate))
            {
                ModelState.AddModelError(string.Empty, "Seçilen tarih ve saat dilimi uygun değil.");
                // Görsel bilgiler
                vm.ServiceName = service.name;
                vm.ServiceDuration = service.duration;
                return View(vm);
            }

            // güncelle
            ap.AppointmentDate = candidate.AppointmentDate;
            ap.StartTime = candidate.StartTime;
            ap.EndTime = candidate.EndTime;
            await _context.SaveChangesAsync();

            TempData["success"] = "Randevu güncellendi.";
            return RedirectToAction(nameof(My));
        }

        // AJAX: Seçilen hizmete göre eğitmenleri getir
        [HttpGet]
        public async Task<IActionResult> TrainersForService(int serviceId)
        {
            var trainerIds = await _context.TrainerServices
                .Where(ts => ts.serviceId == serviceId)
                .Select(ts => ts.trainerId)
                .ToListAsync();

            var trainers = await _context.Trainers
                .Where(t => trainerIds.Contains(t.id))
                .Select(t => new { value = t.id, text = t.fullName })
                .ToListAsync();

            return Json(trainers);
        }

        // Yardımcı: Hizmete uygun eğitmen dropdown
        private async Task<IEnumerable<SelectListItem>> TrainersSelectListForService(int serviceId)
        {
            var trainerIds = await _context.TrainerServices
                .Where(ts => ts.serviceId == serviceId)
                .Select(ts => ts.trainerId)
                .ToListAsync();

            return await _context.Trainers
                .Where(t => trainerIds.Contains(t.id))
                .Select(t => new SelectListItem
                {
                    Value = t.id.ToString(),
                    Text = t.fullName
                })
                .ToListAsync();
        }
    }
}

