using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Onyx.Models.Domain
{
    public class ProcessDataModel : NewProcessDataModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        [Required]
        public ObjectId Id { get; set; }

        [JsonPropertyName("id")]
        public string IdAsString => Id.ToString();
    }
}
