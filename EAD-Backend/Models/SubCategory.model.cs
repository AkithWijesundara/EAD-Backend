
/*
 * File: Product Sub Category Database model
 * Author: Perera V. H. P.
 * Description: This file contains Database model for Product Sub Category.
 * Created: 07/10/2024
*/

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EAD_Backend.Models
{
    public class SubCategory
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public string Name { get; set; }

        [BsonElement("category"), BsonRepresentation(BsonType.ObjectId)]
        public string CategoryId { get; set; }
    }
}