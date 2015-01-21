using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Grep.Net.Entities;

namespace Grep.Net.Data.Contexts
{
    [DbConfigurationType(typeof(GrepNetDbConfiguration))] 
    public class FileTypeDefinitionContext : DbContext
    { 
        public DbSet<FileTypeDefinition> FileTypeDefinitions { get; set; }

        static FileTypeDefinitionContext()
        {
            //Database.SetInitializer<FileTypeDefinitionContext>(new Initializer());
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<FileTypeDefinitionContext, FileTypeDefinitionsMigrations.Configuration>()); 
        }

        public FileTypeDefinitionContext(String nameOrConnectionString) : base(nameOrConnectionString)
        {
            this.Configuration.ProxyCreationEnabled = false;
        }

        public FileTypeDefinitionContext() : base("Default_EntityDB")
        {
            this.Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<FileExtension>()
                  .HasRequired<FileTypeDefinition>(x => x.FileTypeDefinition)
           .WithMany(s => s.FileExtensions).HasForeignKey(s => s.FileTypeDefinitionId);
        }
    }
}