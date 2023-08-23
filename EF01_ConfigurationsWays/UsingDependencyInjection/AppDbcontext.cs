
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace UsingDependencyInjection
{
    public class AppDbcontext : DbContext
    {
        public DbSet<Wallet> wallet { get; set; }
        public AppDbcontext(DbContextOptions options)
             : base(options)
        {

        }
    }
}
