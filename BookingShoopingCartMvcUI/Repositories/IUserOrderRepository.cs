using BookingShoopingCartMvcUI.Models;

namespace BookingShoopingCartMvcUI.Repositories
{
    public interface IUserOrderRepository
    {
        Task<IEnumerable<Order>> UserOrders(bool getAll = false);
  
        Task TogglePaymentStatus(int orderId);
        Task<Order?> GetOrderById(int id);
        Task<IEnumerable<OrderStatus>> GetOrderStatuses();

    }
}