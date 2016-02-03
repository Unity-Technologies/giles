using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System;
using System.Linq;

namespace GILES.Serialization
{
	/**
	 * Creates a wrapper around a type when serializing so that the correct type converter is
	 * called when deserializing.  Allows type requring a converter to be properly deserialized
	 * when stored in a non-strongly-typed structure (Dictionary<string, object> for example).
	 */
	public abstract class pb_UnityTypeConverter<T> : JsonConverter
	{
		/**
		 * Wrap `value` in a `pb_ObjectContainer<T>` type in JSON.  Override `WriteObjectJson` to 
		 * populate the value fields.
		 */
		public sealed override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteStartObject();
			writer.WritePropertyName("$type");
			writer.WriteValue(typeof(pb_ObjectContainer<T>).AssemblyQualifiedName);
			writer.WritePropertyName("value");

			WriteObjectJson(writer, value, serializer);

			writer.WriteEndObject();
		}

		/**
		 * Behaves like WriteJson.
		 */
		public abstract void WriteObjectJson(JsonWriter writer, object value, JsonSerializer serializer);

		public sealed override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject o = JObject.Load(reader);

			JProperty type = o.Property("$type");

			if(type != null)
			{
				System.Type t = Type.GetType(type.Value.ToString());

				if(t == typeof(pb_ObjectContainer<T>))
					return (T) ((pb_ObjectContainer<T>)o.ToObject(typeof(pb_ObjectContainer<T>), serializer)).value;
			}

			return ReadJsonObject(o, objectType, existingValue, serializer);
		}

		/**
		 * Behaves like ReadJson, which is overridden to handle conversion between wrapper and actual type.
		 */
		public abstract object ReadJsonObject(JObject obj, Type objectType, object existingValue, JsonSerializer serializer);

		/**
		 * Returns true if type can be wrapped by this class.
		 */
		public sealed override bool CanConvert(Type objectType)
		{
			return typeof(T).IsAssignableFrom(objectType) || typeof(pb_ObjectContainer<T>).IsAssignableFrom(objectType);
		}
	}
}