using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System;
using System.Linq;

namespace GILES.Serialization
{
	public class pb_ColorConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			JObject o = new JObject();

			o.Add("$type", value.GetType().AssemblyQualifiedName);
			
			if(value is Color)
			{
				o.Add("r", ((UnityEngine.Color)value).r);
				o.Add("g", ((UnityEngine.Color)value).g);
				o.Add("b", ((UnityEngine.Color)value).b);
				o.Add("a", ((UnityEngine.Color)value).a);
			}
			else
			{
				o.Add("r", ((UnityEngine.Color32)value).r);
				o.Add("g", ((UnityEngine.Color32)value).g);
				o.Add("b", ((UnityEngine.Color32)value).b);
				o.Add("a", ((UnityEngine.Color32)value).a);
			}

			o.WriteTo(writer, serializer.Converters.ToArray());
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject o = JObject.Load(reader);

			if(typeof(UnityEngine.Color32).IsAssignableFrom(objectType))
			{
				return new Color32(	(byte) o.GetValue("r"),
									(byte) o.GetValue("g"),
									(byte) o.GetValue("b"),
									(byte) o.GetValue("a"));
			}
			else
			{
				return new Color( 	(float) o.GetValue("r"),
									(float) o.GetValue("g"),
									(float) o.GetValue("b"),
									(float) o.GetValue("a"));
			}
	}

		public override bool CanConvert(Type objectType)
		{
			return typeof(UnityEngine.Color).IsAssignableFrom(objectType);
		}
	}
}