using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDataAccess.Models;
public class JobModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }  = string.Empty;
    public string JobText { get; set; } = string.Empty;
    public int FrequencyInDays { get; set; }
    public UserModel? AssignedTo { get; set; }
    public DateTime? LastCompleted { get; set; }
}
