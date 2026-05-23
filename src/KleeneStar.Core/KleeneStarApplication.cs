using KleeneStar.Core.WebTheme;
using KleeneStar.Model;
using KleeneStar.Model.Config;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.IO;
using System.Xml.Serialization;
using WebExpress.WebCore;
using WebExpress.WebCore.WebApplication;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebComponent;
using WebExpress.WebCore.WebIcon;

namespace KleeneStar.Core
{
    /// <summary>
    /// Represents a the KleeneStar application with a specific name, description,
    /// icon, and context path.
    /// </summary>
    [Name("kleenestar.core:app.name")]
    [Description("kleenestar.core:app.description")]
    [Icon("/assets/img/kleenestar.svg")]
    [IconTheme(TypeIconTheme.Light)]
    [Theme<LightTheme>]
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

            try
            {
                using var db = ModelHub.CreateDbContext();

                // apply a migration path if necessary
                MigrateWithLegacyDbReset(db, componentHub);

                // run seeding
                KleeneStarDbSeeder.SeedAsync(db).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                // surface the failure rather than letting WebExpress swallow it during
                // plugin instantiation (the plugin would then never appear in the sitemap).
                componentHub.LogManager.DefaultLog.Exception(ex);
                throw;
            }
        }

        /// <summary>
        /// Called when the application starts working. The call is concurrent.
        /// </summary>
        public void Run()
        {
        }

        /// <summary>
        /// Applies pending migrations. When the database exists but its schema was
        /// previously created without a <c>__EFMigrationsHistory</c> table (for example
        /// by an older <c>EnsureCreated()</c> code path or by a developer manually
        /// editing the migration files), the first migration throws "table already
        /// exists". In that case the database is reset and the migration is retried —
        /// the seeder will then repopulate every row.
        /// </summary>
        /// <param name="db">The database context.</param>
        /// <param name="componentHub">The component hub used to write a warning entry.</param>
        private static void MigrateWithLegacyDbReset(KleeneStarDbContext db, IComponentHub componentHub)
        {
            try
            {
                db.Database.Migrate();
            }
            catch (Exception ex) when (IsAlreadyExistsError(ex))
            {
                componentHub?.LogManager?.DefaultLog?.Warning
                (
                    "Legacy database schema without migrations history detected. " +
                    "Resetting the database and re-running migrations + seed."
                );

                db.Database.EnsureDeleted();
                db.Database.Migrate();
            }
        }

        /// <summary>
        /// Returns whether the supplied exception (or any of its inner exceptions)
        /// reports the "table already exists" condition that providers raise when
        /// the migration tries to (re-)create a table that is already present in
        /// the database.
        /// </summary>
        /// <param name="ex">The exception to inspect.</param>
        /// <returns><c>true</c> when the message chain contains "already exists".</returns>
        private static bool IsAlreadyExistsError(Exception ex)
        {
            for (var current = ex; current != null; current = current.InnerException)
            {
                if (current is DbException &&
                    current.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
