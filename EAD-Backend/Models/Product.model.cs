/*
 * File: Product Database model
 * Author: Perera V. H. P.
 * Description: This file contains Database model for product.
 * Created: 07/10/2024
*/


using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EAD_Backend.Models
{
    public class Product
    {

        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public required string Name { get; set; }

        [BsonElement("description"), BsonRepresentation(BsonType.String)]
        public required string Description { get; set; }

        [BsonElement("category"), BsonRepresentation(BsonType.ObjectId)]
        public required string Category { get; set; }

        [BsonElement("subCategory"), BsonRepresentation(BsonType.ObjectId)]
        public required string SubCategory { get; set; }


        [BsonElement("price"), BsonRepresentation(BsonType.Double)]
        public required double Price { get; set; }

        [BsonElement("images")]
        public string[] Images { get; set; } = Array.Empty<string>();

        [BsonElement("active"), BsonRepresentation(BsonType.Boolean)]
        public bool? Active { get; set; }

        [BsonElement("stockCount"), BsonRepresentation(BsonType.Int32)]
        public required int StockCount { get; set; }

        [BsonElement("vendorId"), BsonRepresentation(BsonType.ObjectId)]
        public string? VendorId { get; set; }

        [BsonElement("LowStockThreshold"), BsonRepresentation(BsonType.Int32)]
        public int LowStockThreshold { get; set; } = 10;

        [BsonElement("IsPartOfPendingOrder"), BsonRepresentation(BsonType.Boolean)]
        public bool IsPartOfPendingOrder { get; set; } = false;

        [BsonIgnore]
        public string? SubCategoryName { get; set; }


    }
}