﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace Onyx.Models.Domain
{
    public class ShortProcessDataModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public ObjectId Id { get; set; }

        [JsonPropertyName("id")]
        public string IdAsString => Id.ToString();

        [BsonElement("DUT")]
        [JsonPropertyName("DUT")]
        public DUTHeaderModel DUT { get; set; }
    }
}
