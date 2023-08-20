
using WalletControlSystem;

internal class Program
{
    private static void Main(string[] args)
    {
        // Wallet walletToInsert = new Wallet()
        // {

        //     Holder = "mahmoud",
        //     Balance = 7750,
        // };
        // walletToInsert.AddWallet();
 
        Wallet s = new Wallet()
        {
            ID = 1,
        };

        Wallet r = new Wallet()
        {
            ID = 2,
        };

        s.Transfer(r, 300);



        //WalletControler.RemoveAll();
        //WalletControler.AddFromJsonFile("wallets.json");

        WalletControler.RetrieveAll();
    }
}