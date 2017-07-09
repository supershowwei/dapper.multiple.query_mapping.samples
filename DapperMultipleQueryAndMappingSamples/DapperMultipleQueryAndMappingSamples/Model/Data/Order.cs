using System.Collections.Generic;
using System.Linq;

namespace DapperMultipleQueryAndMappingSamples.Model.Data
{
    public class Order
    {
        public int Id { get; set; }

        public Customer Customer { get; set; }

        public string PaymentMethod { get; set; }

        public string DeliveryMethod { get; set; }

        public List<Product> Products { get; set; }

        public int TotalPrice
        {
            get
            {
                return this.Products?.Sum(x => x.Price) ?? 0;
            }
        }
    }
}