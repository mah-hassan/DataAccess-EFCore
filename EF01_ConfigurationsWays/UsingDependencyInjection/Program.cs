using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UsingDependencyInjection;

internal class Program
{
    private static void Main(string[] args)
    {
        var config = new ConfigurationBuilder()
         .AddJsonFile("appSettings.json")
         .Build();
        var conStr = config.GetSection("connection_str").Value;

        var services = new ServiceCollection();
        services.AddDbContext<AppDbcontext>(options => options.UseSqlServer(conStr));

        //services.AddDbContextPool<AppDbcontext>(options => options.UseSqlServer(conStr));


        IServiceProvider serviceProvider = services.BuildServiceProvider();

        using (var context = serviceProvider.GetService<AppDbcontext>())
        {
            if (context != null)
            {
                foreach (var wallet in context.wallet)
                {
                    Console.WriteLine(wallet);
                }
            }
        }
    }
}