using BusinessObject;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace eStore.Models
{
    [DataContract]
    public class CartItem
    {
        [DataMember]
        public int quantity { get; set; }
        [DataMember]
        public int productId { get; set; }
        [DataMember]
        public double discount { get; set; }

        public Product product { get; set; }
    }
}
