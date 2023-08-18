using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace WalletSystem
{
    public static class WalletControler
    {
        private static IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("configuration.json")
            .Build();
        
        private static string? connectionStr = configuration.GetSection("connection_str").Value;
        private static SqlConnection connection = new SqlConnection(connectionStr);

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
            SqlParameter holder = new SqlParameter()
            {
                ParameterName = "@Holder",
                SqlDbType = SqlDbType.VarChar,
                Direction = ParameterDirection.Input,
                Value = wallet.Owner
            };
            SqlParameter balance = new SqlParameter()
            {
                ParameterName = "@Balance",
                SqlDbType = SqlDbType.Decimal,
                Direction = ParameterDirection.Input,
                Value = wallet.Balance
            };
            string sql = "INSERT INTO Wallet (Holder,Balance) VALUES (@Holder,@Balance)"; // CommandType.Text
            //string sqlProcedure = "AddWallet"; // CommandType.StoredProcedure;
            SqlCommand comd = new SqlCommand(sql, connection);
            comd.CommandType = CommandType.Text;
            comd.Parameters.Add(holder);
            comd.Parameters.Add(balance);
            connection.Open();
       
            if (comd.ExecuteNonQuery() > 0)
            {
                Console.WriteLine($"{wallet.Owner}`s Wallet Added Succesfully..");
            }
            connection.Close();
            

        }

        public static void RemoveAll()
        {
            string sql = "DELETE FROM Wallet;"+
                "\nDBCC CHECKIDENT('Wallet', RESEED, 0);";

            SqlCommand comd = new SqlCommand(sql, connection);
            connection.Open();
            if (comd.ExecuteNonQuery() > 0)
            {
                Console.WriteLine($"All Wallets Deleted Succesfully..");
                //string resetSql = "DBCC CHECKIDENT('Wallet', RESEED, 0)";
                //SqlCommand resetCmd = new SqlCommand(resetSql, connection);
                //resetCmd.ExecuteNonQuery();
            }
            connection.Close();
        }

        public static void RetrieveAll()
        {
            //string sql = "SELECT * FROM Wallet;"; // CommandType.Text 
            string sqlProcedure = "GetAllWallets"; //  CommandType.StoredProcedure
            List<Wallet> wallets = new List<Wallet>();
            SqlCommand cmd = new SqlCommand(sqlProcedure, connection);

            cmd.CommandType = CommandType.StoredProcedure;

            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                wallets.Add(new Wallet
                {
                    ID = reader.GetInt32("ID"),
                    Owner = reader.GetString("Holder"),
                    Balance = reader.GetDecimal("Balance")
                });
            }

            connection.Close();
            foreach (var wallet in wallets)
            {
                Console.WriteLine(wallet);
            }
        }

        private static int GetId(this Wallet wallet)
        {
            if (wallet.ID != 0)
                return wallet.ID;

                SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT ID FROM Wallet\n WHERE Holder = @owner AND Balance = @balance";
            SqlParameter owner = new SqlParameter()
            {
                ParameterName = "@owner",
                Value = wallet.Owner,
                SqlDbType = SqlDbType.VarChar,
                Direction = ParameterDirection.Input,
            };
            SqlParameter balance = new SqlParameter()
            {
                ParameterName = "@balance",
                Value = wallet.Balance,
                SqlDbType = SqlDbType.Decimal,
                Direction = ParameterDirection.Input,
            };
            cmd.Parameters.Add(owner);
            cmd.Parameters.Add(balance);

            connection.Open();
            
            wallet.ID =(int)cmd.ExecuteScalar();
            

            connection.Close();
            return wallet.ID;
            
        }
        public static void Deposite(this Wallet wallet , decimal _value)
        {
            string sql = "UPDATE Wallet SET Balance = Balance + @balance \nWHERE ID = @id";

            SqlParameter value = new SqlParameter()
            {
                ParameterName = "@balance",
                Value = _value,
                SqlDbType = SqlDbType.Decimal,
                Direction = ParameterDirection.Input,
            };

            SqlParameter id = new SqlParameter()
            {
                ParameterName = "@id",
                Value = wallet.GetId(),
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input,
            };
            SqlCommand comd = new SqlCommand(sql, connection);
            comd.Parameters.Add(value);
            comd.Parameters.Add(id);
            comd.CommandType = CommandType.Text;


            connection.Open();
            
            if (comd.ExecuteNonQuery() > 0)
            {
                Console.WriteLine("updated succesfully");
            }
            connection.Close();
        }
        public static void Withdraw(this Wallet wallet , decimal _value)
        {
            wallet.Deposite(-_value);
        }

        public static Wallet RandomWallet()
        {
            Random random = new Random();
            Wallet? randomWallet = null;
            string sql = "SELECT COUNT(*) FROM Wallet";
            int counter = 0;

            using(SqlCommand cmd = new SqlCommand(sql, connection))
            {
                cmd.CommandType = CommandType.Text;
                connection.Open();
                counter = Convert.ToInt32(cmd.ExecuteScalar());          
                connection.Close();
            }

            if (counter >= 1)
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT * FROM Wallet \n WHERE ID = @id";

                    SqlParameter id = new SqlParameter
                    {
                        ParameterName = "@id",
                        Value = random.Next(1, counter),
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Input,

                    };

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.Add(id);
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            randomWallet = new Wallet
                            {
                                ID = reader.GetInt32("ID"),
                                Owner = reader.GetString("Holder"),
                                Balance = reader.GetDecimal("Balance")
                            };
                        }
                        connection.Close();
                    }
                }
            }
           
            return randomWallet!;

        }
        public static void Transfer(this Wallet sender ,Wallet receiver, decimal ammount)
        {

            try
            {
                if (ammount > 0)
                {
                    sender.Withdraw(ammount);
                    receiver.Deposite(ammount);
                }
            }
            catch
            {
                Console.WriteLine("Operation Failled..");
            }
           
        }
        public static void LastUpdateForMe(this Wallet wallet)
        {
            var sql = "SELECT * FROM Wallet \n WHERE ID = @ID";

            using(SqlCommand cmd = new SqlCommand(sql, connection))
            {
                SqlParameter id = new SqlParameter
                {
                    ParameterName = "@ID",
                    Value = wallet.ID,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                };
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(id);
                
                connection.Open();
                using(SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        wallet.ID = reader.GetInt32("ID");
                        wallet.Owner = reader.GetString("Holder");
                        wallet.Balance = reader.GetDecimal("Balance");                        
                    }
                }
                connection.Close();
          
            }
        }
    }


}
