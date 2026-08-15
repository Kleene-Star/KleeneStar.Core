using KleeneStar.Core.WebControl;

namespace KleeneStar.Core.Test.WebControl
{
    /// <summary>
    /// Provides unit tests for <see cref="ImagePayload"/> — the picture an avatar control
    /// submits inline, taken apart into the bytes that get written to disk.
    /// </summary>
    /// <remarks>
    /// The control posts <c>file:&lt;name&gt;;data:&lt;mime&gt;;base64,&lt;payload&gt;</c>.
    /// Everything that is not exactly that has to be turned away rather than stored, because
    /// what comes out of here decides both a file name and the bytes behind it.
    /// </remarks>
    public class UnitTestImagePayload
    {
        /// <summary>
        /// A one-pixel PNG, base64 encoded — the smallest payload that is a real image.
        /// </summary>
        private const string PixelPng =
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==";

        /// <summary>
        /// Verifies that the shape the avatar control actually posts is accepted and that the
        /// file name it carries is discarded — it names a file on the user's machine and must
        /// not decide one on the server.
        /// </summary>
        [Fact]
        public void ControlShape_IsParsed()
        {
            var result = ImagePayload.Parse($"file:portrait.png;data:image/png;base64,{PixelPng}");

            Assert.NotNull(result);
            Assert.Equal(".png", result.Extension);
            Assert.NotEmpty(result.Content);
        }

        /// <summary>
        /// Verifies that a bare data url is accepted too, so a caller that already stripped
        /// the prefix does not have to put it back.
        /// </summary>
        [Fact]
        public void BareDataUrl_IsParsed()
        {
            var result = ImagePayload.Parse($"data:image/png;base64,{PixelPng}");

            Assert.NotNull(result);
            Assert.Equal(".png", result.Extension);
        }

        /// <summary>
        /// Verifies the extension chosen per media type. It decides how the icon route later
        /// labels the response, so a JPEG must not end up stored as <c>.png</c>.
        /// </summary>
        /// <param name="mediaType">The media type under test.</param>
        /// <param name="expected">The expected file extension.</param>
        [Theory]
        [InlineData("image/png", ".png")]
        [InlineData("image/jpeg", ".jpg")]
        [InlineData("image/jpg", ".jpg")]
        [InlineData("image/gif", ".gif")]
        [InlineData("image/webp", ".webp")]
        [InlineData("image/svg+xml", ".svg")]
        [InlineData("IMAGE/PNG", ".png")]
        public void MediaType_SelectsExtension(string mediaType, string expected)
        {
            var result = ImagePayload.Parse($"data:{mediaType};base64,{PixelPng}");

            Assert.NotNull(result);
            Assert.Equal(expected, result.Extension);
        }

        /// <summary>
        /// Verifies that identical content yields the same fingerprint and different content
        /// does not. The fingerprint is what gives a replaced picture a URI of its own, and
        /// the icon route answers with a one-year immutable cache — a fingerprint that did
        /// not change would leave every browser on the old picture.
        /// </summary>
        [Fact]
        public void Fingerprint_FollowsTheContent()
        {
            var first = ImagePayload.Parse($"data:image/png;base64,{PixelPng}");
            var again = ImagePayload.Parse($"file:other-name.png;data:image/png;base64,{PixelPng}");
            var other = ImagePayload.Parse("data:image/gif;base64,R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==");

            Assert.Equal(first.Fingerprint, again.Fingerprint);
            Assert.NotEqual(first.Fingerprint, other.Fingerprint);
        }

        /// <summary>
        /// Verifies that the fingerprint is safe to put in a file name.
        /// </summary>
        [Fact]
        public void Fingerprint_IsShortAndHexadecimal()
        {
            var result = ImagePayload.Parse($"data:image/png;base64,{PixelPng}");

            Assert.Equal(8, result.Fingerprint.Length);
            Assert.All(result.Fingerprint, c => Assert.True("0123456789abcdef".Contains(c)));
        }

        /// <summary>
        /// Verifies that a payload which is not a usable picture is turned away. The caller
        /// reads null as "nothing given" and keeps the picture that is already there, so a
        /// rejection here costs the user nothing — storing any of these would.
        /// </summary>
        /// <param name="payload">The payload under test.</param>
        [Theory]
        // nothing submitted
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        // a plain URI, which is what the broken path used to produce
        [InlineData("http:///")]
        [InlineData("/kleenestar/assets/icons/some.svg")]
        // the control prefix without a payload behind it
        [InlineData("file:portrait.png")]
        // not a data url
        [InlineData("file:portrait.png;portrait.png")]
        // a data url that is not base64 encoded
        [InlineData("data:image/png,notbase64")]
        // base64 that does not decode
        [InlineData("data:image/png;base64,!!!!")]
        // an empty image
        [InlineData("data:image/png;base64,")]
        // a media type the icon route cannot serve — an executable must never reach the
        // icons directory just because it was announced as an avatar
        [InlineData("data:application/x-msdownload;base64,TVqQAAMAAAAEAAAA")]
        [InlineData("data:text/html;base64,PHNjcmlwdD5hbGVydCgxKTwvc2NyaXB0Pg==")]
        public void UnusablePayload_IsRejected(string payload)
        {
            Assert.Null(ImagePayload.Parse(payload));
        }
    }
}
