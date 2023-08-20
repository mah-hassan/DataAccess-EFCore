using System.Runtime.CompilerServices;

namespace WalletControlSystem
{
    public class Wallet
    {
        public virtual int ID { get; set; }
        public virtual string? Holder { get; set; }
        public virtual decimal Balance { get; set; }
               

        public override string ToString()
        {
            return $"\nID: [{ID}]\nOwner: {Holder}\nBalance: {Balance}\n\n";
        }


        public virtual void UpdatMe()
        {
            this.Balance = WalletControler.retrieveSingleWallet(this.ID).Balance;
        }
    }
}