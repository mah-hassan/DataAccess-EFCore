using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace WalletControlSystem
{
    public class WalletMapping : ClassMapping<Wallet>
    {
        public WalletMapping()
        {
            Id(x => x.ID, c =>
            {
                c.Generator(Generators.Identity);
                c.Type(NHibernateUtil.Int32);
                c.Column("ID");
            });
            Property(x => x.Holder, c =>
            {
                c.Type(NHibernateUtil.AnsiString); // AnsiString with varchar
                //c.Column("Holder");
                c.NotNullable(true);

            });
            Property(x => x.Balance, c =>
            {
                c.Type(NHibernateUtil.Decimal);
                c.NotNullable(true);
            });

            Table("Wallet");
        }
    }
}
