using System.Data.Entity;
using System.Data.Entity.Migrations;
using DataConcentrator.Model;
using DataConcentrator.Migrations;

namespace DataConcentrator
{
    public class ContextClass : DbContext
    {
        private static ContextClass instance;

        static ContextClass()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ContextClass, Configuration>());
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

        public ContextClass() : base("name=ContextClass") { }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<Alarm> Alarms { get; set; }
        public DbSet<ActivatedAlarm> ActivatedAlarms { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Alarm>()
                .HasRequired(a => a.TagNavigation)
                .WithMany()
                .HasForeignKey(a => a.TagName)
                .WillCascadeOnDelete(false);
        }
    }
}