using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;
using Talabat.Core.Interfaces;
using Talabat.Core.Services.interfaces;
using Talabat.Core.Specifications.OrderSpecifications;

namespace Talabat.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService(IBasketRepository basketRepository ,
          IUnitOfWork unitOfWork,
          IPaymentService paymentService
            )
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }



        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shoppingAddress)
        {
            var basket = await _basketRepository.GetBasketAsync(basketId);

            if (basket == null || !basket.Items.Any())
            {
                return null; // or throw an exception, depending on how you want to handle this case
            }

            var orderItems = new List<OrderItem>();

            foreach (var item in basket.Items)
            {
                var oneProduct = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);

                if(oneProduct == null)
                {
                    return null;
                }


                var productItemOrdered = new ProductItemOrdered(oneProduct.Id, oneProduct.Name, oneProduct.PictureUrl);
                var orderItem = new OrderItem(productItemOrdered, item.Price, item.Quantity);
                orderItems.Add(orderItem);
            }

            var subTotal = orderItems.Sum(oi => oi.Price * oi.Quantity);
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethods>().GetByIdAsync(deliveryMethodId);



            var specs = new OrderWithPaymentIntentIdSpecifications(basket.PaymentIntentId);
            var ExOrder = await _unitOfWork.Repository<Order>().GetWithSpecByIdAsync(specs);

            if(ExOrder is not null)
            {
                _unitOfWork.Repository<Order>().Delete(ExOrder);
                basket =   await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            }
            
            var order = new Order(buyerEmail, shoppingAddress, deliveryMethod, orderItems, subTotal ,basket.PaymentIntentId);

            await _unitOfWork.Repository<Order>().AddAsync(order);

            var result =  await _unitOfWork.CompeleteAsync();
            if(result <= 0)
            {
                return null;
            }
            return order;
        }

        public async Task<Order?> GetOneOrderForSpecificUserAsync(string buyerEmail, int OrderId)
        {
            var spec = new OrderSpecs(buyerEmail, OrderId);

            var order = await _unitOfWork.Repository<Order>().GetWithSpecByIdAsync(spec);

            if(order is null) { return null; }

            return order;
        }

        public async Task<IReadOnlyList<Order>?> GetOrdersForSpecificUserAsync(string buyerEmail)
        {
            var spec = new OrderSpecs(buyerEmail);
            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);
            if(orders is null || orders.Count() ==0)
                return null;

            return orders;
            
        }



    }
}
