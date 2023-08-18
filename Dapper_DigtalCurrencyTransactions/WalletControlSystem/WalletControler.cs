using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Transactions;

namespace WalletControlSystem
{
    public static class WalletControler
    {
        private static IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("configuration.json")
            .Build();
        
        private static string? connectionStr = configuration.GetSection("connection_str").Value;
        static IDbConnection db = new SqlConnection(connectionStr);
        public static void AddFromJsonFile(string path)
        {
            string jsonContent = File.ReadAllText(@path);
            List<Wallet>? wallets = new List<Wallet>();
            if (!jsonContent.IsNullOrEmpty())
            {
                wallets = JsonSerializer.Deserialize<List<Wallet>>(jsonContent,
                    new JsonSerializerOptions {PropertyNameCaseInsensitive = true}); 
            }
            if (wallets is not null)
            {
                foreach (var wallet in wallets)
                {
                    AddWallet(wallet);
                }
            }
        }

        public static void AddWallet(this Wallet wallet)
        {
            string sql = "INSERT INTO Wallet (Holder,Balance) VALUES (@Holder,@Balance);";
            var parameters = new DynamicParameters();
            parameters.Add("@Holder", wallet.Holder);
            parameters.Add("@Balance", wallet.Balance);
      
            db.Execute(sql,parameters);                           
        }

        public static void RemoveAll()
        {
            string sql = "DELETE FROM Wallet;"+
                "\nDBCC CHECKIDENT('Wallet', RESEED, 0);";
            db.Execute(sql);      
        }

        public static void RetrieveAll()
        {
            string sql = "SELECT * FROM Wallet;"; // CommandType.Text 
            //string sqlProcedure = "GetAllWallets"; //  CommandType.StoredProcedure
            List<Wallet> wallets = new List<Wallet>();

            wallets = db.Query<Wallet>(sql).AsList<Wallet>();
        
            foreach (var wallet in wallets)
            {
                Console.WriteLine(wallet);
            }
        }

        private static int GetId(this Wallet wallet)
        {
            if (wallet.ID != 0)
                return wallet.ID;

            string sql = "SELECT ID FROM Wallet\n WHERE Holder = @holder AND Balance = @balance";

            var parameters = new
            {
                holder = wallet.Holder,
                balance = wallet.Balance,
            };
            wallet.ID = db.ExecuteScalar<int>(sql,parameters);
               
            return wallet.ID;
            
        }
        public static void Deposite(this Wallet wallet, decimal _value)
        {
            if (_value > 0)
            {
                wallet.Deposite_(_value);
            }
            else
                throw new InvalidOperationException();
                
        }

        private static void Deposite_(this Wallet wallet , decimal _value)
        {
            string sql = "UPDATE Wallet SET Balance = Balance + @balance \nWHERE ID = @id";
            var Parameters = new
            {
                balance = _value,
                id = wallet.GetId() ,
            };

            db.Execute(sql,Parameters);
        }
        public static void Withdraw(this Wallet wallet , decimal _value)
        {
            if (_value > 0)
            {
                wallet.Deposite_(-_value);                
            }
            else
                throw new InvalidOperationException();
        }

        public static Wallet RandomWallet()
        {
            Random random = new Random();
           
            string sql = "SELECT COUNT(*) FROM Wallet";
            int counter = 0;

            counter = db.ExecuteScalar<int>(sql);

             
            return retrieveSingleWallet(random.Next(1, counter));
        }
        public static void Transfer(this Wallet sender ,Wallet receiver, decimal ammount)
        {
            using(var transaction = new TransactionScope())
            {
                sender.Withdraw(ammount);
                receiver.Deposite(ammount);

                transaction.Complete();
      
            }           
        }
        public static Wallet retrieveSingleWallet(int id)
        {
            var sql = "SELECT * FROM Wallet \n WHERE ID = @id";
            var parameter = new DynamicParameters();
            parameter.Add("@id", id);

            var result = db.Query<Wallet>(sql, parameter).First<Wallet>();
            return result;
        }

        public static Wallet UpdatMe(this Wallet wallet)
        {
           return retrieveSingleWallet(wallet.ID);
        }
    }
}
