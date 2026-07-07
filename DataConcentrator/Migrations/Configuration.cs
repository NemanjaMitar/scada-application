namespace DataConcentrator.Migrations
{
    using System.Data.Entity.Migrations;
    using DataConcentrator.Model;

    public sealed class Configuration : DbMigrationsConfiguration<DataConcentrator.ContextClass>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "DataConcentrator.ContextClass";
        }

        protected override void Seed(DataConcentrator.ContextClass context)
        {
            context.Users.AddOrUpdate(
                u => u.Username,
                new User
                {
                    Username = "admin",
                    Password = "P@ssw0rd12345!",
                    UserRole = Role.Admin
                });
        }
    }
}
