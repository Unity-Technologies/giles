using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GILES;

namespace GILES.Serialization
{
	public class pb_MeshRenderer : pb_SerializableObject<UnityEngine.MeshRenderer>
	{
		public pb_MeshRenderer(UnityEngine.MeshRenderer obj) : base(obj) {}
		public pb_MeshRenderer(SerializationInfo info, StreamingContext context) : base(info, context) {}

		public override Dictionary<string, object> PopulateSerializableDictionary()
		{
			Dictionary<string, object> props = pb_Reflection.ReflectProperties(target, new HashSet<string>() 
				{
					"material",
					"sharedMaterial",
					"materials"
				});

			return props;
		}
	}
}