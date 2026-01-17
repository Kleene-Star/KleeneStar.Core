using KleeneStar.Model;
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
        public KleeneStarApplication(IApplicationContext applicationContext, IComponentHub componentHub)
        {
            CoreHub.ComponentHub = componentHub;
            ModelHub.ComponentHub = componentHub;
            CoreHub.ApplicationContet = applicationContext;
            ModelHub.ApplicationContet = applicationContext;
        }

        /// <summary>
        /// Called when the application starts working. The call is concurrent. 
        /// </summary>
        public void Run()
        {
        }
    }
}
