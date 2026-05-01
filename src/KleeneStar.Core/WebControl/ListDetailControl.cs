using WebExpress.WebApp.WebControl;
using WebExpress.WebUI.WebControl;

namespace KleeneStar.Core.WebControl
{
    /// <summary>
    /// Represents a control that displays a split panel with a REST-backed list and a detail 
    /// frame for workspace data.
    /// </summary>
    public class ListDetailControl : ControlPanelSplit
    {
        /// <summary>
        /// Gets the configuration tile that provides REST access to 
        /// workspace data.
        /// </summary>
        public ControlRestList List { get; } = new ControlRestList()
        {
            Selectable = true,
            Sortable = true,
            Title = "List",
            Margin = new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.Two, PropertySpacing.Space.None, PropertySpacing.Space.None)
        };

        /// <summary>
        /// Gets the configuration tile that provides REST access to 
        /// workspace data.
        /// </summary>
        public ControlFrame Frame { get; } = new ControlFrame("frame_DD186C20B00041378929FF6B74D5A60B")
        {
            Selector = "#wx-content-main",
            Margin = new PropertySpacingMargin(PropertySpacing.Space.Two)
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
