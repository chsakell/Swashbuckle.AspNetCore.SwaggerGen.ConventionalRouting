using System;
using System.Runtime.Serialization;

namespace Web.API.Models
{
    [DataContract]
    public class Order
    {
        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id")]
        public long? Id { get; set; }

        /// <summary>
        /// Gets or Sets PetId
        /// </summary>
        [DataMember(Name = "petId")]
        public long? PetId { get; set; }

        /// <summary>
        /// Gets or Sets Quantity
        /// </summary>
        [DataMember(Name = "quantity")]
        public int? Quantity { get; set; }

        /// <summary>
        /// Gets or Sets ShipDate
        /// </summary>
        [DataMember(Name = "shipDate")]
        public DateTime? ShipDate { get; set; }

        /// <summary>
        /// Order Status
        /// </summary>
        /// <value>Order Status</value>
        [DataMember(Name = "status")]
        public OrderStatus? Status { get; set; }

        /// <summary>
        /// Gets or Sets Complete
        /// </summary>
        [DataMember(Name = "complete")]
        public bool? Complete { get; set; }
    }
}
