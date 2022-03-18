using MongoDataAccess.Models;
using MongoDB.Driver;

namespace MongoDataAccess.DataAccess;
public class JobDataAccess
{   
    // DO NOT STORE YOUR CONNECTION STRING HERE IN PRODUCTION OR OTHERWISE RELEASING TO A PUBLIC !!!
    private const string ConnectionString = "<YOUR_CONNECTION_STRING_FROM_MONGODB>";
    private const string DatabaseName = "job_db";
    private const string JobCollection = "jobs";
    private const string UserCollection = "users";
    private const string JobHistoryCollection = "history";

    private IMongoCollection<T> ConnectToMongo<T>(in string collection)
    {
        var client = new MongoClient(ConnectionString);
        var db = client.GetDatabase(DatabaseName);
        return db.GetCollection<T>(collection);
    }

    public async Task<List<UserModel>> GetAllUsers()
    {
        var usersCollection = ConnectToMongo<UserModel>(UserCollection);
        var results = await usersCollection.FindAsync(_ => true);
        return results.ToList();
    }

    public async Task<List<JobModel>> GetAllJobs()
    {
        var jobsCollection = ConnectToMongo<JobModel>(JobCollection);
        var results = await jobsCollection.FindAsync(_ => true);
        return results.ToList();
    }

    public async Task<List<JobModel>> GetAllJobsForAUser(UserModel user)
    {
        var jobsCollection = ConnectToMongo<JobModel>(JobCollection);
        var results = await jobsCollection.FindAsync(j => j.AssignedTo.Id == user.Id);
        return results.ToList();
    }

    public Task CreateUser(UserModel user)
    {
        var usersCollection = ConnectToMongo<UserModel>(UserCollection);
        return usersCollection.InsertOneAsync(user);
    }

    public Task CreateJob(JobModel job)
    {
        var jobsCollection = ConnectToMongo<JobModel>(JobCollection);
        return jobsCollection.InsertOneAsync(job);
    }

    public Task UpdateJob(JobModel job)
    {
        var jobsCollection = ConnectToMongo<JobModel>(JobCollection);
        var filter = Builders<JobModel>.Filter.Eq("Id", job.Id);
        return jobsCollection.ReplaceOneAsync(filter, job, new ReplaceOptions { IsUpsert = true });
    }

    public Task DeleteJob(JobModel job)
    {
        var jobsCollestion = ConnectToMongo<JobModel>(JobCollection);
        return jobsCollestion.DeleteOneAsync(c => c.Id == job.Id);
    }

    public async Task CompleteJob(JobModel job)
    {
        // Do like this with a caution !!!
        //var jobsCollection = ConnectToMongo<JobModel>(JobCollection);
        //var filter = Builders<JobModel>.Filter.Eq("Id", job.Id);
        //await jobsCollection.ReplaceOneAsync(filter, job);

        //var jobHistoryCollection = ConnectToMongo<JobHistoryModel>(JobHistoryCollection);
        //await jobHistoryCollection.InsertOneAsync(new JobHistoryModel(job));


        // Instead do like this, with a transaction: 
        var client = new MongoClient(ConnectionString);
        using var session = await client.StartSessionAsync();

        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(DatabaseName);
            var jobsCollection = db.GetCollection<JobModel>(JobCollection);
            var filter = Builders<JobModel>.Filter.Eq("Id", job.Id);
            await jobsCollection.ReplaceOneAsync(filter, job);

            var jobHistoryCollection = db.GetCollection<JobHistoryModel>(JobHistoryCollection);
            await jobHistoryCollection.InsertOneAsync(new JobHistoryModel(job));

            await session.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync();
            Console.WriteLine(ex.Message);
        }
    }
}