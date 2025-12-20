using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using odev1.Models;
using odev1.Models.ViewModels;
using odev1.Services;

namespace odev1.Controllers
{
    [Authorize(Roles = "admin")]
    public class TrainerManageController : Controller
    {
        private readonly TrainerManageService _service;

        public TrainerManageController(TrainerManageService service)
        {
            _service = service;
        }


        public async Task<IActionResult> Index()
        {
            var trainers = await _service.GetAllTrainersAsync();
            return View(trainers);
        }

        public IActionResult Create()
        {
            FillDropdowns();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TrainerManageViewModel model)
        {
            if (ModelState.IsValid)
            {

                var result = await _service.CreateTrainerAsync(model);

                if (result.Succeeded)
                    return RedirectToAction(nameof(Index));

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }


            FillDropdowns();
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var trainer = await _service.GetTrainerByIdAsync(id);
            if (trainer == null) return NotFound();

            var model = new TrainerManageViewModel
            {
                TrainerId = trainer.id,

                //USERDAN ALDIM CUNKU DELİRMEK UZEREYİM
                FirstName = trainer.User.userAd,
                LastName = trainer.User.userSoyad,
                email = trainer.User.Email,
                PhoneNumber = trainer.User.PhoneNumber,

                SelectedServiceIds = trainer.trainerServices.Select(x => x.serviceId).ToArray(),
                SelectedExpertiseIds = trainer.trainerExpertises.Select(x => x.expertiseId).ToArray()
            };

            FillDropdowns();
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(TrainerManageViewModel model)
        {
            //edit yaparken şifre girmek zorunlu değil
            if (string.IsNullOrEmpty(model.Password)) ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                await _service.UpdateTrainerAsync(model);
                return RedirectToAction(nameof(Index));
            }

            FillDropdowns(); 
            return View(model);
        }


        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteTrainerAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private void FillDropdowns()
        {
            var services = _service.GetAllServices();
            var expertises = _service.GetAllExpertises();

            // "??" işaretleri sayesinde liste null gelse bile boş liste oluşturur, hata vermez.
            ViewBag.Services = new MultiSelectList(services ?? new List<Service>(), "id", "name");
            ViewBag.Expertises = new MultiSelectList(expertises ?? new List<Expertise>(), "id", "expertise");
        }
    }
}