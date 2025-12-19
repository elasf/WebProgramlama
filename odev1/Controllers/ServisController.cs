using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using odev1.Models;
using odev1.Services;

namespace odev1.Controllers
{
    [Authorize(Roles = "admin")] 
    public class ServicesController : Controller
    {
        private readonly AdminService _adminService;

        public ServicesController(AdminService adminService)
        {
            _adminService = adminService;
        }

        //listeleme
        public async Task<IActionResult> listServices()
        {
            var services = await _adminService.GetAllServicesAsync();
            return View(services);
        }

        [HttpGet] 
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service service)
        {
            ModelState.Remove("trainerService");
            //ModelState.Remove("trainerServices"); //sadece servis ekliyorum hocasını henüz atamadım!!!

            if (ModelState.IsValid)
            {
                await _adminService.CreateServiceAsync(service);
                return RedirectToAction(nameof(listServices));
            }
            return View(service);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var service = await _adminService.GetServiceByIdAsync(id);
            if (service == null) return NotFound();
            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Service service)
        {
            if (id != service.id) return BadRequest();
            if (!ModelState.IsValid) return View(service);

            await _adminService.UpdateServiceAsync(service);
            TempData["success"] = "Hizmet güncellendi.";
            return RedirectToAction(nameof(listServices));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _adminService.DeleteServiceAsync(id);
            TempData["success"] = "Hizmet silindi.";
            return RedirectToAction(nameof(listServices));
        }
    }
}