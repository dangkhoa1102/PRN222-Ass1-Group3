using BusinessObjects.DTO;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Service;

namespace Group03_MVC.Controllers.Api
{
    [ApiController]
    [Route("api/order")]
    public class OrderApiController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderApiController(OrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns>List of all orders</returns>
        [HttpGet]
        public async Task<ActionResult<List<Orderdto>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrders();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get orders by status
        /// </summary>
        /// <param name="status">Order status (Pending, Confirmed, Completed)</param>
        /// <returns>List of orders with specified status</returns>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<List<Orderdto>>> GetOrdersByStatus(string status)
        {
            try
            {
                var orders = await _orderService.GetByStatus(status);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get customer orders
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>List of customer orders</returns>
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<List<Orderdto>>> GetCustomerOrders(Guid customerId)
        {
            try
            {
                var orders = await _orderService.GetCustomerOrders(customerId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Order details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Orderdto>> GetOrder(Guid id)
        {
            try
            {
                var order = await _orderService.GetOrderById(id);
                if (order == null)
                {
                    return NotFound(new { message = "Order not found" });
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="createOrderDto">Order creation data</param>
        /// <returns>Created order result</returns>
        [HttpPost]
        public async Task<ActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _orderService.CreateOrder(createOrderDto);
                if (success)
                {
                    return Ok(new { message = "Order created successfully" });
                }
                else
                {
                    return BadRequest(new { message = "Failed to create order. Vehicle may not be available." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Confirm an order (Staff only)
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Confirmation result</returns>
        [HttpPost("{id}/confirm")]
        public async Task<ActionResult> ConfirmOrder(Guid id, [FromBody] ConfirmOrderDto confirmDto)
        {
            try
            {
                // N?u b?n ch?a có Auth thì t?m l?y staffId t? body
                var success = await _orderService.ConfirmOrder(id, confirmDto.StaffId);

                if (success)
                {
                    return Ok(new { message = "Order confirmed successfully" });
                }
                else
                {
                    return BadRequest(new { message = "Failed to confirm order. Order may not be in Pending status." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Complete payment for an order (Customer only)
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Payment completion result</returns>
        [HttpPost("{id}/payment")]
        public async Task<ActionResult> CompletePayment(Guid id, [FromBody] ProcessPaymentDto paymentDto)
        {
            try
            {
                // ví d?: gi? s? l?y customerId t? token claims
                var customerId = Guid.Parse(User.FindFirst("id").Value);

                var success = await _orderService.CompletePayment(id, customerId);

                if (success)
                    return Ok(new { message = "Payment completed successfully" });

                return BadRequest(new { message = "Failed to complete payment. Order may not be in Confirmed status." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }


    }
}
