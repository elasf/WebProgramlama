using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using odev1.Models.ViewModels; 
using odev1.Services;        

namespace odev1.Controllers
{
    [Authorize(Roles = "admin")] 
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        public IActionResult Index() //deneme için
        {
            return View();
        }

        public IActionResult MemberList() 
        {
            var members = _adminService.getAllMembers();
            return View(members);
        }

        [HttpGet]
        public IActionResult createTrainer()
        {
            return View();
        }


        
    }
}