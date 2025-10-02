using Microsoft.AspNetCore.Mvc;
using Services.Service;

namespace Group03_MVC.Controllers
{
    public class StaffTestDriveAppointmentsController : Controller
    {
        private readonly ITestDriveAppointmentService _testDriveService;

        public StaffTestDriveAppointmentsController(ITestDriveAppointmentService testDriveService)
        {
            _testDriveService = testDriveService ?? throw new ArgumentNullException(nameof(testDriveService));
        }

        public async Task<IActionResult> Index()
        {
            var dealerIdString = HttpContext.Session.GetString("DealerId");
            if (string.IsNullOrEmpty(dealerIdString))
            {
                return Unauthorized("Dealer ID not found. Please login again.");
            }

            var dealerId = Guid.Parse(dealerIdString);
            var appointments = await _testDriveService.GetAllTestDriveAppointments(dealerId);
            return View(appointments);
        }

        public async Task<IActionResult> Details(Guid appointmentId)
        {
            var dealerIdString = HttpContext.Session.GetString("DealerId");
            if (string.IsNullOrEmpty(dealerIdString))
            {
                return Unauthorized("Dealer ID not found. Please login again.");
            }

            var dealerId = Guid.Parse(dealerIdString);
            var appointment = await _testDriveService.GetAppointmentById(appointmentId);
            if (appointment == null || appointment.DealerId != dealerId)
            {
                return NotFound();
            }

            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmAppointment(Guid appointmentId)
        {
            var dealerIdString = HttpContext.Session.GetString("DealerId");
            if (string.IsNullOrEmpty(dealerIdString))
            {
                return Unauthorized("Dealer ID not found. Please login again.");
            }

            var dealerId = Guid.Parse(dealerIdString);
            var appointment = await _testDriveService.GetAppointmentById(appointmentId);
            if (appointment == null || appointment.DealerId != dealerId)
            {
                return NotFound();
            }

            var result = await _testDriveService.BrowseTestDriveAppointments(true, appointmentId);
            if (result)
            {
                return RedirectToAction(nameof(Index));
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> CancelAppointment(Guid appointmentId)
        {
            var dealerIdString = HttpContext.Session.GetString("DealerId");
            if (string.IsNullOrEmpty(dealerIdString))
            {
                return Unauthorized("Dealer ID not found. Please login again.");
            }

            var dealerId = Guid.Parse(dealerIdString);
            var appointment = await _testDriveService.GetAppointmentById(appointmentId);
            if (appointment == null || appointment.DealerId != dealerId)
            {
                return NotFound();
            }

            var result = await _testDriveService.BrowseTestDriveAppointments(false, appointmentId);
            if (result)
            {
                return RedirectToAction(nameof(Index));
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> CompleteAppointment(Guid appointmentId)
        {
            var dealerIdString = HttpContext.Session.GetString("DealerId");
            if (string.IsNullOrEmpty(dealerIdString))
            {
                return Unauthorized("Dealer ID not found. Please login again.");
            }

            var dealerId = Guid.Parse(dealerIdString);
            var appointment = await _testDriveService.GetAppointmentById(appointmentId);
            if (appointment == null || appointment.DealerId != dealerId)
            {
                return NotFound();
            }

            var result = await _testDriveService.CompleteTestDriveAppointments(appointmentId);
            if (result)
            {
                return RedirectToAction(nameof(Index));
            }
            return BadRequest();
        }


    }
}
