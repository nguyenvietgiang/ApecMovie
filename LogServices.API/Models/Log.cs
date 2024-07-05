using LogServices.API.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace LogServices.API.Models
{
    [BsonIgnoreExtraElements]
    public class Log
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Timestamp")]
        public DateTime Timestamp { get; set; }

        [BsonElement("Level")]
        public string Level { get; set; }

        [BsonElement("MessageTemplate")]
        public string MessageTemplate { get; set; }

        [BsonElement("RenderedMessage")]
        public string RenderedMessage { get; set; }

        [BsonElement("Properties")]
        public Properties Properties { get; set; }

        [BsonElement("UtcTimestamp")]
        [BsonSerializer(typeof(CustomDateTimeSerializer))]
        public DateTime UtcTimestamp { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class Properties
    {
        [BsonElement("FullName")]
        public string FullName { get; set; }

        [BsonElement("EventId")]
        public EventId EventId { get; set; }

        [BsonElement("SourceContext")]
        public string SourceContext { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class EventId
    {
        [BsonElement("Id")]
        public int Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }
    }
}

