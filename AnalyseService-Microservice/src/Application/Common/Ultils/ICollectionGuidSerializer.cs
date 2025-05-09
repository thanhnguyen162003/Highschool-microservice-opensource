// using MongoDB.Bson;
// using MongoDB.Bson.Serialization;
// using MongoDB.Bson.Serialization.Serializers;
//
// namespace Application.Common.Ultils;
//
// public class ICollectionGuidSerializer : SerializerBase<ICollection<Guid>>
// {
//     public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, ICollection<Guid> value)
//     {
//         // Serialize ICollection<Guid> as a list of GUIDs
//         context.Writer.WriteStartArray();
//         foreach (var guid in value)
//         {
//             context.Writer.WriteBinaryData(new BsonBinaryData(guid, GuidRepresentation.CSharpLegacy));
//         }
//         context.Writer.WriteEndArray();
//     }
//
//     public override ICollection<Guid> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
//     {
//         // Deserialize a BSON array back into ICollection<Guid>
//         var list = new List<Guid>();
//         context.Reader.ReadStartArray();
//         while (context.Reader.ReadBsonType() != BsonType.EndOfDocument)
//         {
//             var binaryData = context.Reader.ReadBinaryData();
//             list.Add(binaryData.ToGuid());
//         }
//         context.Reader.ReadEndArray();
//         return list;
//     }
// }
