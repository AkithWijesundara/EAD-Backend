/*
 * File: Category model
 * Author: Perera V. H. P.
 * Description: This file contains Database model for product Category.
 * Created: 07/10/2024
*/

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EAD_Backend.Models
{
    public class Category
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public required string Name { get; set; }
    }
}