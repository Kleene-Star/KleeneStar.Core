using System;
using System.Security.Cryptography;

namespace KleeneStar.Core.WebControl
{
    /// <summary>
    /// An image an avatar control submitted inline: the decoded bytes, the file extension
    /// matching its media type, and a short fingerprint of the content.
    /// </summary>
    /// <remarks>
    /// <see cref="WebExpress.WebUI.WebControl.ControlFormItemInputAvatar"/> does not upload to
    /// an endpoint of its own; it posts the picture as part of the form value, shaped as
    /// <c>file:&lt;name&gt;;data:&lt;mime&gt;;base64,&lt;payload&gt;</c>. Nothing in WebExpress
    /// takes that apart — <c>RestValueConverterImageIcon</c> hands the whole string to
    /// <c>ImageIcon.FromString</c>, which parses it as a URI and yields <c>http:///</c>. This
    /// type is the missing step: it turns the submitted value into something that can be
    /// written to disk and served.
    /// </remarks>
    /// <param name="Content">The decoded image bytes.</param>
    /// <param name="Extension">The file extension for the media type, including the dot.</param>
    /// <param name="Fingerprint">A short, stable hash of <paramref name="Content"/>.</param>
    public sealed record ImagePayload(byte[] Content, string Extension, string Fingerprint)
    {
        /// <summary>
        /// The media types the icon route can serve, mapped to the extension they are stored
        /// under. A picture of any other type is rejected rather than stored under a name the
        /// route would answer as <c>application/octet-stream</c>.
        /// </summary>
        private static readonly (string MediaType, string Extension)[] _supported =
        [
            ("image/png", ".png"),
            ("image/jpeg", ".jpg"),
            ("image/jpg", ".jpg"),
            ("image/gif", ".gif"),
            ("image/webp", ".webp"),
            ("image/svg+xml", ".svg")
        ];

        /// <summary>
        /// The largest picture that is accepted, in bytes. An avatar is cropped and scaled down
        /// by the control before it is submitted, so anything past this is not a portrait but a
        /// mistake, and it would otherwise be written to the icons directory unchallenged.
        /// </summary>
        private const int MaxContentLength = 4 * 1024 * 1024;

        /// <summary>
        /// Parses the value an avatar control submitted.
        /// </summary>
        /// <param name="payload">
        /// The submitted value. Accepts both the <c>file:&lt;name&gt;;&lt;data url&gt;</c> shape
        /// the control posts and a bare data URL.
        /// </param>
        /// <returns>
        /// The parsed image, or <see langword="null"/> when the payload is empty, is not a data
        /// URL, carries a media type the icon route cannot serve, or exceeds
        /// <see cref="MaxContentLength"/>.
        /// </returns>
        public static ImagePayload Parse(string payload)
        {
            if (string.IsNullOrWhiteSpace(payload))
            {
                return null;
            }

            var value = payload.Trim();

            // strip the "file:<name>;" prefix the control puts in front of the data url. The
            // name is the file the user picked and says nothing about the content, so it is
            // dropped rather than used: it would otherwise decide a path on disk.
            if (value.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
            {
                var separator = value.IndexOf(';');

                if (separator < 0)
                {
                    return null;
                }

                value = value[(separator + 1)..].TrimStart();
            }

            if (!value.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var comma = value.IndexOf(',');

            if (comma < 0)
            {
                return null;
            }

            var header = value[5..comma];
            var body = value[(comma + 1)..];

            if (!header.EndsWith(";base64", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var mediaType = header[..^";base64".Length].Trim();
            var extension = ExtensionFor(mediaType);

            if (extension is null)
            {
                return null;
            }

            byte[] content;

            try
            {
                content = Convert.FromBase64String(body);
            }
            catch (FormatException)
            {
                return null;
            }

            if (content.Length == 0 || content.Length > MaxContentLength)
            {
                return null;
            }

            return new ImagePayload(content, extension, ComputeFingerprint(content));
        }

        /// <summary>
        /// Returns the extension the given media type is stored under, or
        /// <see langword="null"/> when the icon route cannot serve it.
        /// </summary>
        /// <param name="mediaType">The media type taken from the data URL header.</param>
        /// <returns>The file extension including the dot, or <see langword="null"/>.</returns>
        private static string ExtensionFor(string mediaType)
        {
            foreach (var (supported, extension) in _supported)
            {
                if (string.Equals(mediaType, supported, StringComparison.OrdinalIgnoreCase))
                {
                    return extension;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a short, stable hash of the content, used to give a replaced picture a file
        /// name of its own.
        /// </summary>
        /// <param name="content">The image bytes.</param>
        /// <returns>Eight lower-case hexadecimal characters.</returns>
        private static string ComputeFingerprint(byte[] content)
        {
            var hash = SHA256.HashData(content);

            return Convert.ToHexString(hash)[..8].ToLowerInvariant();
        }
    }
}
