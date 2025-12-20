using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odev1.Data;
using odev1.Models;

namespace odev1.Controllers
{
    // Genel kullanıcı paneli
    [Authorize(Roles = "user")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserDetails> _userManager;

        public UserController(ApplicationDbContext context, UserManager<UserDetails> userManager)
        {
            _context = context;
            _userManager = userManager;
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

        // Kısayollar
        public IActionResult Book() => RedirectToAction("Create", "Appointments");
        public IActionResult MyAppointments() => RedirectToAction("My", "Appointments");
    }
}

