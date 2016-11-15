using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GILES.Serialization
{
	/**
	 * Interface for objects that want to customize serialization process at the 
	 * pre-Json.NET level.  Converters for Components fall into this category - 
	 * see pb_SerializableObject class for more on customizing the serialization
	 * pipeline.
	 */
	public interface pb_ISerializable : ISerializable
	{
		/// The type of component stored.
		System.Type type { get; set; }

		/// Called after an object is deserialized and constructed to it's base `type`.
		void ApplyProperties(object obj);

		/// Called before serialization, any properties stoed in the returned dictionary
		/// will be saved and re-applied in ApplyProperties.
		Dictionary<string, object> PopulateSerializableDictionary();
	}	
}