using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System;
using System.Linq;

namespace GILES.Serialization
{
	public class pb_ObjectConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
	        throw new NotImplementedException("Cannot write objects!");
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
	        throw new NotImplementedException("Cannot read objects!");

			// @todo - #16
			// JObject o = JObject.Load(reader);
			// var obj = o.GetValue("value").ToObject<dynamic>(serializer);
			// return ((pb_ObjectWrapper)obj).GetValue();
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType is pb_ObjectWrapper;
		}

		public override bool CanWrite
		{
			get { return false; }
		}
	}
}
