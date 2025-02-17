using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace EAD_Backend.Models
{
    public class OrderLine
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public required string OrderLineNo {get; set;}

        [BsonElement("productNo"), BsonRepresentation(BsonType.ObjectId)]
        public required string ProductNo {get; set;}

        [BsonElement("orderNo"), BsonRepresentation(BsonType.String)]
        public required string OrderNo {get; set;}

        [BsonElement("vendorNo"), BsonRepresentation(BsonType.ObjectId)]
        public required string VendorNo {get; set;}

        [BsonElement("status"), BsonRepresentation(BsonType.String)]
        public required string Status {get; set;}

        [BsonElement("qty"), BsonRepresentation(BsonType.Int32)]
        public required int Qty {get; set;}

        [BsonElement("unitPrice"), BsonRepresentation(BsonType.Double)]
        public required float UnitPrice {get; set;}

        [BsonElement("total"), BsonRepresentation(BsonType.Double)]
        public required float Total {get; set;}

    }
}