using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PetStore.API.Models
{
    /// <summary>
    /// pet status in the store
    /// </summary>
    /// <value>pet status in the store</value>
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum PetStatus
    {

        /// <summary>
        /// Enum AvailableEnum for available
        /// </summary>
        [EnumMember(Value = "available")]
        AvailableEnum = 1,

        /// <summary>
        /// Enum PendingEnum for pending
        /// </summary>
        [EnumMember(Value = "pending")]
        PendingEnum = 2,

        /// <summary>
        /// Enum SoldEnum for sold
        /// </summary>
        [EnumMember(Value = "sold")]
        SoldEnum = 3
    }
}
