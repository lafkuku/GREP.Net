using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Grep.Net.Entities;

namespace Grep.Net.Data.Contexts
{
    [DbConfigurationType(typeof(GrepNetDbConfiguration))] 
    public class TemplateContext : DbContext
    {
        static TemplateContext()
        {
            Database.SetInitializer<TemplateContext>(new Initializer());
        }

        public DbSet<Template> Templates { get; set; }
        
        public TemplateContext(String nameOrConnectionString) : base(nameOrConnectionString)
        {
            this.Configuration.ProxyCreationEnabled = false;
        }

        public TemplateContext() : base()
        {
            this.Configuration.ProxyCreationEnabled = false;
        }

        internal class Initializer : DropCreateDatabaseIfModelChanges<TemplateContext>
        {
            protected override void Seed(TemplateContext context)
            {
                base.Seed(context);

                bool debug = false;
                #if DEBUG
                debug = true;
                #endif

                if (!context.Database.Exists() ||
                    !context.Database.CompatibleWithModel(false) ||
                    debug)
                {
                    context.Database.Delete();
                    context.Database.Create();

                    //Populate defaults.
                    List<Template> templates = DefaultDataPopulatorHelper.GetAllEntitiesFromDirectory<Template>();

                    templates.ForEach(x =>
                    {
                        if (x.Id == Guid.Empty)
                        {
                            x.Id = Guid.NewGuid();
                        }
                    });

                    context.Templates.AddRange(templates);

                    context.SaveChanges();
                }
            }
        }
    }
}