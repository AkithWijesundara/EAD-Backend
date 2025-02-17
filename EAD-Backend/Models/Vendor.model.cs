/*
 * File: Vendor Database model
 * Author: Perera V. H. P.
 * Description: This file contains Database model for vendor.
 * Created: 07/10/2024
*/


using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EAD_Backend.Models
{
    public class Vendor
    {

        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Name { get; set; }

    }
}