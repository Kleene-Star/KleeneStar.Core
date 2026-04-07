using WebExpress.WebCore.WebApplication;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WebIcon
{
    /// <summary>
    /// Represents an icon for the workspace.
    /// </summary>
    public class KleeneStarIcon : ImageIcon
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public KleeneStarIcon()
            : base(new UriEndpoint("/kleenestar/assets/img/kleenestar.svg"), null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="applicationContext">The application context to be associated with the icon.</param>
        public KleeneStarIcon(IApplicationContext applicationContext = null)
            : base(new UriEndpoint("/kleenestar/assets/img/kleenestar.svg"), null, applicationContext)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="size">The size of the icon.</param>
        /// <param name="applicationContext">The application context to be associated with the icon.</param>
        public KleeneStarIcon(PropertySizeIcon size, IApplicationContext applicationContext = null)
            : base(new UriEndpoint("/kleenestar/assets/img/kleenestar.svg"), size, applicationContext)
        {
        }
    }
}
