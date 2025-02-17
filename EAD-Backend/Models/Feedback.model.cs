/*
 * File: Feedback Database model
 * Author: Udumulla C.J
 * Description: This file contains Database model for Reviews.
 * Created: 08/10/2024
*/


using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EAD_Backend.Models
{
    public class Feedback
    {
        [BsonId]  // Automatically handles ObjectId conversion
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("customerId"), BsonRepresentation(BsonType.ObjectId)]  // No need for BsonRepresentation for strings
        public string? CustomerId { get; set; }

        [BsonElement("productId"), BsonRepresentation(BsonType.ObjectId)]
        public string? ProductId { get; set; }

        [BsonElement("message")]
        public string? Message { get; set; }

        [BsonElement("rating")]  // For int, you don't need BsonRepresentation either
        public int? Rating { get; set; }
    }
}
