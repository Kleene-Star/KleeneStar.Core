using WebExpress.WebApp.WebControl;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Core.WebControl
{
    public class ListDetailControl : ControlPanelSplit
    {
        /// <summary>
        /// Gets the configuration tile that provides REST access to 
        /// workspace data.
        /// </summary>
        public ControlRestList List { get; } = new ControlRestList()
        {
        };

        /// <summary>
        /// Gets the configuration tile that provides REST access to 
        /// workspace data.
        /// </summary>
        public ControlFrame Frame { get; } = new ControlFrame()
        {
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="id">The unique identifier for the control.</param>
        public ListDetailControl(string id = null)
            : base(id)
        {
            AddSidePanel(List);
            AddMainPanel(Frame);

            SidePanelInitialSize = 250;
        }
    }
}
