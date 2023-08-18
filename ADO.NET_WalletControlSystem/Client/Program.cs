using System.Text.Json;
using WalletSystem;

internal class Program
{
    private static void Main(string[] args)
    {
        WalletControler.RemoveAll();
        var LuffyWallet = new Wallet()
        {
            Owner = "Luffy",
            Balance = 100000,
        };
        LuffyWallet.AddWallet();
        WalletControler.AddFromJsonFile("Wallets.json");
        Wallet sender = WalletControler.RandomWallet();
        
        Console.WriteLine(sender);

        sender.Transfer(LuffyWallet, sender.Balance - 20);
        sender.LastUpdateForMe();
        Console.WriteLine(sender);
        LuffyWallet.LastUpdateForMe();
        Console.WriteLine(LuffyWallet) ;
        WalletControler.RetrieveAll();

    }



}