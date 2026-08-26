using KleeneStar.Core.WebManager;
using KleeneStar.Model.Entities;
using System.Collections.Generic;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebIndex.Queries;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace KleeneStar.Core.WebFragment.Landing
{
    /// <summary>
    /// The activity area: the last few things that happened in the installation, each as one
    /// sentence - who did what to which record, and how long ago.
    /// </summary>
    /// <remarks>
    /// Read from the audit log, which is the installation-wide record of what happened and the
    /// only source that covers every kind of change rather than one manager's. The sentence is
    /// composed from the typed fields (<see cref="AuditEvent.Action"/>,
    /// <see cref="AuditEvent.TargetType"/>) exactly as the audit view composes it - the model
    /// deliberately has no free-form message field, so a display sentence is always built,
    /// never stored.
    /// <para>
    /// A run of events in time is a timeline, and the control draws one: the entries hang off a
    /// single line in the order they happened, which is the shape that says "and then" without
    /// a word.
    /// </para>
    /// </remarks>
    internal static class LandingActivitySection
    {
        /// <summary>
        /// The number of events shown.
        /// </summary>
        private const int MaxItems = 6;

        /// <summary>
        /// Builds the section.
        /// </summary>
        /// <param name="auditManager">The audit manager the events are read from.</param>
        /// <param name="renderContext">The render context.</param>
        /// <param name="visualTree">The visual tree.</param>
        /// <returns>The section control.</returns>
        public static IControl Build(IAuditManager auditManager, IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var events = GetEvents(auditManager);

            var section = new ControlSection("landing-activity")
            {
                Header = _ => "kleenestar.core:landing.activity.card",
                HeaderIcon = _ => new IconClock(),
                Layout = _ => TypeLayoutSection.Rule
            };

            if (events.Count == 0)
            {
                section.Add(new ControlText("landing-activity-empty")
                {
                    Text = _ => "kleenestar.core:landing.activity.empty",
                    TextColor = _ => new PropertyColorText(TypeColorText.Secondary)
                });

                return section;
            }

            var timeline = new ControlTimeline("landing-activity-timeline");

            foreach (var @event in events)
            {
                timeline.Add(BuildEntry(@event, renderContext));
            }

            section.Add(timeline);

            return section;
        }

        /// <summary>
        /// Builds a single entry: the sentence naming what somebody did, and how long ago.
        /// </summary>
        /// <param name="event">The event to render.</param>
        /// <param name="renderContext">The render context.</param>
        /// <returns>The timeline item.</returns>
        private static ControlTimelineItem BuildEntry(AuditEvent @event, IRenderControlContext renderContext)
        {
            var actor = ResolveActor(@event, renderContext);

            // the two halves of the sentence fall in a different order per language - "created
            // an issue" against "hat einen Vorgang angelegt" - so the pattern carries the
            // placeholders and is translated before the parts are put in
            var predicate = string.Format
            (
                LandingHtml.Culture(renderContext),
                I18N.Translate(renderContext, "kleenestar.core:landing.activity.sentence"),
                I18N.Translate(renderContext, @event.TargetType.Text()),
                I18N.Translate(renderContext, @event.Action.Text())
            );

            var title = LandingHtml.Join(actor + " " + predicate, @event.TargetKey);
            var age = LandingHtml.Age(@event.Timestamp, renderContext);

            return new ControlTimelineItem("landing-activity-" + @event.Id.ToString("N"))
            {
                Title = _ => title,
                Timestamp = _ => age,
                Color = _ => new PropertyColorBackground(Tone(@event.Severity))
            };
        }

        /// <summary>
        /// Returns the colour a severity is marked with, so a warning stands out of a run of
        /// ordinary changes without being read as an error.
        /// </summary>
        /// <param name="severity">The severity of the event.</param>
        /// <returns>The background colour of the timeline marker.</returns>
        private static TypeColorBackground Tone(AuditSeverity severity)
        {
            return severity switch
            {
                AuditSeverity.Warning => TypeColorBackground.Warning,
                AuditSeverity.Notice => TypeColorBackground.Info,
                _ => TypeColorBackground.Secondary
            };
        }

        /// <summary>
        /// Resolves who caused an event: the identity's current name, falling back to the name
        /// snapshotted when the event was written, and finally to "system" for the events
        /// nobody caused.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <param name="renderContext">The render context used for translating.</param>
        /// <returns>The actor's display name.</returns>
        private static string ResolveActor(AuditEvent @event, IRenderControlContext renderContext)
        {
            var identity = @event.ActorId.HasValue
                ? CoreHub.IdentityManager?.GetIdentity(@event.ActorId.Value)
                : null;

            return identity?.Name
                ?? @event.ActorName
                ?? I18N.Translate(renderContext, "kleenestar.core:audit.actor.system");
        }

        /// <summary>
        /// Fetches the newest audit events.
        /// </summary>
        /// <param name="auditManager">The audit manager.</param>
        /// <returns>The capped, newest-first set of events. The list may be empty.</returns>
        private static IReadOnlyList<AuditEvent> GetEvents(IAuditManager auditManager)
        {
            var query = new Query<AuditEvent>()
                .OrderByDesc(x => x.Sequence)
                .WithPaging(0, MaxItems);

            return [.. auditManager.GetEvents(query)];
        }
    }
}
