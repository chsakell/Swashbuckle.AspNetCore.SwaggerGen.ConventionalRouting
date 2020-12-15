﻿using System.Runtime.Serialization;

namespace Web.API.Models
{
    [DataContract]
    public class Tag
    {
        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id")]
        public long? Id { get; set; }

        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
