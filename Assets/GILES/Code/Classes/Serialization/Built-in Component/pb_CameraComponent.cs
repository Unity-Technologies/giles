using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GILES;

namespace GILES.Serialization
{
	/**
	 * Specialized component wrapper for serializing and deserializing Camera components.
	 */
	public class pb_CameraComponent : pb_SerializableObject<UnityEngine.Camera>
	{
		public pb_CameraComponent(UnityEngine.Camera obj) : base(obj) {}
		public pb_CameraComponent(SerializationInfo info, StreamingContext context) : base(info, context) {}

		public override Dictionary<string, object> PopulateSerializableDictionary()
		{
			Dictionary<string, object> props = pb_Reflection.ReflectProperties(target);

			props.Remove("worldToCameraMatrix");
			props.Remove("projectionMatrix");

			return props;
		}
	}
}