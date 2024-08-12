using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Onyx.Models.Domain.ProcessData
{
    public class NewProcessDataModel
    {
        [BsonElement("DUT")]
        [JsonPropertyName("DUT")]
        public ProcessDutModel DUT { get; set; }

        [BsonElement("Steps")]
        [JsonPropertyName("Steps")]
        public List<ProcessStepModel> Steps { get; set; } = new List<ProcessStepModel>();
    }

    public class ProcessDutModel
    {
        [BsonElement("serial_nr")]
        [JsonPropertyName("serial_nr")]
        public string SerialNr { get; set; }

        [BsonElement("type_id")]
        [JsonPropertyName("type_id")]
        public int TypeID { get; set; }

        [BsonElement("country_code")]
        [JsonPropertyName("country_code")]
        public string CountryCode { get; set; } = null!;

        [BsonElement("system_type")]
        [JsonPropertyName("system_type")]
        public int SystemType { get; set; }

        [BsonElement("track_nr")]
        [JsonPropertyName("track_nr")]
        public int? Track { get; set; }

        [BsonElement("ps01_press_nr")]
        [JsonPropertyName("ps01_press_nr")]
        public int? Press { get; set; }

        [BsonElement("wpc_number")]
        [JsonPropertyName("wpc_number")]
        public int? WpcNumber { get; set; }

        [BsonElement("wpc_height")]
        [JsonPropertyName("wpc_height")]
        public int? WpcHeight { get; set; }

        [BsonElement("machine_id")]
        [JsonPropertyName("machine_id")]
        public string Line { get; set; } = null!;
    }

    public class ProcessStepModel
    {
        [BsonElement("stepname")]
        [JsonPropertyName("stepname")]
        public string Stepname { get; set; } = null!;

        [BsonElement("unitx")]
        [JsonPropertyName("unitx")]
        public string UnitX { get; set; } = null!;

        [BsonElement("unity")]
        [JsonPropertyName("unity")]
        public string UnitY { get; set; } = null!;

        [BsonElement("Measurements")]
        [JsonPropertyName("Measurements")]
        public List<ProcessMeasurementModel> Measurements { get; set; } = new List<ProcessMeasurementModel>();
    }

    public class ProcessMeasurementModel
    {
        [BsonElement("Date")]
        [JsonPropertyName("Date")]
        public string TimeStamp { get; set; } = null!;

        [BsonElement("MeasurementValue")]
        [JsonPropertyName("MeasurementValue")]
        public string Value { get; set; } = "";
    }
}
