using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Wiz.Gringotts.UIWeb.Infrastructure.Extensions
{
    public static class ControllerExtensions
    {
        public static ContentResult JsonNet(this ControllerBase controller, object model,
            IEnumerable<string> fields = null)
        {
            var serialized = JsonConvert.SerializeObject(model, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new FieldListContractResolver(fields ?? new[] {"*"})
            });

            return new ContentResult
            {
                Content = serialized,
                ContentType = "application/json"
            };
        }

        private class FieldListContractResolver : DefaultContractResolver
        {
            private readonly IEnumerable<string> fields;

            public FieldListContractResolver(IEnumerable<string> fields)
            {
                this.fields = fields;
            }

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);

                if (fields.Any(f => f.Equals("*")))
                    return property;

                if (!fields.Any(f => property.PropertyName.ToLower().Equals(f.ToLower())))
                    property.Ignored = true;

                return property;
            }
        }
    }
}