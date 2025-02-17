using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace EAD_Backend.Models
{
    public class Order
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? OrderId {get; set;}

        [BsonElement("orderNo"), BsonRepresentation(BsonType.String)]
        public required string OrderNo {get; set;}

        [BsonElement("customer"), BsonRepresentation(BsonType.ObjectId)]
        public required string CustomerNo {get; set;}

        [BsonElement("deliveryAddress"), BsonRepresentation(BsonType.String)]
        public required string DeliveryAddress {get; set;}

        [BsonElement("orderDate"), BsonRepresentation(BsonType.DateTime)]
        public required DateTime OrderDate {get; set;}

        [BsonElement("status"), BsonRepresentation(BsonType.String)]
        public required string Status {get; set;}

        [BsonElement("isCancelRequested"), BsonRepresentation(BsonType.Boolean)]
        public Boolean? IsCancelRequested {get; set;}

        [BsonElement("comments"), BsonRepresentation(BsonType.String)]
        public string? Comments {get; set;}
    }
}