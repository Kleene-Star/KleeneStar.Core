using KleeneStar.Core.WebAttribute;
using KleeneStar.Core.WebManager;
using KleeneStar.Core.WebParameter;
using System;
using WebExpress.WebApp.WebPage;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WWW.Workspaces._key_.Classes._id_
{
    /// <summary>
    /// Provides functionality for managing the current class page.
    /// </summary>
    [WebIcon<IconGlobe>]
    [SegmentKey<KeyParameter>()]
    [Scope<IScopeGeneral>]
    [Cache]
    public sealed class Index : IPage<VisualTreeWebApp>, IScopeGeneral
    {
        private readonly IClassManager _classManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="classManager">
        /// The class manager used to retrieve class information. Cannot be null.
        /// </param>
        public Index(IClassManager classManager)
        {
            _classManager = classManager;
        }

        /// <summary>
        /// Processing of the resource.
        /// </summary>
        /// <param name="renderContext">The context for rendering the page.</param>
        /// <param name="visualTree">The visual tree of the web application.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            var classParameter = renderContext.Request.GetParameter<ClassParameter>();
            var guid = Guid.TryParse(classParameter.Value, out var id) ? id : Guid.Empty;
            var workspace = _classManager.GetClass(guid);

            visualTree.Title = workspace?.Name;
            visualTree.Content.MainPanel.Headline.Title = workspace?.Name;
        }
    }
}
