using Microsoft.AspNetCore.Mvc;
using Services.Service;
using BusinessObjects.Models;
using System.Globalization;

namespace Group03_MVC.Controllers
{
    public class CustomerTestDriveController : Controller
    {
        private readonly ITestDriveAppointmentService _testDriveService;
        private readonly IVehicleService _vehicleService;

        public CustomerTestDriveController(ITestDriveAppointmentService testDriveService, IVehicleService vehicleService)
        {
            _testDriveService = testDriveService ?? throw new ArgumentNullException(nameof(testDriveService));
            _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
        }

        // Hiển thị danh sách lịch hẹn của khách hàng
        public async Task<IActionResult> Index()
        {
            // Kiểm tra đăng nhập
            var customerIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(customerIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            // Chỉ cho phép customer truy cập
            var userRole = HttpContext.Session.GetString("Role");
            if (userRole != "customer")
            {
                return RedirectToAction("Forbidden", "Home");
            }

            var customerId = Guid.Parse(customerIdString);
            var appointments = await _testDriveService.GetAppointmentsByCusId(customerId);
            return View(appointments);
        }

        // Hiển thị form đặt lịch hẹn
        [HttpGet]
        public async Task<IActionResult> BookAppointment(Guid? vehicleId = null)
        {
            // Kiểm tra đăng nhập
            var customerIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(customerIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            // Chỉ cho phép customer truy cập
            var userRole = HttpContext.Session.GetString("Role");
            if (userRole != "customer")
            {
                return RedirectToAction("Forbidden", "Home");
            }

            try
            {
                var customerId = Guid.Parse(customerIdString);
                
                // Kiểm tra xem khách hàng đã có lịch hẹn đang chờ xử lý hay chưa
                var existingAppointments = await _testDriveService.GetAppointmentsByCusId(customerId);
                var activeAppointments = existingAppointments?.Where(a => 
                    a.Status.ToLower() == "process" || 
                    a.Status.ToLower() == "confirmed" || 
                    a.Status.ToLower() == "pending").ToList();

                if (activeAppointments != null && activeAppointments.Any())
                {
                    // Có lịch hẹn đang chờ xử lý
                    var earliestAppointment = activeAppointments.OrderBy(a => a.AppointmentDate).First();
                    var statusText = earliestAppointment.Status.ToLower() switch
                    {
                        "process" => "đang xử lý",
                        "confirmed" => "đã được xác nhận",
                        "pending" => "đang chờ xử lý",
                        _ => earliestAppointment.Status
                    };
                    
                    TempData["WarningMessage"] = $"Bạn đã có lịch hẹn lái thử vào ngày {earliestAppointment.AppointmentDate:dd/MM/yyyy HH:mm} đang {statusText}. " +
                                               $"Vui lòng hoàn thành hoặc hủy lịch hẹn hiện tại trước khi đặt lịch mới.";
                    ViewBag.HasActiveAppointment = true;
                    ViewBag.ActiveAppointmentId = earliestAppointment.Id;
                }

                var vehicles = await _testDriveService.GetAvailableVehiclesForTestDrive();
                ViewBag.Vehicles = vehicles;
                
                // Nếu có vehicleId được truyền vào, set làm selected
                if (vehicleId.HasValue && vehicleId.Value != Guid.Empty)
                {
                    ViewBag.SelectedVehicleId = vehicleId.Value;
                    
                    // Kiểm tra xe có tồn tại trong danh sách có sẵn không
                    var selectedVehicle = vehicles.FirstOrDefault(v => v.Id == vehicleId.Value);
                    if (selectedVehicle != null)
                    {
                        ViewBag.SelectedVehicleName = selectedVehicle.Name;
                    }
                }
                
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải danh sách xe: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Xử lý đặt lịch hẹn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookAppointment(Guid vehicleId, string appointmentDate, string appointmentTime, string AppointmentDateTime, string? notes)
        {
            // Kiểm tra đăng nhập
            var customerIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(customerIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            // Chỉ cho phép customer truy cập
            var userRole = HttpContext.Session.GetString("Role");
            if (userRole != "customer")
            {
                return RedirectToAction("Forbidden", "Home");
            }

            try
            {
                var customerId = Guid.Parse(customerIdString);

                // Kiểm tra lại xem khách hàng có lịch hẹn đang chờ xử lý không
                var existingAppointments = await _testDriveService.GetAppointmentsByCusId(customerId);
                var activeAppointments = existingAppointments?.Where(a => 
                    a.Status.ToLower() == "process" || 
                    a.Status.ToLower() == "confirmed" || 
                    a.Status.ToLower() == "pending").ToList();

                if (activeAppointments != null && activeAppointments.Any())
                {
                    TempData["ErrorMessage"] = "Bạn đã có lịch hẹn đang chờ xử lý. Vui lòng hoàn thành hoặc hủy lịch hẹn hiện tại trước khi đặt lịch mới.";
                    await LoadVehiclesForView();
                    return View();
                }

                // Validate input
                if (vehicleId == Guid.Empty)
                {
                    TempData["ErrorMessage"] = "Vui lòng chọn xe muốn lái thử.";
                    await LoadVehiclesForView();
                    return View();
                }

                // Parse appointment date and time
                DateTime finalAppointmentDate;
                
                // Try to parse from combined datetime first
                if (!string.IsNullOrEmpty(AppointmentDateTime))
                {
                    if (!DateTime.TryParse(AppointmentDateTime, out finalAppointmentDate))
                    {
                        TempData["ErrorMessage"] = "Định dạng ngày giờ không hợp lệ.";
                        await LoadVehiclesForView();
                        return View();
                    }
                }
                // Fall back to separate date and time fields
                else if (!string.IsNullOrEmpty(appointmentDate) && !string.IsNullOrEmpty(appointmentTime))
                {
                    if (!DateTime.TryParseExact($"{appointmentDate} {appointmentTime}", "yyyy-MM-dd HH:mm", 
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out finalAppointmentDate))
                    {
                        TempData["ErrorMessage"] = "Định dạng ngày giờ không hợp lệ.";
                        await LoadVehiclesForView();
                        return View();
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Vui lòng chọn ngày và giờ hẹn.";
                    await LoadVehiclesForView();
                    return View();
                }

                // Kiểm tra thời gian hẹn phải sau thời điểm hiện tại ít nhất 1 giờ
                var oneHourFromNow = DateTime.Now.AddHours(1);
                if (finalAppointmentDate <= oneHourFromNow)
                {
                    TempData["ErrorMessage"] = "Thời gian hẹn phải sau thời điểm hiện tại ít nhất 1 giờ.";
                    await LoadVehiclesForView();
                    return View();
                }

                // Kiểm tra xe có sẵn không
                var vehicle = await _vehicleService.GetVehicleById(vehicleId);
                if (vehicle == null || vehicle.IsActive != true || vehicle.StockQuantity <= 0)
                {
                    TempData["ErrorMessage"] = "Xe được chọn hiện không khả dụng.";
                    await LoadVehiclesForView();
                    return View();
                }

                // Kiểm tra khung giờ có trống không
                var isAvailable = await _testDriveService.IsTimeSlotAvailable(vehicleId, finalAppointmentDate);
                if (!isAvailable)
                {
                    TempData["ErrorMessage"] = "Khung giờ này đã có lịch hẹn khác. Vui lòng chọn thời gian khác.";
                    await LoadVehiclesForView();
                    return View();
                }

                // Lấy dealer đầu tiên có sẵn
                var dealer = await _testDriveService.GetFirstAvailableDealer();
                if (dealer == null)
                {
                    TempData["ErrorMessage"] = "Hiện tại không có đại lý nào có sẵn. Vui lòng thử lại sau.";
                    await LoadVehiclesForView();
                    return View();
                }

                var appointment = new TestDriveAppointment
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    VehicleId = vehicleId,
                    DealerId = dealer.Id,
                    AppointmentDate = finalAppointmentDate,
                    Notes = notes ?? string.Empty,
                    Status = "pending", 
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var result = await _testDriveService.CreateTestDriveAppointment(appointment);
                if (result)
                {
                    TempData["SuccessMessage"] = "Đặt lịch hẹn lái thử thành công! Chúng tôi sẽ liên hệ với bạn sớm nhất có thể.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi đặt lịch hẹn. Vui lòng thử lại.";
                    await LoadVehiclesForView();
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                try 
                {
                    await LoadVehiclesForView();
                    return View();
                }
                catch 
                {
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        private async Task LoadVehiclesForView()
        {
            try
            {
                var vehicles = await _testDriveService.GetAvailableVehiclesForTestDrive();
                ViewBag.Vehicles = vehicles;
            }
            catch
            {
                ViewBag.Vehicles = new List<BusinessObjects.Models.Vehicle>();
            }
        }

        // Xem chi tiết lịch hẹn
        public async Task<IActionResult> Details(Guid id)
        {
            // Kiểm tra đăng nhập
            var customerIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(customerIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            // Chỉ cho phép customer truy cập
            var userRole = HttpContext.Session.GetString("Role");
            if (userRole != "customer")
            {
                return RedirectToAction("Forbidden", "Home");
            }

            try
            {
                var customerId = Guid.Parse(customerIdString);
                var appointment = await _testDriveService.GetAppointmentById(id);
                
                if (appointment == null || appointment.CustomerId != customerId)
                {
                    return NotFound();
                }

                return View(appointment);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Hủy lịch hẹn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelAppointment(Guid appointmentId)
        {
            // Kiểm tra đăng nhập
            var customerIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(customerIdString))
            {
                return RedirectToAction("Login", "Account");
            }

            // Chỉ cho phép customer truy cập
            var userRole = HttpContext.Session.GetString("Role");
            if (userRole != "customer")
            {
                return RedirectToAction("Forbidden", "Home");
            }

            try
            {
                var customerId = Guid.Parse(customerIdString);
                var result = await _testDriveService.CancelCustomerAppointment(appointmentId, customerId);
                
                if (result)
                {
                    TempData["SuccessMessage"] = "Hủy lịch hẹn thành công.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể hủy lịch hẹn. Lịch hẹn có thể đã được xử lý hoặc không tồn tại.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // API để kiểm tra khung giờ có sẵn không
        [HttpPost]
        public async Task<IActionResult> CheckTimeSlotAvailability([FromBody] TimeSlotRequest request)
        {
            try
            {
                if (request?.VehicleId == Guid.Empty || request?.AppointmentDate == DateTime.MinValue)
                {
                    return Json(new { available = false, error = "Dữ liệu không hợp lệ" });
                }

                var isAvailable = await _testDriveService.IsTimeSlotAvailable(request.VehicleId, request.AppointmentDate);
                return Json(new { available = isAvailable });
            }
            catch (Exception ex)
            {
                return Json(new { available = false, error = ex.Message });
            }
        }
    }

    public class TimeSlotRequest
    {
        public Guid VehicleId { get; set; }
        public DateTime AppointmentDate { get; set; }
    }
}