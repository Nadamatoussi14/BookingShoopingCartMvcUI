﻿using BookingShoopingCartMvcUI.Data;
using BookingShoopingCartMvcUI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookingShoopingCartMvcUI.Repositories
{
    public class UserOrderRepository :IUserOrderRepository
    {
      
            private readonly ApplicationDbContext _db;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly UserManager<IdentityUser> _userManager;


            public UserOrderRepository(ApplicationDbContext db,
                UserManager<IdentityUser> userManager,
                 IHttpContextAccessor httpContextAccessor)
            {
                _db = db;
                _httpContextAccessor = httpContextAccessor;
                _userManager = userManager;
            }

            
            public async Task<Order?> GetOrderById(int id)
            {
                return await _db.Orders.FindAsync(id);
            }

            public async Task<IEnumerable<OrderStatus>> GetOrderStatuses()
            {
                return await _db.OrderStatuses.ToListAsync();
            }

            public async Task TogglePaymentStatus(int orderId)
            {
                var order = await _db.Orders.FindAsync(orderId);
                if (order == null)
                {
                    throw new InvalidOperationException($"order withi id:{orderId} does not found");
                }
                order.IsPaid = !order.IsPaid;
                await _db.SaveChangesAsync();
            }

            public async Task<IEnumerable<Order>> UserOrders(bool getAll = false)
            {
                var orders = _db.Orders
                               .Include(x => x.OrderStatus)
                               .Include(x => x.OrderDetail)
                               .ThenInclude(x => x.Book)
                               .ThenInclude(x => x.Genre).AsQueryable();
                if (!getAll)
                {
                    var userId = GetUserId();
                    if (string.IsNullOrEmpty(userId))
                        throw new Exception("User is not logged-in");
                    orders = orders.Where(a => a.UserId == userId);
                    return await orders.ToListAsync();
                }

                return await orders.ToListAsync();
            }

            private string GetUserId()
            {
                var principal = _httpContextAccessor.HttpContext.User;
                string userId = _userManager.GetUserId(principal);
                return userId;
            }
        }
}
