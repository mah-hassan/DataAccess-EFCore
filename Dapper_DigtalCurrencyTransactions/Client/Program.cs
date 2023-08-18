using WalletControlSystem;

internal class Program
{
    private static void Main(string[] args)
    {
        //var key = Console.ReadKey().Key;
        WalletControler.RemoveAll();
        WalletControler.AddFromJsonFile("wallets.json");
        Wallet sender = WalletControler.RandomWallet();
        Console.Write(sender + "sender => Before");

        Wallet receiver = WalletControler.RandomWallet();
        Console.Write(receiver + "receiver => Before");

        sender.Transfer(receiver, 30);

        sender = WalletControler.UpdatMe(sender);
        receiver = WalletControler.UpdatMe(receiver);

        Console.WriteLine("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        Console.Write(sender + "\nsender => After");
        Console.Write(receiver + "\nreceiver => After");

        //WalletControler.RetrieveAll();

    }
}