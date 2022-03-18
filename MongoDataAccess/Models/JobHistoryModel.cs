using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MongoDataAccess.Models;
public class JobHistoryModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } 
    public string JobId { get; set; }
    public string JobText { get; set; }
    public DateTime DateCompleted { get; set; }
    public UserModel WhoCompleted { get; set; }

    public JobHistoryModel()
    {

    }

    public JobHistoryModel(JobModel job)
    {
        JobId = job.Id;
        DateCompleted = job.LastCompleted ?? DateTime.Now;
        WhoCompleted = job.AssignedTo;
        JobText = job.JobText;
    }
}
