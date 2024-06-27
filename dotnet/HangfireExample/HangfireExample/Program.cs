using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using HangfireExample;
using MongoDB.Driver;

var databaseName = "jobs";
var connectionString = "mongodb://root:example@localhost:27017/";

//var mongoUrlBuilder = new MongoUrlBuilder(connectionString + databaseName);
//mongoUrlBuilder.ToMongoUrl() removes the port
var mongoClient = new MongoClient(connectionString);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseMongoStorage(mongoClient, databaseName, new MongoStorageOptions
        {
            MigrationOptions = new MongoMigrationOptions
            {
                MigrationStrategy = new MigrateMongoMigrationStrategy(),
                BackupStrategy = new CollectionMongoBackupStrategy()
            },
            Prefix = "hangfire.mongo",
            CheckConnection = true,
            CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection // local single-node
        })
    );

builder.Services.AddHangfireServer(optionsAction =>
{
    optionsAction.ServerName = "Hangfire.Mongo";
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseHangfireDashboard();

//Run once
//BackgroundJob.Enqueue<Job>(j => j.Run());

//Recurring every 5 minutes
RecurringJob.AddOrUpdate<Job>("jobId", j => j.Run(), "*/5 * * * *");

app.Run();
