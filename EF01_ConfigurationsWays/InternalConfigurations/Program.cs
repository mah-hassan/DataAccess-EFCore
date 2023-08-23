using InternalConfigurations;

internal class Program
{
    private static void Main(string[] args)
    {
       
        using(var context = new AppDbcontext())
        {
            foreach (var wallet in context.wallet)
            {
                Console.WriteLine(wallet);
            }
        }
    }
}