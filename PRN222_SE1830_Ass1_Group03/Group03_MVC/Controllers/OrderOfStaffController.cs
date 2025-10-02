using BusinessObjects.DTO;
using BusinessObjects.Models;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Group03_MVC.Attributes;

namespace Group03_MVC.Controllers
{
    [RoleAuthorization("dealer_staff", "dealer_manager", "evm_staff")]
    public class OrderOfStaffController : Controller
    {
        private readonly OrderRepository _orderRepo;
        private readonly Vehicle_Dealer_ManagementContext _context;

        public OrderOfStaffController(OrderRepository orderRepo, Vehicle_Dealer_ManagementContext context)
        {
            _orderRepo = orderRepo;
            _context = context;
        }

        // Index: hiển thị orders của dealer staff đang login
        public async Task<IActionResult> Index(string? status)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (!Guid.TryParse(userIdStr, out var userId)) return RedirectToAction("Login", "Account");

            var role = HttpContext.Session.GetString("Role");
            List<OrderDTO> orders;
            // Tất cả role được phép (dealer_staff, dealer_manager, evm_staff)
            // chỉ nhìn thấy các đơn do CHÍNH họ tạo
            var staff = await _context.Users.FindAsync(userId);
            if (staff == null) return RedirectToAction("Forbidden", "Home");
            orders = await _orderRepo.GetOrdersByCreatorAsync(userId);

            if (!string.IsNullOrWhiteSpace(status))
            {
                orders = orders.Where(o => string.Equals(o.Status, status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Thống kê xe bán chạy
            var vehicleStats = await _orderRepo.GetVehicleSalesStatsAsync();

            ViewBag.FilterStatus = status;
            ViewBag.VehicleStats = vehicleStats;
            return View(orders);
        }

        // Details
        public async Task<IActionResult> Details(Guid id)
        {
            var dto = await _orderRepo.GetOrderByIdAsync(id);
            if (dto == null) return NotFound();

            // Only creator can view details
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (!Guid.TryParse(userIdStr, out var userId)) return RedirectToAction("Login", "Account");
            var isCreator = await _orderRepo.IsOrderCreatedByUserAsync(id, userId);
            if (!isCreator) return RedirectToAction("Forbidden", "Home");

            return View(dto);
        }

        /// GET Create
        public async Task<IActionResult> Create()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (!Guid.TryParse(userIdStr, out var userId)) return RedirectToAction("Login", "Account");

            var staff = await _context.Users.FindAsync(userId);
            if (staff == null)
                return RedirectToAction("Forbidden", "Home");
            if (staff.DealerId == null)
            {
                TempData["Error"] = "Tài khoản của bạn chưa được gán vào đại lý nào, không thể tạo đơn.";
                return RedirectToAction(nameof(Index));
            }

            var vehicles = await _orderRepo.GetAvailableVehiclesAsync();
            ViewBag.Vehicles = vehicles;
            ViewBag.DealerId = staff.DealerId.Value;
            ViewBag.CurrentStaffId = staff.Id;
            ViewBag.CurrentStaffName = staff.FullName;

            return View();
        }

        // POST Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderRequest model)
        {
            if (!ModelState.IsValid)
            {
                // thu thập lỗi để debug
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                TempData["Error"] = "Dữ liệu không hợp lệ: " + string.Join("; ", errors);

                ViewBag.Vehicles = _context.Vehicles
                    .Select(v => new
                    {
                        v.Id,
                        v.Name,
                        v.Price
                    }).ToList();

                return View(model);
            }

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (!Guid.TryParse(userIdStr, out var userId))
                return RedirectToAction("Login", "Account");

            var staff = await _context.Users.FindAsync(userId);
            if (staff == null)
                return RedirectToAction("Forbidden", "Home");
            if (staff.DealerId == null)
            {
                TempData["Error"] = "Tài khoản của bạn chưa được gán vào đại lý nào, không thể tạo đơn.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var orderId = await _orderRepo.CreateOrderAsync(model, staff.DealerId.Value, userId);

                TempData["Success"] = "Tạo đơn hàng thành công.";
                return RedirectToAction(nameof(Details), new { id = orderId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi khi tạo đơn hàng: " + ex.Message;

                ViewBag.Vehicles = _context.Vehicles
                    .Select(v => new
                    {
                        v.Id,
                        v.Name,
                        v.Price
                    }).ToList();

                return View(model);
            }
        }

        // GET Edit
        public async Task<IActionResult> Edit(Guid id)
        {
            var dto = await _orderRepo.GetOrderByIdAsync(id);
            if (dto == null) return NotFound();

            // Only creator can edit
            var userIdStrCreator = HttpContext.Session.GetString("UserId");
            if (!Guid.TryParse(userIdStrCreator, out var currentUserIdForEdit)) return RedirectToAction("Login", "Account");
            var isCreatorForEdit = await _orderRepo.IsOrderCreatedByUserAsync(id, currentUserIdForEdit);
            if (!isCreatorForEdit) return RedirectToAction("Forbidden", "Home");

            var model = new UpdateOrderRequest
            {
                Id = dto.Id,
                VehicleId = dto.VehicleId,
                TotalAmount = dto.TotalAmount,
                Status = dto.Status,
                PaymentStatus = dto.PaymentStatus,
                Notes = dto.Notes,
                CustomerName = dto.CustomerName,
                CustomerPhone = dto.CustomerPhone,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };

            ViewBag.Vehicles = new SelectList(await _orderRepo.GetAvailableVehiclesAsync(), "Id", "Name");
            ViewBag.StatusList = new List<SelectListItem>
            {
                new SelectListItem("processing", "processing", model.Status == "processing"),
                new SelectListItem("completed", "completed", model.Status == "completed"),
                new SelectListItem("cancelled", "cancelled", model.Status == "cancelled")
            };
            ViewBag.PaymentStatusList = new List<SelectListItem>
            {
                new SelectListItem("unpaid", "unpaid", model.PaymentStatus == "unpaid"),
                new SelectListItem("paid", "paid", model.PaymentStatus == "paid"),
                new SelectListItem("refunded", "refunded", model.PaymentStatus == "refunded")
            };
            return View(model);
        }

        // POST Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateOrderRequest model)
        {
            try
            {
                // Only creator can edit
                var userIdStrCreator2 = HttpContext.Session.GetString("UserId");
                if (!Guid.TryParse(userIdStrCreator2, out var currentUserIdForEdit2)) return RedirectToAction("Login", "Account");
                var isCreatorForEdit2 = await _orderRepo.IsOrderCreatedByUserAsync(model.Id, currentUserIdForEdit2);
                if (!isCreatorForEdit2) return RedirectToAction("Forbidden", "Home");

                if (model.VehicleId == Guid.Empty)
                {
                    ModelState.AddModelError(nameof(model.VehicleId), "Vui lòng chọn xe");
                }
                if (model.TotalAmount <= 0)
                {
                    ModelState.AddModelError(nameof(model.TotalAmount), "Tổng tiền phải lớn hơn 0");
                }

                // CustomerName/CustomerPhone are display-only on Edit; ignore their required checks
                ModelState.Remove(nameof(model.CustomerName));
                ModelState.Remove(nameof(model.CustomerPhone));

                if (!ModelState.IsValid)
                {
                    ViewBag.Vehicles = new SelectList(await _orderRepo.GetAvailableVehiclesAsync(), "Id", "Name");
                    ViewBag.StatusList = new List<SelectListItem>
                    {
                        new SelectListItem("processing", "processing", model.Status == "processing"),
                        new SelectListItem("completed", "completed", model.Status == "completed"),
                        new SelectListItem("cancelled", "cancelled", model.Status == "cancelled")
                    };
                    ViewBag.PaymentStatusList = new List<SelectListItem>
                    {
                        new SelectListItem("unpaid", "unpaid", model.PaymentStatus == "unpaid"),
                        new SelectListItem("paid", "paid", model.PaymentStatus == "paid"),
                        new SelectListItem("refunded", "refunded", model.PaymentStatus == "refunded")
                    };
                    return View(model);
                }

                var userIdStr = HttpContext.Session.GetString("UserId");
                if (!Guid.TryParse(userIdStr, out var userId)) return RedirectToAction("Login", "Account");

                var ok = await _orderRepo.UpdateOrderAsync(model, userId);
                if (!ok)
                {
                    TempData["Error"] = "Không tìm thấy đơn hàng để cập nhật.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Success"] = "Cập nhật thành công.";
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi khi cập nhật: " + ex.Message;
                ViewBag.Vehicles = new SelectList(await _orderRepo.GetAvailableVehiclesAsync(), "Id", "Name");
                ViewBag.StatusList = new List<SelectListItem>
                {
                    new SelectListItem("processing", "processing", model.Status == "processing"),
                    new SelectListItem("completed", "completed", model.Status == "completed"),
                    new SelectListItem("cancelled", "cancelled", model.Status == "cancelled")
                };
                ViewBag.PaymentStatusList = new List<SelectListItem>
                {
                    new SelectListItem("unpaid", "unpaid", model.PaymentStatus == "unpaid"),
                    new SelectListItem("paid", "paid", model.PaymentStatus == "paid"),
                    new SelectListItem("refunded", "refunded", model.PaymentStatus == "refunded")
                };
                return View(model);
            }
        }

        // POST Delete (soft delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (!Guid.TryParse(userIdStr, out var userId)) return RedirectToAction("Login", "Account");

            // Only creator can delete
            var canDelete = await _orderRepo.IsOrderCreatedByUserAsync(id, userId);
            if (!canDelete) return RedirectToAction("Forbidden", "Home");

            var ok = await _orderRepo.SoftDeleteOrderAsync(id, userId);
            if (!ok) return NotFound();

            TempData["Success"] = "Đã huỷ đơn hàng.";
            return RedirectToAction(nameof(Index));
        }
    }
}
