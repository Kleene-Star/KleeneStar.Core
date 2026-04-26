using System.Text;
using System.Text.Json;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebRestApi;

namespace KleeneStar.Core.WWW.Api._1_.Forms.Form._formid_
{
    /// <summary>
    /// Provides a catalog of available form fields and their metadata for use in form generation or validation
    /// scenarios.
    /// </summary>
    [Title("Form field catalog")]
    public sealed class FieldCatalog : IRestApi
    {
        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
        private static readonly object[] _fields =
        [
            new { id = "Name",        type = "string"    },
            new { id = "Description", type = "text"      },
            new { id = "Email",       type = "string"    },
            new { id = "AppearsIn",   type = "ref"       },
            new { id = "Groups",      type = "tags"      },
            new { id = "Icon",        type = "file"      },
            new { id = "Status",      type = "enum"      },
            new { id = "Affiliation", type = "enum"      },
            new { id = "DueDate",     type = "timestamp" },
            new { id = "Priority",    type = "enum"      },
            new { id = "Weapon",      type = "string"    },
            new { id = "IslandOrigin",type = "ref"       }
        ];

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public FieldCatalog()
        {
        }

        /// <summary>
        /// Returns the field catalog as <c>{ "fields": [ … ] }</c>.
        /// </summary>
        /// <param name="request">The incoming request.</param>
        /// <returns>200 OK with the catalog JSON.</returns>
        [Method(RequestMethod.GET)]
        public IResponse Retrieve(IRequest request)
        {
            var json = JsonSerializer.Serialize(new { fields = _fields }, _jsonOptions);
            var content = Encoding.UTF8.GetBytes(json);

            return new ResponseOK
            {
                Content = content
            }
                .AddHeaderContentType("application/json");
        }
    }
}
