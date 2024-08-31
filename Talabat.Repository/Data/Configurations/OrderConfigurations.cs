using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order;

namespace Talabat.Repository.Data.Configurations
{
    public class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
           builder.Property(o=>o.SubTotal).HasColumnType("decimal(18,2)");


            builder.Property(o => o.Status)
                     .HasConversion(
                      ostatus => ostatus.ToString(),
                      ostatus => (OrderStatus)Enum.Parse(typeof(OrderStatus), ostatus)
                      );




            builder.OwnsOne(o => o.ShippingAddress, sa => sa.WithOwner());

            builder.HasOne(o => o.DeliveryMethods)
                   .WithMany()
                   .HasForeignKey(o => o.DeliveryMethodsId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
