using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EAD_Backend.Models
{
    public class Role
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public required string Name { get; set; }
    }
}