using System.Data.Entity;
using DataConcentrator.Model; // Dodati referencu na modele

namespace DataConcentrator
{
    public class ContextClass : DbContext
    {
        private static ContextClass instance;

        static ContextClass()
        {
            // Baza se gradi iz modela (sve tabele) i seed-uje admin nalog.
            // Rekreira se automatski ako se model promeni.
            Database.SetInitializer(new ScadaDbInitializer());
        }

        public static ContextClass Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ContextClass();
                }
                return instance;
            }
        }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<Alarm> Alarms { get; set; }
        public DbSet<ActivatedAlarm> ActivatedAlarms { get; set; }

        // DODANO: Tabela za korisnike
        public DbSet<User> Users { get; set; }
    }
}