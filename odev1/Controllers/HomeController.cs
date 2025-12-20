using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using odev1.Data;
using odev1.Models;
using System.Diagnostics;

namespace odev1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult TrainerCatalog()
        {
            
            var trainers = _context.Trainers
                .Include(x => x.User) 
                .Include(x => x.trainerExpertises)
                    .ThenInclude(te => te.expertise)
                .Include(x => x.trainerServices)
            .ThenInclude(ts => ts.service)
                .ToList();

            return View(trainers);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
