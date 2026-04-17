using KleeneStar.Model;
using KleeneStar.Model.Config;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Xml.Serialization;
using WebExpress.WebCore;
using WebExpress.WebCore.WebApplication;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebComponent;

namespace KleeneStar.Core
{
    /// <summary>
    /// Represents a the KleeneStar application with a specific name, description, 
    /// icon, and context path.
    /// </summary>
    [Name("kleenestar.core:app.name")]
    [Description("kleenestar.core:app.description")]
    [Icon("/assets/img/kleenestar.svg")]
    [ContextPath("/kleenestar")]
    public sealed class KleeneStarApplication : IApplication
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="applicationContext">The application context.</param>
        /// <param name="componentHub">The component hub.</param>
        /// <param name="httpServerContext">The reference to the context of the host.</param>
        public KleeneStarApplication(IApplicationContext applicationContext, IComponentHub componentHub, IHttpServerContext httpServerContext)
        {
            CoreHub.HttpServerContext = httpServerContext;
            ModelHub.HttpServerContext = httpServerContext;
            CoreHub.ComponentHub = componentHub;
            ModelHub.ComponentHub = componentHub;
            CoreHub.ApplicationContet = applicationContext;
            ModelHub.ApplicationContext = applicationContext;

            CoreHub.ComponentHub.IdentityManager.RegisterIdentityProvider(new WebIdentity.IdentityProvider(), applicationContext);

            // load configuration
            try
            {
                var configFile = Path.Combine(httpServerContext.ConfigPath, "kleenestar.db.config.xml");
                using var reader = new FileStream(configFile, FileMode.Open);
                var serializer = new XmlSerializer(typeof(DbConfig));
                var config = serializer.Deserialize(reader) as DbConfig;
                ModelHub.DatabaseConfig = config;
            }
            catch
            {
                // default
                ModelHub.DatabaseConfig = new DbConfig()
                {
                    Provider = "SQLite",
                    Assembly = "KleeneStar.Model.Sqlite",
                    ConnectionString = "Data Source=data/db/kleenestar.db"
                };
            }

            using var db = ModelHub.CreateDbContext();

            // apply a migration path if necessary
            db.Database.Migrate();

            // run seeding
            _ = KleeneStarDbSeeder.SeedAsync(db);
        }

        /// <summary>
        /// Called when the application starts working. The call is concurrent. 
        /// </summary>
        public void Run()
        {
        }
    }
}
