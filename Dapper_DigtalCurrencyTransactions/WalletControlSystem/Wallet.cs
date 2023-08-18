namespace WalletControlSystem
{
    public class Wallet
    {
        public int ID { get; set; }
        public string? Holder { get; set; }
        public decimal Balance { get; set; }


        public override string ToString()
        {
            return $"\nID: [{ID}]\nOwner: {Holder}\nBalance: {Balance}\n\n";
        }
    }
}