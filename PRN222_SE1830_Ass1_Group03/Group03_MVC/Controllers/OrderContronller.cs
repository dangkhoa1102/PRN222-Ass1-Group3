using BusinessObjects.DTO;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Service;

namespace VehicleDealer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetAll()
        {
            return Ok(await _orderService.GetAllOrders());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById(Guid id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult> Add(OrderDTO dto)
        {
            var success = await _orderService.AddOrder(dto);
            if (!success) return BadRequest("Không thể tạo đơn hàng");
            return Ok("Tạo đơn hàng thành công");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, OrderDTO dto)
        {
            dto.Id = id;
            var success = await _orderService.UpdateOrder(dto);
            if (!success) return NotFound("Không tìm thấy đơn hàng để cập nhật");
            return Ok("Cập nhật đơn hàng thành công");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var success = await _orderService.DeleteOrder(id);
            if (!success) return NotFound("Không tìm thấy đơn hàng để xóa");
            return Ok("Xóa đơn hàng thành công");
        }
    }
}
