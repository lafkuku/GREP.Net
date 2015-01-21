using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Grep.Net.Entities;

namespace Grep.Net.Data.Contexts
{
    [DbConfigurationType(typeof(GrepNetDbConfiguration))] 
    public class PatternPackageContext : DbContext
    {
        static PatternPackageContext()
        {
            //Database.SetInitializer<PatternPackageContext>(new Initializer());
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<PatternPackageContext, PatternPackagesMigrations.Configuration>()); 
        }

        public DbSet<PatternPackage> PatternPackages { get; set; }

        public PatternPackageContext(String nameOrConnectionString) : base(nameOrConnectionString)
        {
            Init();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
        }

        public PatternPackageContext() : base("Default_EntityDB")
        {
            Init();
        }

        private void Init()
        {
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
        }
    }
}