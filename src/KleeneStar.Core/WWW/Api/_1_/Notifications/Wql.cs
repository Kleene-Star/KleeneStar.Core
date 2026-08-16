using KleeneStar.Model.Entities;
using System.Collections.Generic;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;

namespace KleeneStar.Core.WWW.Api._1_.Notifications
{
    /// <summary>
    /// Provides the search prompt of the notification center.
    /// </summary>
    [Cache]
    public sealed class Wql : RestApiWqlPrompt<UserNotification>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Wql()
        {
        }

        /// <summary>
        /// Retrieves the suggestions offered under the search box.
        /// </summary>
        /// <remarks>
        /// The examples name the two things a reader remembers about a notification: what it
        /// was about, and whether they have dealt with it yet.
        /// </remarks>
        /// <param name="request">The triggering request.</param>
        /// <returns>The suggested queries.</returns>
        protected override IEnumerable<string> GetHistory(IRequest request)
        {
            yield return "Subject ~ \"BUG-\"";
            yield return "Read = false";
            yield return "TitleKey ~ \"created\"";
        }
    }
}
