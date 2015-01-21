namespace Grep.Net.Data.FileTypeDefinitionsMigrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Grep.Net.Entities;

    internal sealed class Configuration : DbMigrationsConfiguration<Grep.Net.Data.Contexts.FileTypeDefinitionContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            MigrationsDirectory = @"FileTypeDefintionsMigrations";
            ContextKey = "Grep.Net.Data.Contexts.FileTypeDefinitionContext";
        }

        protected override void Seed(Grep.Net.Data.Contexts.FileTypeDefinitionContext context)
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
            //Populate defaults.
            List<FileTypeDefinition> fileTypeDefs = DefaultDataPopulatorHelper.GetAllEntitiesFromDirectory<FileTypeDefinition>();

            fileTypeDefs.ForEach(x =>
            {

                context.FileTypeDefinitions.AddOrUpdate(x);
                context.SaveChanges();
                /*
                foreach (FileExtension fe in x.FileExtensions)
                {
                    context.Entry(fe).State = System.Data.Entity.EntityState.Unchanged;
                }*/


            });
        }
    }
}