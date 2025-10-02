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
            try
            {
                var userRole = HttpContext.Session.GetString("Role");
                
                // Cho phép tất cả các role staff và admin truy cập
                if (string.IsNullOrEmpty(userRole) || 
                    !(userRole == "dealer_manager" || userRole == "evm_staff" || userRole == "dealer_staff" || userRole == "admin"))
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền truy cập trang này.";
                    return RedirectToAction("Index", "Home");
                }

                List<BusinessObjects.Models.TestDriveAppointment> appointments;

                // Nếu là admin hoặc evm_staff, lấy tất cả lịch hẹn
                if (userRole == "admin" || userRole == "evm_staff")
                {
                    // Tạm thời sử dụng dealerId rỗng để lấy tất cả (cần sửa service)
                    appointments = await _testDriveService.GetAllTestDriveAppointments(Guid.Empty);
                }
                else
                {
                    // Với dealer staff, chỉ lấy lịch hẹn của dealer đó
                    var dealerIdString = HttpContext.Session.GetString("DealerId");
                    if (string.IsNullOrEmpty(dealerIdString))
                    {
                        TempData["ErrorMessage"] = "Không tìm thấy thông tin đại lý.";
                        return RedirectToAction("Index", "Home");
                    }

                    var dealerId = Guid.Parse(dealerIdString);
                    appointments = await _testDriveService.GetAllTestDriveAppointments(dealerId);
                }

                return View(appointments);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách lịch hẹn.";
                return View(new List<BusinessObjects.Models.TestDriveAppointment>());
            }
        }

        public async Task<IActionResult> Details(Guid appointmentId)
        {
            try
            {
                var userRole = HttpContext.Session.GetString("Role");
                
                // Cho phép tất cả các role staff và admin truy cập
                if (string.IsNullOrEmpty(userRole) || 
                    !(userRole == "dealer_manager" || userRole == "evm_staff" || userRole == "dealer_staff" || userRole == "admin"))
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền truy cập trang này.";
                    return RedirectToAction("Index", "Home");
                }

                var appointment = await _testDriveService.GetAppointmentById(appointmentId);
                
                if (appointment == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy lịch hẹn.";
                    return RedirectToAction(nameof(Index));
                }

                // Chỉ kiểm tra quyền truy cập với dealer staff
                if (userRole == "dealer_staff" || userRole == "dealer_manager")
                {
                    var dealerIdString = HttpContext.Session.GetString("DealerId");
                    if (!string.IsNullOrEmpty(dealerIdString))
                    {
                        var dealerId = Guid.Parse(dealerIdString);
                        if (appointment.DealerId != dealerId)
                        {
                            TempData["ErrorMessage"] = "Bạn không có quyền truy cập lịch hẹn này.";
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }

                return View(appointment);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải chi tiết lịch hẹn.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmAppointment(Guid appointmentId)
        {
            try
            {
                var userRole = HttpContext.Session.GetString("Role");
                
                // Cho phép tất cả các role staff và admin thực hiện
                if (string.IsNullOrEmpty(userRole) || 
                    !(userRole == "dealer_manager" || userRole == "evm_staff" || userRole == "dealer_staff" || userRole == "admin"))
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền thực hiện hành động này.";
                    return RedirectToAction(nameof(Index));
                }

                var appointment = await _testDriveService.GetAppointmentById(appointmentId);
                
                if (appointment == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy lịch hẹn.";
                    return RedirectToAction(nameof(Index));
                }

                // Chỉ kiểm tra quyền truy cập với dealer staff
                if (userRole == "dealer_staff" || userRole == "dealer_manager")
                {
                    var dealerIdString = HttpContext.Session.GetString("DealerId");
                    if (!string.IsNullOrEmpty(dealerIdString))
                    {
                        var dealerId = Guid.Parse(dealerIdString);
                        if (appointment.DealerId != dealerId)
                        {
                            TempData["ErrorMessage"] = "Bạn không có quyền xử lý lịch hẹn này.";
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }

                var result = await _testDriveService.BrowseTestDriveAppointments(true, appointmentId);
                if (result)
                {
                    TempData["SuccessMessage"] = "Xác nhận lịch hẹn thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể xác nhận lịch hẹn. Vui lòng thử lại.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xác nhận lịch hẹn.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelAppointment(Guid appointmentId)
        {
            try
            {
                var userRole = HttpContext.Session.GetString("Role");
                
                // Cho phép tất cả các role staff và admin thực hiện
                if (string.IsNullOrEmpty(userRole) || 
                    !(userRole == "dealer_manager" || userRole == "evm_staff" || userRole == "dealer_staff" || userRole == "admin"))
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền thực hiện hành động này.";
                    return RedirectToAction(nameof(Index));
                }

                var appointment = await _testDriveService.GetAppointmentById(appointmentId);
                
                if (appointment == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy lịch hẹn.";
                    return RedirectToAction(nameof(Index));
                }

                // Chỉ kiểm tra quyền truy cập với dealer staff
                if (userRole == "dealer_staff" || userRole == "dealer_manager")
                {
                    var dealerIdString = HttpContext.Session.GetString("DealerId");
                    if (!string.IsNullOrEmpty(dealerIdString))
                    {
                        var dealerId = Guid.Parse(dealerIdString);
                        if (appointment.DealerId != dealerId)
                        {
                            TempData["ErrorMessage"] = "Bạn không có quyền xử lý lịch hẹn này.";
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }

                var result = await _testDriveService.BrowseTestDriveAppointments(false, appointmentId);
                if (result)
                {
                    TempData["SuccessMessage"] = "Hủy lịch hẹn thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể hủy lịch hẹn. Vui lòng thử lại.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi hủy lịch hẹn.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteAppointment(Guid appointmentId)
        {
            try
            {
                var userRole = HttpContext.Session.GetString("Role");
                
                // Cho phép tất cả các role staff và admin thực hiện
                if (string.IsNullOrEmpty(userRole) || 
                    !(userRole == "dealer_manager" || userRole == "evm_staff" || userRole == "dealer_staff" || userRole == "admin"))
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền thực hiện hành động này.";
                    return RedirectToAction(nameof(Index));
                }

                var appointment = await _testDriveService.GetAppointmentById(appointmentId);
                
                if (appointment == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy lịch hẹn.";
                    return RedirectToAction(nameof(Index));
                }

                // Kiểm tra trạng thái lịch hẹn phải là "confirmed"
                if (appointment.Status.ToLower() != "confirmed")
                {
                    TempData["ErrorMessage"] = "Chỉ có thể hoàn thành lịch hẹn đã được xác nhận.";
                    return RedirectToAction(nameof(Index));
                }

                // Chỉ kiểm tra quyền truy cập với dealer staff
                if (userRole == "dealer_staff" || userRole == "dealer_manager")
                {
                    var dealerIdString = HttpContext.Session.GetString("DealerId");
                    if (!string.IsNullOrEmpty(dealerIdString))
                    {
                        var dealerId = Guid.Parse(dealerIdString);
                        if (appointment.DealerId != dealerId)
                        {
                            TempData["ErrorMessage"] = "Bạn không có quyền xử lý lịch hẹn này.";
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }

                // Sử dụng service CompleteTestDriveAppointments
                var result = await _testDriveService.CompleteTestDriveAppointments(appointmentId);
                if (result)
                {
                    TempData["SuccessMessage"] = "Hoàn thành lịch hẹn thành công! Khách hàng giờ có thể đặt lịch hẹn mới.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể hoàn thành lịch hẹn. Vui lòng thử lại.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Có lỗi xảy ra khi hoàn thành lịch hẹn: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
