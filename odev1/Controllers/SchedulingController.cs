using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using odev1.Models;
using odev1.Services;

namespace odev1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchedulingController : ControllerBase
    {
        private readonly ISchedulingQueryService _query;
        private readonly UserManager<UserDetails> _userManager;

        public SchedulingController(ISchedulingQueryService query, UserManager<UserDetails> userManager)
        {
            _query = query;
            _userManager = userManager;
        }

        // 1) Tüm antrenörler (LINQ filtre basit)
        [HttpGet("trainers")]
        public async Task<IActionResult> GetAllTrainers()
        {
            var list = await _query.GetAllTrainersAsync();
            return Ok(list);
        }

        // 2) Belirli tarihte uygun antrenörler (LINQ)
        [HttpGet("availability")]
        public async Task<IActionResult> GetAvailableTrainers([FromQuery] DateTime date, [FromQuery] int serviceId)
        {
            if (serviceId <= 0) return BadRequest(new { message = "serviceId zorunlu" });
            var list = await _query.GetAvailableTrainersByDateAsync(date, serviceId);
            return Ok(list);
        }

        // 3) Kullanıcının randevuları (auth)
        [Authorize]
        [HttpGet("appointments/me")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            var items = await _query.GetUserAppointmentsAsync(user.Id);
            return Ok(items);
        }
    }
}