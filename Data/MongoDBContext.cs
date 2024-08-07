using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using Onyx.Models.Domain;

namespace Onyx.Data
{
    public class MongoDBContext: DbContext
    {
        public MongoDBContext(DbContextOptions options): base(options)
        {
        }

        public DbSet<ProcessDataModel> ProcessData { get; init; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ProcessDataModel>().ToCollection("process_data");
        }
    }
}
