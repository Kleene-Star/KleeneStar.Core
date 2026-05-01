using WebExpress.WebUI.WebControl;

namespace KleeneStar.Core.WebControl
{
    /// <summary>
    /// Represents a control panel that displays detailed information about a specific 
    /// object within the user interface.
    /// </summary>
    public class ObjectDetailControl : ControlPanel
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="id">The unique identifier for the control.</param>
        public ObjectDetailControl(string id = null)
            : base(id)
        {
        }
    }
}
