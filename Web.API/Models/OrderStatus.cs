using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Web.API.Models
{
    /// <summary>
    /// Order Status
    /// </summary>
    /// <value>Order Status</value>
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum OrderStatus
    {

        /// <summary>
        /// Enum PlacedEnum for placed
        /// </summary>
        [EnumMember(Value = "placed")]
        PlacedEnum = 1,

        /// <summary>
        /// Enum ApprovedEnum for approved
        /// </summary>
        [EnumMember(Value = "approved")]
        ApprovedEnum = 2,

        /// <summary>
        /// Enum DeliveredEnum for delivered
        /// </summary>
        [EnumMember(Value = "delivered")]
        DeliveredEnum = 3
    }
}
