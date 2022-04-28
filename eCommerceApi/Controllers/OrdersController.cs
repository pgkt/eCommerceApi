using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using eCommerceApi.Domain.Entities;
using eCommerceApi.Domain.Interfaces;
using eCommerceApi.DTOs;
using eCommerceApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eCommerceApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : BaseController
    {
        private IApplicationDbContext _dbContext { get; }
        private IMapper _mapper;
        private ILogger<OrdersController> _logger;

        public OrdersController(IApplicationDbContext dbContext, IMapper mapper, ILogger<OrdersController> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        [Route("CreateOrder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto order)
        {
            try
            {
                var orderObj = _mapper.Map<Order>(order);
                orderObj.CreatedOn = DateTime.UtcNow;
                orderObj.CreatedBy = GetUser();

                await _dbContext.Orders.AddAsync(orderObj);
                await _dbContext.SaveChangesAsync(default);

                return Ok(orderObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error occured while adding Product");
            }
        }

        [HttpPost]
        [Route("CreateOrderItems/{OrderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrderItems(int OrderId, [FromBody] List<OrderItemDto> orderItems)
        {
            try
            {
                var items = _mapper.Map<List<OrderItem>>(orderItems);
                foreach(var item in items)
                {
                    item.OrderId = OrderId;
                }

                await _dbContext.OrderItems.AddRangeAsync(items);
                await _dbContext.SaveChangesAsync(default);

                return Ok(new { message="Order items added successfully"});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error occured while adding Product");
            }
        }

        [HttpGet]
        [Route("GetOrder/{OrderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrder(int OrderId)
        {
            try
            {
                var order = await _dbContext.Orders.Where(o => o.OrderId == OrderId && (o.Status != "Deleted" || o.Status != "Cancelled")).FirstOrDefaultAsync();
                
                if (order == null)
                {
                    return Ok(new { message = "Order not found" });
                }
                var orderInfo = _mapper.Map<OrderDto>(order);
               
                return Ok(orderInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error occured while adding Product");
            }
        }

        [HttpGet]
        [Route("GetOrderItems/{OrderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrderItems(int OrderId)
        {
            try
            {
                if(!_dbContext.Orders.Any(o => o.OrderId == OrderId && (o.Status != "Deleted" || o.Status != "Cancelled")))
                {
                    return Ok(new { message = "Order not found" });
                }
                var items = await _dbContext.OrderItems.Where(oi => oi.OrderId == OrderId && oi.isDeleted == false).ToListAsync();
                var products = await _dbContext.Products.ToListAsync();
                var orderItems = _mapper.Map<List<OrderItemViewModel>>(items);
                foreach (var item in orderItems)
                {
                    item.ProductName = products.Where(p => p.ProductId == item.ProductId).FirstOrDefault().ProductName;
                }

                return Ok(orderItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error occured while adding Product");
            }
        }

        [HttpGet]
        [Route("GetOrderList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrderList()
        {
            try
            {
                var user = GetUser();
                var orders = await _dbContext.Orders.Where(o => o.CreatedBy == user && (o.Status != "Deleted" || o.Status != "Cancelled")).ToListAsync();

                if (orders == null)
                {
                    return Ok(new { message = "Orders not found" });
                }
                var orderList = _mapper.Map<List<OrderDto>>(orders);

                return Ok(orderList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception("Error occured while adding Product");
            }
        }
    }
}
