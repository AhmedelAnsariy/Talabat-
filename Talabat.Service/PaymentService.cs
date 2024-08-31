using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using CustomerBasket = Talabat.Core.Entities.CustomerBasket;
using Product = Talabat.Core.Entities.Product;
using Talabat.Core.Entities.Order;
using Talabat.Core.Interfaces;
using Talabat.Core.Specifications.OrderSpecifications;

namespace Talabat.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public PaymentService(IBasketRepository basketRepository , IUnitOfWork unitOfWork , IConfiguration configuration)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }



        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _basketRepository.GetBasketAsync(basketId);
            if (basket == null)
            {
                return null;
            }

            if (basket.Items.Count() > 0)
            {
                foreach (var item in basket.Items)
                {
                    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                    if (item.Price != product.Price)
                    {
                        item.Price = product.Price;
                    }
                }
            }

            var subtotal = basket.Items.Sum(i => i.Price * i.Quantity);

            decimal shippingPrice = 0;
            if (basket.DeliveryMethodId.HasValue)
            {
                var deliverymethod = await _unitOfWork.Repository<DeliveryMethods>().GetByIdAsync(basket.DeliveryMethodId.Value);
                if (deliverymethod != null)
                {
                    shippingPrice = deliverymethod.Cost;
                }
            }

            StripeConfiguration.ApiKey = _configuration["StripeKeys:Secretkey"];
            var service = new PaymentIntentService();
            PaymentIntent paymentIntent;

            var totalAmount = (subtotal + shippingPrice) * 100; // Convert total to cents

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)totalAmount,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" }
                };
                paymentIntent = await service.CreateAsync(options);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)totalAmount,
                };

                paymentIntent = await service.UpdateAsync(basket.PaymentIntentId, options);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }

            await _basketRepository.UpdateBasketAsync(basket);
            return basket;
        }

        public async Task<Order> UpdatePaymentIntentToSuccessedOrFailed(string paymentIntentId, bool flag)
        {
            var spec = new OrderWithPaymentIntentIdSpecifications(paymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetWithSpecByIdAsync(spec);

           

            if (flag)
            {
                order.Status = OrderStatus.PaymentReceived;
            }
            else
            {
                order.Status = OrderStatus.PaymentFailed;
            }

            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.CompeleteAsync();

            return order;
        }

    }
}
