/*
File: Notification.model.cs
Author: Rathnayaka M.R.T.N
Description: Notification model class for the EAD project
created:  02/10/2024
*/


using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace EAD_Backend.Models

{
    public class Notification
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("title"), BsonRepresentation(BsonType.String)]
        public required string Title { get; set; }

        [BsonElement("message"), BsonRepresentation(BsonType.String)]
        public required string Message { get; set; }

        [BsonElement("IsRead"), BsonRepresentation(BsonType.Boolean)]
        public bool IsRead { get; set; } = false;

        [BsonElement("userId"), BsonRepresentation(BsonType.ObjectId)]
        public string? UserId { get; set; }

        [BsonElement("createdOn"), BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }

}
