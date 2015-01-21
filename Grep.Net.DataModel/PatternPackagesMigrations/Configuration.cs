namespace Grep.Net.Data.PatternPackagesMigrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Grep.Net.Entities;

    internal sealed class Configuration : DbMigrationsConfiguration<Grep.Net.Data.Contexts.PatternPackageContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            MigrationsDirectory = @"PatternPackagesMigrations";
        }

        protected override void Seed(Grep.Net.Data.Contexts.PatternPackageContext context)
        {
            //  This method will be called after migrating to the latest version.
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            base.Seed(context);
  
            //Populate defaults.
            List<PatternPackage> ppDefs = DefaultDataPopulatorHelper.GetAllEntitiesFromDirectory<PatternPackage>();

            ppDefs.ForEach(x =>
            {
            

                context.PatternPackages.AddOrUpdate(x);
                context.SaveChanges();
               // context.PatternPackages.AddOrUpdate<PatternPackage>(x);
            });

          
        }
    }
}