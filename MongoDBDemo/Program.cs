using MongoDataAccess.DataAccess;
using MongoDataAccess.Models;
using MongoDB.Driver;
using MongoDBDemo;


// Create new instance of JobDataAccess
JobDataAccess db = new JobDataAccess();

// Create a new user
await db.CreateUser(new UserModel() { FirstName = "Tim", LastName = "Corey" });

// Get all users
var users = await db.GetAllUsers();

// Declare a new job
var job = new JobModel() 
{ 
    AssignedTo = users.First(), 
    JobText ="Mow the lawn", 
    FrequencyInDays = 7 
};

// Create a new job
await db.CreateJob(job);

// Get all jobs
var jobs = await db.GetAllJobs();

// Update first job to Completed
var newJob = jobs.First();
newJob.LastCompleted = DateTime.UtcNow;

await db.CompleteJob(newJob);