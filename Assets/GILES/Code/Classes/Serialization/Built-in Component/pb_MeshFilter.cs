using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GILES.Serialization
{
	/**
	 * Specialized component wrapper for serializing and deserializing MeshFilter components.
	 */
	public class pb_MeshFilter : pb_SerializableObject<UnityEngine.MeshFilter>
	{
		public pb_MeshFilter(UnityEngine.MeshFilter obj) : base(obj) {}
		public pb_MeshFilter(SerializationInfo info, StreamingContext context) : base(info, context) {}

		public override Dictionary<string, object> PopulateSerializableDictionary()
		{
			Dictionary<string, object> props = new Dictionary<string, object>(); // pb_Reflection.ReflectProperties(target);

			props.Add("sharedMesh", target.sharedMesh);
			props.Add("tag", target.tag);
			props.Add("name", target.name);
			props.Add("hideFlags", target.hideFlags);

			return props;
		}
	}
}