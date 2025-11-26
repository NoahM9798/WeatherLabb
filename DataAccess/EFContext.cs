using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Core.Models;
namespace DataAccess

{
    public class EFContext : DbContext
    {
        //Får connection string från config fil. 
        public string ConnectionString
        {
            get
            {
                var basePath = Path.GetDirectoryName(typeof(EFContext).Assembly.Location);

                var builder = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("Config.json", optional: false, reloadOnChange: true);

                var config = builder.Build();

                return config["DefaultConnection"];
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
        public DbSet<Weather> Weathers { get; set; }

        //Skapar databasen om den inte finns, add-migrations är att föredra men använder denna för uppgiftens skull
        public static void Initialize()
        {
            using var context = new EFContext();
            context.Database.EnsureCreated();
        }

    }
}
