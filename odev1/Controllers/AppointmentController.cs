using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using odev1.Services;
using odev1.Models;

namespace odev1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : Controller
    {
        private readonly AppointmentService _appointmentService;


        public AppointmentController(AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost]
        public IActionResult create([FromBody] Appointment appointment)
        {

            try
            {

                var created = _appointmentService.create(appointment);

                return Ok(created);

            }
            catch (InvalidOperationException ex)
            {

                return BadRequest(new { message = ex.Message });

            }

        }

        [HttpPut("{id}")]
        public IActionResult update(int id, [FromBody] Appointment appointment)
        {
            try
            {
                appointment.id = id;

                var updated = _appointmentService.update(appointment);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public IActionResult Cancel(int id)
        {
            try
            {
                var cancelled = _appointmentService.delete(id);
                return Ok(cancelled);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var appointment = _appointmentService
                .GetType()
                .GetMethod("GetById")?
                .Invoke(_appointmentService, new object[] { id });

            if (appointment == null)
                return NotFound();

            return Ok(appointment);
        }




    }
}