using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Onyx.Models.Domain.AcousticData
{
    public class AcousticDataModel : NewAcousticDataModel
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
