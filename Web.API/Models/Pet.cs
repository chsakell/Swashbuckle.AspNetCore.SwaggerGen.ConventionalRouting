using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PetStore.API.Models
{
    public class Pet
    {
        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        [DataMember(Name = "id")]
        public long? Id { get; set; }

        /// <summary>
        /// Gets or Sets Category
        /// </summary>
        [DataMember(Name = "category")]
        public Category Category { get; set; }

        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [Required]
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets PhotoUrls
        /// </summary>
        [Required]
        [DataMember(Name = "photoUrls")]
        public List<string> PhotoUrls { get; set; }

        /// <summary>
        /// Gets or Sets Tags
        /// </summary>
        [DataMember(Name = "tags")]
        public List<Tag> Tags { get; set; }

        /// <summary>
        /// pet status in the store
        /// </summary>
        /// <value>pet status in the store</value>
        [DataMember(Name = "status")]
        public PetStatus? Status { get; set; }

        public Pet()
        {
            PhotoUrls = new List<string>();
            Tags = new List<Tag>();
        }
    }
}
