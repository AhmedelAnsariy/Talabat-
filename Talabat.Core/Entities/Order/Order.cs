using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Order
{
    public class Order : BaseEntity
    {





        public Order()
        {

        }




        public Order(string buyerEmail, Address shippingAddress, DeliveryMethods deliveryMethods, ICollection<OrderItem> items, decimal subTotal , string paymentIntentId)
        {
            BuyerEmail = buyerEmail;
            ShippingAddress = shippingAddress;
            DeliveryMethods = deliveryMethods;
            Items = items;
            SubTotal = subTotal;
            PaymentIntetId = paymentIntentId;
        }


       

        public string BuyerEmail { get; set; }

        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public Address ShippingAddress { get; set; }



        public int DeliveryMethodsId { get; set; }
        public DeliveryMethods DeliveryMethods { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>();

        public decimal SubTotal { get; set; }

        public decimal GetTotal() => SubTotal + DeliveryMethods.Cost;


        public string PaymentIntetId { get; set; } = string.Empty;
    }
}
