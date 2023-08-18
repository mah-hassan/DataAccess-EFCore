namespace WalletSystem
{
    public class Wallet
    {
        public int ID { get; set; }
        public string? Owner { get; set; }
        public decimal Balance { get; set; }


        public override string ToString()
        {
            return $"ID: [{ID}]\nOwner: {Owner}\nBalance: {Balance}\n\n";
        }
    }
}