using Talabat.Core.Entities;

namespace Talabat.APIS.DTO
{
    public class CustomerBasketDto
    {
        public string Id { get; set; }

        public List<BasketItems> Items { get; set; }


        public int? DeliveryMethodId { get; set; }

        public string? PaymentIntentId { get; set; }

        public string? ClientSecret { get; set; }
    }
}
