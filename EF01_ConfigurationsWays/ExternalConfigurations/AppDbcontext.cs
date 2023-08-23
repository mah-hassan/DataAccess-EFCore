
using Microsoft.EntityFrameworkCore;
namespace InternalConfigurations
{
    public class AppDbcontext : DbContext
    {
        public DbSet<Wallet> wallet { get; set; }

        public AppDbcontext(DbContextOptions options)
            :base(options)
        {
            
        }

    }
}
