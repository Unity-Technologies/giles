using UnityEngine;
using System.Runtime.Serialization;

namespace GILES.Serialization
{
	/**
	 * Defines a interface that allows for pb_ObjectContainer types to be treated as anonymous.
	 */
	public interface pb_ObjectWrapper
	{
		/**
		 * Return the contained value (actual type as defined by T).
		 */
		object GetValue();
	}

	/**
	 * Json.Net when deserializing arrays or child objects will not invoke custom converters
	 * because Deserialize<T> is called with `object` as the type, not the correct type.  By
	 * storing custom classes in container objects with strongly typed properties it is
	 * possible to circumvent this restriction.
	 */
	public class pb_ObjectContainer<T> : ISerializable, pb_ObjectWrapper
	{
		/// The value to be serialized.
		public T value;

		/**
		 * Create a new container object with T type.
		 */
		public pb_ObjectContainer(T value)
		{
			this.value = value;
		}

		/**
		 * Return the value stored in this container.
		 */
		public static implicit operator T(pb_ObjectContainer<T> container)
		{
			return container.value;
		} 

		/**
		 * Return the type contained within this wrapper.
		 */
		public new System.Type GetType()
		{
			return typeof(T);
		}

		/**
		 * Get the contained value.
		 */
		public object GetValue()
		{
			return (T) value;
		}

		/**
		 * Constructor coming from serialization.
		 */
		public pb_ObjectContainer(SerializationInfo info, StreamingContext context)
		{
			value = (T) info.GetValue("value", typeof(T));
		}

		/**
		 * Serialize data for ISerializable.
		 */
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("value", value, typeof(T));
		}

		public override string ToString()
		{
			return "Container: " + this.value.ToString();
		}
	}
}