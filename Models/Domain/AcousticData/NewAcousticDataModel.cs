using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Onyx.Models.Domain.AcousticData
{
    public class NewAcousticDataModel
    {
        [BsonElement("DUT")]
        [JsonPropertyName("DUT")]
        public AcousticDutModel DUT { get; set; }

        [BsonElement("Steps")]
        [JsonPropertyName("Steps")]
        public List<AcousticStepModel> Steps { get; set; } = new List<AcousticStepModel>();
    }

    public class AcousticDutModel
    {
        [BsonElement("dutpass")]
        [JsonPropertyName("dutpass")]
        public bool Pass { get; set; }

        [BsonElement("dutclass")]
        [JsonPropertyName("dutclass")]
        public string DutClass { get; set; }

        [BsonElement("typeid")]
        [JsonPropertyName("typeid")]
        public string TypeID { get; set; }

        [BsonElement("typename")]
        [JsonPropertyName("typename")]
        public string TypeName { get; set; }

        [BsonElement("system")]
        [JsonPropertyName("system")]
        public string TestSystem { get; set; }

        [BsonElement("workorder")]
        [JsonPropertyName("workorder")]
        public string WorkOrder { get; set; }

        [BsonElement("runningnr")]
        [JsonPropertyName("runningnr")]
        public int RunningNr { get; set; }

        [BsonElement("serialnr")]
        [JsonPropertyName("serialnr")]
        public string SerialNr { get; set; }

        [BsonElement("executiontime(s)")]
        [JsonPropertyName("executiontime(s)")]
        public float ExecutionTime { get; set; }

        [BsonElement("duttime")]
        [JsonPropertyName("duttime")]
        public DateTime DutTime { get; set; }

        [BsonElement("nestnumber")]
        [JsonPropertyName("nestnumber")]
        public int NestNumber { get; set; }
    }

    public class AcousticStepModel
    {
        [BsonElement("stepname")]
        [JsonPropertyName("stepname")]
        public string Stepname { get; set; }

        [BsonElement("steppass")]
        [JsonPropertyName("steppass")]
        public bool StepPass { get; set; }

        [BsonElement("unitx")]
        [JsonPropertyName("unitx")]
        public string UnitX { get; set; }

        [BsonElement("unity")]
        [JsonPropertyName("unity")]
        public string UnitY { get; set; }

        [BsonElement("measurement")]
        [JsonPropertyName("measurement")]
        public List<List<decimal>> Measurement { get; set; }

        [BsonElement("upperlimit")]
        [JsonPropertyName("upperlimit")]
        public List<List<decimal>> UpperLimit { get; set; }

        [BsonElement("lowerlimit")]
        [JsonPropertyName("lowerlimit")]
        public List<List<decimal>> LowerLimit { get; set; }
    }
}
