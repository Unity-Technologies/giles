#undef PB_DEBUG

using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using GILES;

namespace GILES.Serialization
{
	/**
	 * @todo
	 */
	public class pb_ContractResolver : DefaultContractResolver
	{
		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			JsonProperty property = base.CreateProperty(member, memberSerialization);

			property.ShouldSerialize = instance =>
			{
#if !PB_DEBUG
				try
				{
#endif
					if( pb_Reflection.HasIgnoredAttribute(member) )
						return false;

					if(member is PropertyInfo)
					{
						PropertyInfo prop = (PropertyInfo) member;

						if (prop.CanRead && prop.CanWrite && !prop.IsSpecialName)
						{
							prop.GetValue(instance, null);
							return true;
						}
					}
					else if(member is FieldInfo)
					{
						return true;
					}
#if !PB_DEBUG
				}
				catch (System.Exception e) {
					Debug.LogWarning("Can't create field \"" + member.Name + "\" " + member.DeclaringType + " -> " + member.ReflectedType + "\n\n" + e.ToString());
				}
#endif

				return false;
			};

			return property;
		}

		/**
		 * Return special case type converters.
		 */
		protected override JsonConverter ResolveContractConverter(Type type)
		{
			if( typeof(UnityEngine.Color).IsAssignableFrom(type) || typeof(UnityEngine.Color32).IsAssignableFrom(type) )
				return GetConverter<pb_ColorConverter>();

			if( typeof(UnityEngine.Matrix4x4).IsAssignableFrom(type) )
				return GetConverter<pb_MatrixConverter>();

			if( typeof(UnityEngine.Vector2).IsAssignableFrom(type) ||
				typeof(UnityEngine.Vector3).IsAssignableFrom(type) ||
				typeof(UnityEngine.Vector4).IsAssignableFrom(type) ||
				typeof(UnityEngine.Quaternion).IsAssignableFrom(type) )
				return GetConverter<pb_VectorConverter>();

			if( typeof(UnityEngine.Mesh).IsAssignableFrom(type))
				return GetConverter<pb_MeshConverter>();

			if( typeof(UnityEngine.Material).IsAssignableFrom(type))
				return GetConverter<pb_MaterialConverter>();

			return base.ResolveContractConverter(type);
		}

		static Dictionary<Type, JsonConverter> converters = new Dictionary<Type, JsonConverter>();

		private static T GetConverter<T>() where T : JsonConverter, new()
		{
			JsonConverter conv;

			if(converters.TryGetValue(typeof(T), out conv))
				return (T) conv;

			conv = new T();

			converters.Add(typeof(T), conv);

			return (T) conv;
		}
	}
}