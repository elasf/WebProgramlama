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
        public async Task<IActionResult> Create(Service service)
        {
            ModelState.Remove("trainerService");
            ModelState.Remove("trainerServices"); //sadece servis ekliyorum hocasını henüz atamadım!!!

            if (ModelState.IsValid)
            {
                await _adminService.CreateServiceAsync(service);
                return RedirectToAction(nameof(listServices));
            }
            return View(service);
        }

        [HttpGet]
        public IActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Service service)
        {
            if (ModelState.IsValid)
            {
                await _adminService.UpdateServiceAsync(service);
                return RedirectToAction(nameof(listServices));
            }
            return View(service);
        }


        public async Task<IActionResult> Delete(int id)
        {
            await _adminService.DeleteServiceAsync(id);
            return RedirectToAction(nameof(listServices));
        }
    }
}