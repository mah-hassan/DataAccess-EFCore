using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using Remotion.Linq.Clauses.ResultOperators;
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
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
            using(var session = CreateSession()) 
            {
                using(var transaction = session.BeginTransaction())
                {
                    session.Save(wallet);
                    transaction.Commit();
                }
            }     
        }

        public static void RemoveAll()
        {
            using (var session = CreateSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    string sql = "DELETE FROM Wallet;\nDBCC CHECKIDENT('Wallet', RESEED, 0);";
                    var quary = session.CreateSQLQuery(sql);
                    quary.ExecuteUpdate();
                    transaction.Commit();
                }
            }
        }

        public static void RetrieveAll()
        {
         
            
            using (var session = CreateSession())                
            {
                using(var transaction = session.BeginTransaction())
                {
                    var wallets = session.Query<Wallet>().ToList();
                    foreach (var wallet in wallets)
                    {
                        Console.WriteLine(wallet);
                    }
                }
            }
        }

        //private static int GetId(this Wallet wallet)
        //{
        //    if (wallet.ID != 0)
        //        return wallet.ID;

        //    string sql = "SELECT ID FROM Wallet\n WHERE Holder = @holder AND Balance = @balance";

        //    var parameters = new
        //    {
        //        holder = wallet.Holder,
        //        balance = wallet.Balance,
        //    };
        //    wallet.ID = db.ExecuteScalar<int>(sql,parameters);

        //    return wallet.ID;

        //}
        public static void Deposite(this Wallet wallet, decimal _value)
        {
            if (_value > 0)
            {
                wallet.Deposite_(_value);
            }
            else
                throw new InvalidOperationException();

        }

        private static void Deposite_(this Wallet wallet, decimal _value)
        {
            using(var session = CreateSession())
            {
                wallet = session.Get<Wallet>(wallet.ID);
                wallet.Balance += _value;
                session.Update(wallet);
            }
        }
        public static void Withdraw(this Wallet wallet, decimal _value)
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
            int counter = 0;
            using (var session = CreateSession())
            {
                counter = session.Query<Wallet>().Count();
            }
           
            return retrieveSingleWallet(random.Next(1, counter));
        }
        public static void Transfer(this Wallet sender, Wallet receiver, decimal amount)
        {
            using (var session = CreateSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    sender = session.Get<Wallet>(sender.ID);
                    receiver = session.Get<Wallet>(receiver.ID);

                    sender.Balance -= amount;
                    receiver.Balance += amount;

                    if (sender.Balance < 0)
                    {
                        throw new InvalidOperationException("their is no enough mony");
                    }

                    session.SaveOrUpdate(sender);
                    session.SaveOrUpdate(receiver);

                    transaction.Commit();
                }
            }
        }
        public static Wallet retrieveSingleWallet(int id)
        {
            var result = new Wallet();
            using (var session = CreateSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                     result = session.Query<Wallet>().FirstOrDefault<Wallet>(x => x.ID == id);
                }
            }         
            return result!;
        }






        private static ISession CreateSession()
        {
            var mapper = new ModelMapper();
            mapper.AddMappings(typeof(Wallet).Assembly.ExportedTypes);

            HbmMapping domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
            //Console.WriteLine(domainMapping.AsString());    
            var hibernateConfig = new Configuration();

            hibernateConfig.DataBaseIntegration(c =>
            {
                c.Driver<MicrosoftDataSqlClientDriver>();
                c.Dialect<MsSql2012Dialect>();
                c.ConnectionString = connectionStr;

                c.LogSqlInConsole = true;
                c.LogFormattedSql = true;

            });

            hibernateConfig.AddMapping(domainMapping);

            var sessionFac = hibernateConfig.BuildSessionFactory();

            var session = sessionFac.OpenSession();

            return session;

        }



         

    }
}
