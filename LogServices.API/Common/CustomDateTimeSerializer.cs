using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;

namespace LogServices.API.Common
{
    public class CustomDateTimeSerializer : SerializerBase<DateTime>
    {
        private static readonly string[] DateTimeFormats =
        {
        "yyyy-MM-ddTHH:mm:ss.fffZ",
        "yyyy-MM-ddTHH:mm:ssZ",
        "yyyy-MM-dd HH:mm:ssZ"
    };

        public override DateTime Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;
            var dateTimeAsString = bsonReader.ReadString();
            return DateTime.ParseExact(dateTimeAsString, DateTimeFormats, null, System.Globalization.DateTimeStyles.AdjustToUniversal);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateTime value)
        {
            var bsonWriter = context.Writer;
            bsonWriter.WriteString(value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
        }
    }
}
