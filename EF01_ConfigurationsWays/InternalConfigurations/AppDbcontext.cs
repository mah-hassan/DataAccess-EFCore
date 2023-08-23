
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace InternalConfigurations
{
    public class AppDbcontext : DbContext
    {
        public DbSet<Wallet> wallet { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json")
                .Build();

            var conStr = config.GetSection("connection_str").Value;
            //Console.WriteLine(conStr);  
            optionsBuilder.UseSqlServer(conStr);    
        }
    }
}
