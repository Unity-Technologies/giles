using System;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GILES;

namespace GILES.Serialization
{
	/**
	 * Container class for Unity component types, used to serialize and reconstitute components.  If you want to override
	 * serialization behavior for your MonoBehaviour, implement the pb_ISerializableComponent interface.
	 */
	[System.Serializable]
	public class pb_SerializableObject<T> : pb_ISerializable
	{
		/// A reference to the component being serialized.  Will be null on deserialization.
		protected T target;
		
		public Type type { get; set; }

		/// A key-value store of all serializable properties and fields on this object.  Populated on serialization & deserialization.
		protected Dictionary<string, object> reflectedProperties;

		/**
		 * Create a new serializable object from a component.
		 */
		public pb_SerializableObject(T obj)
		{
			this.target = obj;
		}

		/**
		 * Explicit cast return target.  If obj is null but reflectedProperties is valid, a new instance
		 * of T is returned with those properties applied.  The new instance is constructed using default(T).
		 */
		public static explicit operator T(pb_SerializableObject<T> obj)
		{
			if(obj.target == null)
			{
				T val = default(T);
				obj.ApplyProperties(val);
				return val;
			}
			else
			{
				return obj.target;
			}
		}

		/**
		 * Constructor coming from serialization.
		 */
		public pb_SerializableObject(SerializationInfo info, StreamingContext context)
		{
			string typeName 		= (string) info.GetValue("typeName", typeof(string));
			type 					= Type.GetType(typeName);
			reflectedProperties 	= (Dictionary<string, object>) info.GetValue("reflectedProperties", typeof(Dictionary<string, object>));
		}

		/**
		 * Serialize data for ISerializable.
		 */
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			Type type = target.GetType();
			info.AddValue("typeName", type.AssemblyQualifiedName, typeof(string));			

			reflectedProperties = PopulateSerializableDictionary();
			info.AddValue("reflectedProperties", reflectedProperties, typeof(Dictionary<string, object>));
		}

		public virtual void ApplyProperties(object obj)
		{
			pb_ISerializableComponent ser = obj as pb_ISerializableComponent;

			if(ser != null)
			{
				ser.ApplyDictionaryValues(reflectedProperties);
			}
			else
			{
				pb_Reflection.ApplyProperties(obj, reflectedProperties);
			}
		}

		public virtual Dictionary<string, object> PopulateSerializableDictionary()
		{
			pb_ISerializableComponent ser = target as pb_ISerializableComponent;

			if(ser != null)
				return ser.PopulateSerializableDictionary();
			else
				return pb_Reflection.ReflectProperties(target);
		}
	}
}
