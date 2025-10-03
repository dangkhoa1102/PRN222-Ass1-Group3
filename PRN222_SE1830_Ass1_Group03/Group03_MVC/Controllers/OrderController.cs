using BusinessObjects.DTO;
using BusinessObjects.Models;
using Group03_MVC.Attributes;
using Microsoft.AspNetCore.Mvc;
using Services.Service;

namespace Group03_MVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IVehicleService _vehicleService;
        private readonly IDealerService _dealerService;

        public OrderController(IOrderService orderService, IVehicleService vehicleService, IDealerService dealerService)
        {
            _orderService = orderService;
            _vehicleService = vehicleService;
            _dealerService = dealerService;
        }

        // Thêm action History để xử lý URL /Order/History
        public async Task<IActionResult> History()
        {
            // Kiểm tra đăng nhập
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "Account");
            }

            // Chỉ cho phép customer xem lịch sử đơn hàng của mình
            var role = HttpContext.Session.GetString("Role");
            if (role != "customer")
            {
                return RedirectToAction("Forbidden", "Home");
            }

            try
            {
                var customerId = Guid.Parse(userIdStr);
                var orders = await _orderService.GetCustomerOrders(customerId);
                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải lịch sử đơn hàng: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        // Customer: View their orders
        public async Task<IActionResult> MyOrders(Guid customerId, string? status, string? paymentStatus, DateTime? fromDate, DateTime? toDate)
        {
            var orders = await _orderService.GetCustomerOrders(customerId);

            if (!string.IsNullOrEmpty(status))
                orders = orders.Where(o => o.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(paymentStatus))
                orders = orders.Where(o => o.PaymentStatus.Equals(paymentStatus, StringComparison.OrdinalIgnoreCase)).ToList();

            if (fromDate.HasValue)
                orders = orders.Where(o => o.CreatedAt >= fromDate.Value).ToList();

            if (toDate.HasValue)
                orders = orders.Where(o => o.CreatedAt <= toDate.Value).ToList();

            ViewBag.CustomerId = customerId;
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> RejectOrder(Guid orderId, Guid customerId)
        {
            var success = await _orderService.RejectOrder(orderId, customerId);
            if (success)
            {
                TempData["SuccessMessage"] = "Đơn hàng đã được từ chối.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể từ chối đơn hàng. Vui lòng thử lại.";
            }

            return RedirectToAction("MyOrders", new { customerId });
        }

        // Customer: Create new order - CẢI THIỆN XỬ LÝ NOTES
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorization("customer")]
        public async Task<IActionResult> CreateOrder(Guid VehicleId, Guid DealerId, string? Notes)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Account");

            try
            {
                var customerId = Guid.Parse(userIdStr);
                
                // Tạo DTO với xử lý Notes an toàn
                var dto = new CreateOrderDTO
                {
                    CustomerId = customerId,
                    VehicleId = VehicleId,
                    DealerId = DealerId,
                    Notes = string.IsNullOrWhiteSpace(Notes) ? "Không có ghi chú" : Notes.Trim()
                };

                // Validate required fields
                if (dto.VehicleId == Guid.Empty || dto.DealerId == Guid.Empty)
                {
                    TempData["ErrorMessage"] = "Vui lòng chọn đầy đủ thông tin xe và đại lý.";
                    return RedirectToAction("Details", "Vehicle", new { id = VehicleId });
                }

                var success = await _orderService.CreateOrder(dto);
                if (success)
                {
                    TempData["SuccessMessage"] = "Đơn hàng đã được tạo thành công!";
                    return RedirectToAction(nameof(History));
                }

                TempData["ErrorMessage"] = "Số lượng xe đã hết nên không thể tạo đơn hàng. Vui lòng chọn xe khác.";
                return RedirectToAction("Details", "Vehicle", new { id = VehicleId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Có lỗi xảy ra: {ex.Message}";
                return RedirectToAction("Details", "Vehicle", new { id = VehicleId });
            }
        }

        // Staff: Confirm order
        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(Guid orderId, Guid staffId)
        {
            var success = await _orderService.ConfirmOrder(orderId, staffId);
            if (success)
            {
                TempData["SuccessMessage"] = "Đơn hàng đã được xác nhận thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xác nhận đơn hàng. Vui lòng thử lại.";
            }
            return RedirectToAction("PendingOrders");
        }

        // Customer: Complete payment
        [HttpPost]
        public async Task<IActionResult> CompletePayment(Guid orderId, Guid customerId)
        {
            var success = await _orderService.CompletePayment(orderId, customerId);
            if (success)
            {
                TempData["SuccessMessage"] = "Thanh toán hoàn tất thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể hoàn tất thanh toán. Vui lòng thử lại.";
            }
            return RedirectToAction("MyOrders", new { customerId });
        }

        // View order details
        public async Task<IActionResult> Details(Guid id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null) return NotFound();

            // Nếu bạn vẫn muốn show thông tin dealer trong view
            var dealers = await _dealerService.GetAllDealers();
            ViewBag.Dealers = dealers;

            return View(order);
        }

        // API endpoints for AJAX calls
        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(Guid id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null) return NotFound();
            return Json(order);
        }

        public async Task<IActionResult> Index()
        {
            // Lấy role và userId từ Session
            var role = HttpContext.Session.GetString("Role");
            var userIdStr = HttpContext.Session.GetString("UserId");
            Guid customerId = string.IsNullOrEmpty(userIdStr) ? Guid.Empty : Guid.Parse(userIdStr);

            var myOrders = new List<OrderDTO>();
            var pendingOrders = new List<OrderDTO>();

            if (role == "customer")
            {
                myOrders = await _orderService.GetCustomerOrders(customerId);
            }
            else if (role == "admin")
            {
                // admin thấy tất cả
                myOrders = await _orderService.GetAllOrders();
                pendingOrders = await _orderService.GetByStatus("Processing"); // hoặc toàn bộ tùy yêu cầu
            }
            else if (role == "evm_staff" || role == "dealer_staff" || role == "dealer_manager")
            {
                pendingOrders = await _orderService.GetByStatus("Processing");
            }

            var vm = new OrdersDashboardViewModel
            {
                MyOrders = myOrders,
                PendingOrders = pendingOrders
            };

            return View(vm);
        }
    }
}