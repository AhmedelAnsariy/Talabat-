using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order;

namespace Talabat.Core.Specifications.OrderSpecifications
{
    public class OrderSpecs : BaseSpecifications<Order>
    {

        public OrderSpecs(string email) : base ( o=> o.BuyerEmail == email)
        {
            Includes.Add(o => o.DeliveryMethods);
            Includes.Add(o => o.Items);
            AddOrderByDesc(o => o.OrderDate);
        }


        public OrderSpecs(string email, int orderId)
       : base(o => o.BuyerEmail == email && o.Id == orderId)
        {
            Includes.Add(o => o.DeliveryMethods);
            Includes.Add(o => o.Items);
        }




    }
}
