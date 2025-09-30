using BusinessObjects.DTO;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Service;

namespace Group03_MVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        // Customer: View their orders
        public async Task<IActionResult> MyOrders(Guid customerId)
        {
            var orders = await _orderService.GetCustomerOrders(customerId);
            return View(orders);
        }

        // Customer: Create new order
        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
        {
            if (ModelState.IsValid)
            {
                var success = await _orderService.CreateOrder(dto);
                if (success)
                {
                    TempData["SuccessMessage"] = "Đơn hàng đã được tạo thành công!";
                    return RedirectToAction("MyOrders", new { customerId = dto.CustomerId });
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể tạo đơn hàng. Vui lòng thử lại.";
                }
            }
            return RedirectToAction("Details", "Vehicle", new { id = dto.VehicleId });
        }

        // Staff: View pending orders
        public async Task<IActionResult> PendingOrders()
        {
            var orders = await _orderService.GetByStatus("Pending");
            return View(orders);
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
            if (order == null)
            {
                return NotFound();
            }
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

            var myOrders = new List<Orderdto>();
            var pendingOrders = new List<Orderdto>();

            if (role == "customer")
            {
                myOrders = await _orderService.GetCustomerOrders(customerId);
            }
            else if (role == "evm_staff" || role == "admin" || role == "dealer_staff" || role == "dealer_manager")
            {
                pendingOrders = await _orderService.GetByStatus("Pending");
            }
            else if (role == "admin")
            {
                myOrders = await _orderService.GetAllOrders();
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