using System;
using Newtonsoft.Json;

namespace PERLS.Data.Converters
{
    /// <summary>
    /// A converter for JSON data that represents a relative URI.
    /// </summary>
    public class RelativeUriConverter : JsonConverter
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException(nameof(objectType));
            }

            return objectType.Equals(typeof(Uri));
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            switch (reader.TokenType)
            {
                case JsonToken.String:
                    if (reader.Value is string value)
                    {
                        return new Uri(value, UriKind.Relative);
                    }

                    if (existingValue == null)
                    {
                        return null;
                    }

                    break;
            }

            throw new InvalidOperationException();
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
