using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order;

namespace Talabat.Core.Services.interfaces
{
    public interface IOrderService
    {
        Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shoppingAddress);


        Task<IReadOnlyList<Order>?> GetOrdersForSpecificUserAsync(string buyerEmail);


        Task<Order?> GetOneOrderForSpecificUserAsync(string buyerEmail, int OrderId);


    }
}
