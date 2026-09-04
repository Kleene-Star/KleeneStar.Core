using System;
using WebExpress.WebApp.WebControl;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Object
{
    /// <summary>
    /// Builds the collaborative container an object's editing surfaces are rendered inside, so
    /// two people working on the same object see each other doing it: who is here, where their
    /// pointer is, where their caret sits, and what they type into a plain field.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The container id <b>is</b> the routing channel: the framework's control filters incoming
    /// messages by it, so everybody on the same object has to be given the same id and nobody
    /// else may be. That is why it is derived from the object id here rather than spelled out at
    /// each call site - a literal would drift, and a drifted id fails silently as "nobody else is
    /// here" rather than as an error.
    /// </para>
    /// <para>
    /// The transport is the message queue the application already runs; there is no server
    /// component to add. Peers reach each other when their sockets share a domain, which every
    /// object surface does through the data services it declares.
    /// </para>
    /// <para>
    /// <b>Framework gap.</b> The control value-syncs <c>input</c> and <c>textarea</c> only - see
    /// <c>_onInput</c> in <c>webexpress.webapp.collaborative.js</c>, which deliberately skips
    /// contenteditable hosts because writing rich HTML through <c>textContent</c> would flatten
    /// it. On the prose editor that means the title is mirrored live while the body is not; the
    /// body converges through the shared draft instead (one <see cref="Model.Entities.ObjectDraft"/>
    /// per object, last autosave wins). Closing the gap is a framework change: give the
    /// collaborative control an HTML-valued input message for editor hosts and apply it through
    /// the editor's own value API rather than through the DOM, so the remote caret and the
    /// undo history survive the write.
    /// </para>
    /// </remarks>
    internal static class ObjectCollaborativeHost
    {
        /// <summary>
        /// Builds the collaborative container for the supplied object, populated with the
        /// supplied controls.
        /// </summary>
        /// <param name="object">The object whose surface is shared. Must not be null.</param>
        /// <param name="surface">A short token naming which surface of the object this is, so
        /// two surfaces of one object stay separate channels rather than echoing each other.</param>
        /// <param name="renderContext">The render context, read for the current identity.</param>
        /// <param name="controls">The controls rendered inside the container.</param>
        /// <returns>The configured container.</returns>
        public static ControlCollaborative Create
        (
            Model.Entities.Object @object,
            string surface,
            IRenderControlContext renderContext,
            params IControl[] controls
        )
        {
            ArgumentNullException.ThrowIfNull(@object);

            var identityId = CoreHub.SessionManager.GetCurrentIdentityId(renderContext?.Request);
            var identity = identityId == Guid.Empty ? null : CoreHub.IdentityManager.GetIdentity(identityId);

            return new ControlCollaborative("collab-" + surface + "-" + @object.Id.ToString("N"), controls)
            {
                Classes = ["wx-kleenestar-collaborative"],
                Presence = _ => true,
                Cursor = _ => true,
                Input = _ => true,

                // an anonymous request still takes part - it is given no id here and the client
                // generates one per session, so a reader without an account shows up as a
                // visitor rather than as a second copy of whoever else is nameless
                UserId = _ => identityId == Guid.Empty ? null : identityId.ToString("N"),
                UserName = _ => identity?.Name
            };
        }
    }
}
