using InternalConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

internal class Program
{
    private static void Main(string[] args)
    {

        var config = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json")
            .Build();
        var conStr = config.GetSection("connection_str").Value;

        var optionsBuilder = new DbContextOptionsBuilder();
        optionsBuilder.UseSqlServer(conStr);

        var options = optionsBuilder.Options;

        using (var context = new AppDbcontext(options))
        {
            foreach (var wallet in context.wallet)
            {
                Console.WriteLine(wallet);
            }
        }
    }
}