using System;
using WebExpress.WebApp.WebControl;
using WebExpress.WebApp.WebData;
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
        /// Represents the unique identifier for the list used in this context.
        /// </summary>
        public static readonly string ListId = "id_8C7E2A1B4D6F49A2B5C3E8F1A7D9B4E2";

        /// <summary>
        /// Gets or sets the delegate used to generate a RESTful URI based on the provided rendering context.
        /// When set, the inner <see cref="List"/> control receives a <see cref="ControlDataList.ServiceFactory"/>
        /// descriptor that queries this URI. Clears the descriptor when set to <see langword="null"/>.
        /// </summary>
        public Func<IRenderControlContext, IUri> RestUri
        {
            get => renderContext => InnerRestUri?.Invoke(renderContext);
            set => InnerRestUri = value;
        }

        private Func<IRenderControlContext, IUri> InnerRestUri { get; set; }

        /// <summary>
        /// Gets or sets the data service descriptor for the inner <see cref="List"/> control.
        /// Mirrors the property on <see cref="ControlDataList"/>; setting it routes the data
        /// service straight to the list without going through <see cref="RestUri"/>.
        /// </summary>
        public Func<IRenderControlContext, DataServiceDescriptor> ServiceFactory
        {
            get => List.ServiceFactory;
            set => List.ServiceFactory = value;
        }

        /// <summary>
        /// Gets or sets the delegate used to create a binding for a given render control context.
        /// </summary>
        public Func<IRenderControlContext, IBinding> Bind { get; set; }

        /// <summary>
        /// Gets the configuration tile that provides REST access to
        /// workspace data.
        /// </summary>
        public ControlDataList List { get; } = new ControlDataList(ListId)
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
        /// Gets the dismissible panel that hosts the detail <see cref="Frame"/>. The
        /// panel hides when the user clicks its dismiss button and is re-shown via
        /// <see cref="BindShow"/> whenever a list entry is activated.
        /// </summary>
        public ControlPanelDismissible PanelDismissible { get; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="id">The unique identifier for the control.</param>
        public ListDetailControl(string id = null)
            : base(id)
        {
            List.ServiceFactory = renderContext => DataServiceDescriptor.QueryData(RestUri?.Invoke(renderContext)?.ToString());
            List.Bind = renderContext => Bind?.Invoke(renderContext);

            PanelDismissible = new ControlPanelDismissible()
            {
                Bind = _ => new Binding().Add(new BindShow { Source = ListId })
            };
            PanelDismissible.Add(Frame);

            AddSidePanel(List);
            AddMainPanel(PanelDismissible);

            SidePanelInitialSize = _ => 250;
        }
    }
}
