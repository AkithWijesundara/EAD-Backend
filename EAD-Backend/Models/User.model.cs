/*
 * File: User Database model
 * Author: Perera V. H. P.
 * Description: This file contains Database model for user.
 * Created: 07/10/2024
*/


using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EAD_Backend.Models
{
    public class User
    {

        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public required string Name { get; set; }

        [BsonElement("email"), BsonRepresentation(BsonType.String)]
        public required string Email { get; set; }

        [BsonElement("password"), BsonRepresentation(BsonType.String)]
        public required string Password { get; set; }

        [BsonElement("role"), BsonRepresentation(BsonType.ObjectId)]
        public required string Role { get; set; }

        [BsonElement("status"), BsonRepresentation(BsonType.String)]
        public string? Status { get; set; }

        [BsonElement("profileImage"), BsonRepresentation(BsonType.String)]
        public string? ProfileImage { get; set; }

        [BsonElement("otp"), BsonRepresentation(BsonType.String)]
        public string? Otp { get; set; }

        [BsonElement("active"), BsonRepresentation(BsonType.Boolean)]
        public bool? Active { get; set; }

        [BsonElement("isFirstLogin"), BsonRepresentation(BsonType.Boolean)]
        public bool? IsFirstLogin { get; set; }

    }
}