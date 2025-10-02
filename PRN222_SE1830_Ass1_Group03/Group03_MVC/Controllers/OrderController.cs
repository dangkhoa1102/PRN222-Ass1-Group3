using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Service;

namespace Group03_MVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public IActionResult History()
        {
            var userId = HttpContext.Session.GetString("UserId");
            Guid id = Guid.Parse(userId);
            // 1. Lấy dữ liệu lịch sử order từ Service/Repository
            List<Order> orders = _orderService.GetOrderByUserId(id).Result;
            // 2. Truyền dữ liệu đó vào View
            return View(orders); // Trả về View tương ứng (History.cshtml)
        }
    }
}
