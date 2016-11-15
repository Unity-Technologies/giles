using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System;
using System.Linq;

namespace GILES.Serialization
{
	public class pb_MaterialConverter : pb_UnityTypeConverter<Material>
	{
		public override void WriteObjectJson(JsonWriter writer, object value, JsonSerializer serializer)
	    {
	    	JObject o = new JObject();

	    	Material mat = ((UnityEngine.Material)value);

	    	o.Add("name", mat.name);
			o.Add("shader", mat.shader.name.ToString());   	
			o.Add("shaderObj", JObject.FromObject(mat.shader));

	    	o.WriteTo(writer, serializer.Converters.ToArray());
	    }

	    public override object ReadJsonObject(JObject obj, Type objectType, object existingValue, JsonSerializer serializer)
	    {
	    	try
	    	{
	    		string name = obj.GetValue("name").ToObject<string>();
	    		string shader = obj.GetValue("shader").ToObject<string>();

	    		Material mat = new Material(Shader.Find(shader));
	    		mat.name = name;
	    		
	    		return mat;
	    	}
	    	catch
	    	{
	    		return null;
	    	}
	    }
	}
}