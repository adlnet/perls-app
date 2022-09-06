using System;
using JsonNet.PrivateSettersContractResolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using PERLS.Data.Definition;
using PERLS.DataImplementation.Models;

namespace PERLS.DataImplementation
{
    /// <summary>
    /// The custom creation converter for IRemoteResource.
    /// </summary>
    public class RemoteResourceCustomCreationConverter : CustomCreationConverter<IRemoteResource>
    {
        /// <inheritdoc/>
        public override IRemoteResource Create(Type objectType)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            JObject theObject = JObject.Load(reader);
            var jsonString = theObject.ToString(Formatting.None);
            var targetType = GetType(theObject);

            if (targetType == null)
            {
                return null;
            }

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterContractResolver(),
            };

            return JsonConvert.DeserializeObject(jsonString, targetType, settings);
        }

        Type GetType(JObject theObject)
        {
            if (theObject.ContainsKey("nid") && theObject.ContainsKey("vid"))
            {
                return typeof(Node);
            }

            if (theObject.ContainsKey("tid"))
            {
                return typeof(TaxonomyTerm);
            }

            if (theObject.ContainsKey("gid"))
            {
                return typeof(Group);
            }

            if (theObject.ContainsKey("login"))
            {
                return typeof(Learner);
            }

            return null;
        }
    }
}
