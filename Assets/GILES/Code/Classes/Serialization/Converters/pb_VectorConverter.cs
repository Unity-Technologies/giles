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
	 * JsonConverter override for serializing Vector{2,3,4} and Quaternion types from UnityEngine.
	 */
	public class pb_VectorConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			JObject o = new JObject();

			o.Add("$type", value.GetType().AssemblyQualifiedName);

			if( value is Vector2 )
			{
				o.Add("x", ((UnityEngine.Vector2)value).x);
				o.Add("y", ((UnityEngine.Vector2)value).y);
			}
			else if( value is Vector3 )
			{
				o.Add("x", ((UnityEngine.Vector3)value).x);
				o.Add("y", ((UnityEngine.Vector3)value).y);
				o.Add("z", ((UnityEngine.Vector3)value).z);
			}
			else if( value is Vector4 )
			{
				o.Add("x", ((UnityEngine.Vector4)value).x);
				o.Add("y", ((UnityEngine.Vector4)value).y);
				o.Add("z", ((UnityEngine.Vector4)value).z);
				o.Add("w", ((UnityEngine.Vector4)value).w);
			}
			else if( value is Quaternion )
			{
				o.Add("x", ((UnityEngine.Quaternion)value).x);
				o.Add("y", ((UnityEngine.Quaternion)value).y);
				o.Add("z", ((UnityEngine.Quaternion)value).z);
				o.Add("w", ((UnityEngine.Quaternion)value).w);
			}

			o.WriteTo(writer, serializer.Converters.ToArray());
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject o = JObject.Load(reader);

			if(objectType == typeof(Vector2))
			{
				return new Vector2(	(float) o.GetValue("x"),
									(float) o.GetValue("y"));
			}
			else
			if(objectType == typeof(Vector3))
			{
				return new Vector3(	(float) o.GetValue("x"),
									(float) o.GetValue("y"),
									(float) o.GetValue("z"));
			}
			else
			if(objectType == typeof(Vector4))
			{
				return new Vector4(	(float) o.GetValue("x"),
									(float) o.GetValue("y"),
									(float) o.GetValue("z"),
									(float) o.GetValue("w"));
			}
			else
			if(objectType == typeof(Quaternion))
			{
				return new Quaternion(	(float) o.GetValue("x"),
										(float) o.GetValue("y"),
										(float) o.GetValue("z"),
										(float) o.GetValue("w"));
			}
			
			return new Vector3(0,0,0);
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(UnityEngine.Vector2).IsAssignableFrom(objectType) || typeof(UnityEngine.Quaternion).IsAssignableFrom(objectType);
		}
	}
}