using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;
using storeme.Data.Model;

namespace storeme.Data
{
    public class DashboardContext
    {
        public DashboardContext()
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