using System.Runtime.Serialization;

namespace Web.API.Models
{
    [DataContract]
    public class ApiResponse
    {
        /// <summary>
        /// Gets or Sets Code
        /// </summary>
        [DataMember(Name = "code")]
        public int? Code { get; set; }

        /// <summary>
        /// Gets or Sets Type
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or Sets Message
        /// </summary>
        [DataMember(Name = "message")]
        public string Message { get; set; }
    }
}
