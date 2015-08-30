using storeme.Data.Model;
using System.Data.Entity;
using MongoDB.Driver;

namespace storeme.Data
{
    public class DashboardContext : DbContext
    {
        public DashboardContext() : base("DefaultConnection")
        {
        }

        public DbSet<Dashboard> Dashboards { get; set; }    

        public DbSet<DashboardItem> DashboardItems { get; set; }

        public DbSet<DashboardFile> Files { get; set; }
    }

    public class MongoDashboardContext
    {
        public MongoDashboardContext()
        {
            var settings = new MongoClientSettings();
            settings.Server = new MongoServerAddress("127.0.0.1");
            var client = new MongoClient(settings);
            var database = client.GetDatabase("Workspaces");
            Dashboards = database.GetCollection<Dashboard>("Dashboard");
        }

        public IMongoCollection<Dashboard> Dashboards { get; set; }
    }
}