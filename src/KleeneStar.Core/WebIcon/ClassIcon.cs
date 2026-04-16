using WebExpress.WebCore.WebApplication;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Core.WebIcon
{
    /// <summary>
    /// Represents an icon for the class.
    /// </summary>
    public class ClassIcon : ImageIcon
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ClassIcon()
            : base(new UriEndpoint("/kleenestar/assets/img/class.svg"), null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="applicationContext">The application context to be associated with the icon.</param>
        public ClassIcon(IApplicationContext applicationContext = null)
            : base(new UriEndpoint("/kleenestar/assets/img/class.svg"), null, applicationContext)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="size">The size of the icon.</param>
        /// <param name="applicationContext">The application context to be associated with the icon.</param>
        public ClassIcon(PropertySizeIcon size, IApplicationContext applicationContext = null)
            : base(new UriEndpoint("/kleenestar/assets/img/class.svg"), size, applicationContext)
        {
        }
    }
}
