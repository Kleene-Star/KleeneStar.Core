using System;
using WebExpress.WebApp.WebControl;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebControl
{
    /// <summary>
    /// Represents a control that displays a split panel with a REST-backed list and a detail 
    /// frame for workspace data.
    /// </summary>
    public class ListDetailControl : ControlPanelSplit
    {
        /// <summary>
        /// Represents the unique identifier for the frame used in this context.
        /// </summary>
        public static readonly string FrameId = "id_D85AD7256B374857914E70938DFA1F81";

        /// <summary>
        /// Gets or sets the delegate used to generate a RESTful URI based on the provided rendering context.
        /// </summary>
        public Func<IRenderControlContext, IUri> RestUri { get; set; }

        /// <summary>
        /// Gets or sets the delegate used to create a binding for a given render control context.
        /// </summary>
        public Func<IRenderControlContext, IBinding> Bind { get; set; }

        /// <summary>
        /// Gets the configuration tile that provides REST access to 
        /// workspace data.
        /// </summary>
        public ControlRestList List { get; } = new ControlRestList()
        {
            Selectable = _ => true,
            Sortable = _ => true,
            Title = _ => "List",
            Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.Two, PropertySpacing.Space.None, PropertySpacing.Space.None)
        };

        /// <summary>
        /// Gets the configuration tile that provides REST access to 
        /// workspace data.
        /// </summary>
        public ControlFrame Frame { get; } = new ControlFrame(FrameId)
        {
            Selector = _ => "#wx-content-main",
            Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two)
        };

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="id">The unique identifier for the control.</param>
        public ListDetailControl(string id = null)
            : base(id)
        {
            List.RestUri = renderContext => RestUri?.Invoke(renderContext);
            List.Bind = renderContext => Bind?.Invoke(renderContext);

            AddSidePanel(List);
            AddMainPanel(Frame);

            SidePanelInitialSize = _ => 250;
        }
    }
}
