// #define REFLECTION_DEBUG

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using GILES.Serialization;

namespace GILES
{
	/**
	 * Helper class for working with reflection.  Automates some common functionality like
	 * extracting/applying properties and fields.
	 */
	public static class pb_Reflection
	{
		/**
		 * Return a PropertyInfo collection where each entry is checked against IsSpecialName and HasIgnoredAttribute.
		 */
		public static IEnumerable<PropertyInfo> GetSerializableProperties(Type type, BindingFlags flags)
		{
			// check that:
			// 	- setter exists
			// 	- flags don't care about public, or they do but setter is public anyways
			// 	- it's not special
			//	- it's not tagged as obsolete or ignore 
			return type.GetProperties(flags).Where(
				x => x.CanWrite &&
				( (flags & BindingFlags.Public) == 0 || x.GetSetMethod() != null ) &&
				!x.IsSpecialName &&
				!HasIgnoredAttribute(x) );
		}

		/**
		 * Return a FieldInfo collection where each entry is checked against IsSpecialName and HasIgnoredAttribute.
		 */
		public static IEnumerable<FieldInfo> GetSerializableFields(Type type, BindingFlags flags)
		{
			return type.GetFields(flags).Where(x => 
				( (flags & BindingFlags.Public) == 0 || !x.IsPrivate ) &&
				!HasIgnoredAttribute(x) );
		}

		/**
		 * Iterate all public instance fields on an object and store them in a dictionary
		 * with property/field name as the key, and value as value.
		 */
		public static Dictionary<string, object> ReflectProperties<T>(T obj)
		{
			return ReflectProperties<T>(obj, BindingFlags.Instance | BindingFlags.Public, null);
		}

		public static Dictionary<string, object> ReflectProperties<T>(T obj, HashSet<string> ignoreFields)
		{
			return ReflectProperties<T>(obj, BindingFlags.Instance | BindingFlags.Public, ignoreFields);
		}

		public static Dictionary<string, object> ReflectProperties<T>(T obj, BindingFlags flags, HashSet<string> ignoreFields)
		{
#if REFLECTION_DEBUG
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.AppendLine("Component type(" + obj.GetType().ToString() + ")");
#endif

			Dictionary<string, object> properties = new Dictionary<string, object>();
			
			Type type = obj.GetType();

			foreach(PropertyInfo prop in pb_Reflection.GetSerializableProperties(type, flags))
			{
				try
				{
					bool willWrite = 	prop.CanWrite &&
										!prop.IsSpecialName &&
										!HasIgnoredAttribute(prop) &&
										!(ignoreFields != null && ignoreFields.Contains(prop.Name));

#if REFLECTION_DEBUG
					sb.AppendLine((willWrite ? "p - " : "px- ") + prop.Name + " : " + prop.GetValue(obj, null));
#endif

					if(willWrite)
					{
						ParameterInfo[] parms = prop.GetIndexParameters();

						if(parms != null && parms.Length > 0)
						{
							// Debug.LogWarning("ParameterInfo[] > 0 : " + prop.Name);
						}
						else
						{
							properties.Add(prop.Name, prop.GetValue(obj, parms));
						}
					}
				}
				catch {}
			}
			
			foreach(FieldInfo field in type.GetFields(flags) )
			{
				try
				{
#if REFLECTION_DEBUG
					sb.AppendLine((HasIgnoredAttribute(field) ? "fx - " : "f  - ") + field.Name + " : " + field.GetValue(obj));
#endif

					if( HasIgnoredAttribute(field) ||
						(ignoreFields != null && ignoreFields.Contains(field.Name)))
						continue;

					properties.Add(field.Name, field.GetValue(obj));
				}
				catch {
					Debug.LogError("Failed extracting property: " + field.Name);
				}
			}

#if REFLECTION_DEBUG
			Debug.Log(sb.ToString());
#endif

			return properties;
		}

		/**
		 * Get the value for a property or field from an object with it's name.
		 */
		public static T GetValue<T>(object obj, string name)
		{
			return GetValue<T>(obj, name, BindingFlags.Instance | BindingFlags.Public);
		}

		/**
		 * Get the value for a property or field from an object with it's name.
		 */
		public static T GetValue<T>(object obj, string name, BindingFlags flags)
		{
			if(obj == null)
				return default(T);

			PropertyInfo prop = obj.GetType().GetProperty(name, flags);

			if(prop != null)
				return (T) prop.GetValue(obj, null);

			FieldInfo field = obj.GetType().GetField(name, flags);

			if(field != null)
				return (T) field.GetValue(obj);

			return default(T);
		}

		public static bool SetValue(object obj, string name, object value)
		{
			return SetValue(obj, name, value, BindingFlags.Instance | BindingFlags.Public);
		}

		public static bool SetValue(object obj, string name, object value, BindingFlags flags)
		{
			if(obj == null)
				return false;

			PropertyInfo prop = obj.GetType().GetProperty(name, flags);

			if(prop != null)
				return SetPropertyValue(obj, prop, value);

			FieldInfo field = obj.GetType().GetField(name, flags);

			if(field != null)
				return SetFieldValue(obj, field, value);

			return false;
		}

		/**
		 * Attempt to set a value with PropertyInfo and target object.  Handles converting from object to actual type, and covers
		 * some serialization "gotchas" when coming from JSON.
		 * SetValue and it's overrides end up calling this or SetFieldValue.
		 */
		public static bool SetPropertyValue(object target, PropertyInfo propertyInfo, object value)
		{
			if(propertyInfo == null || target == null)
				return false;

			try
			{
				var val = value is JToken ? ((JToken)value).ToObject(propertyInfo.PropertyType, pb_Serialization.Serializer) : value;

				if(propertyInfo.PropertyType.IsEnum)
				{
					int conv = (int) Convert.ChangeType(val, typeof(int));
					propertyInfo.SetValue(target, conv, null);
				}
				else
				if(val is pb_ObjectWrapper)
				{
					var actual = ((pb_ObjectWrapper)val).GetValue();
					var conv = Convert.ChangeType(actual, propertyInfo.PropertyType);
					propertyInfo.SetValue(target, conv, null);
				}
				else
				{
					var conv = Convert.ChangeType(val, propertyInfo.PropertyType);
					propertyInfo.SetValue(target, conv, null);
				}

				return true;
			}
#if PB_DEBUG
			catch(System.Exception e)
			{
				Debug.LogWarning(e.ToString());
				return false;
			}
#else
			catch
			{
				return false;
			}
#endif
		}

		/**
		 * Attempt to set a value with Field and target object.  Handles converting from object to actual type, and covers
		 * some serialization "gotchas" when coming from JSON.
		 */
		public static bool SetFieldValue(object target, FieldInfo field, object value)
		{
			if(target == null || field == null)
				return false;

			try
			{
				var val = value is JToken ? ((JToken)value).ToObject(field.FieldType, pb_Serialization.Serializer) : value;

				if(field.FieldType.IsEnum)
				{
					int conv = (int) Convert.ChangeType(val, typeof(int));
					field.SetValue(target, conv);
				}
				else
				if(val is pb_ObjectWrapper)
				{
					var actual = ((pb_ObjectWrapper)val).GetValue();
					var conv = Convert.ChangeType(actual, field.FieldType);
					field.SetValue(target, conv);
				}
				else
				{
					var conv = Convert.ChangeType(val, field.FieldType);
					field.SetValue(target, conv);
				}

				return true;
			}
			catch(System.Exception e)
			{
				Debug.LogError(e.ToString());
				return false;
			}
		}

		/**
		 * Iterate through an object instance applying matching property keys to the object's fields.
		 */
		public static void ApplyProperties<T>(T obj, Dictionary<string, object> properties)
		{
			foreach(KeyValuePair<string, object> kvp in properties)
			{
				SetValue(obj, kvp.Key, kvp.Value);
			}
		}

		public static readonly HashSet<System.Type> IgnoreAttributes = new HashSet<System.Type>
		{
			typeof(System.ObsoleteAttribute),
			typeof(pb_JsonIgnoreAttribute)
		};

		/**
		 * Returns true if reflected info finds any deprecated or obsolete attributes.
		 */
		public static bool HasIgnoredAttribute(Type type)
		{
			return type.GetCustomAttributes(true).Any(x => IgnoreAttributes.Contains(x.GetType()));
		}

		public static bool HasIgnoredAttribute<T>(T info) where T : MemberInfo
		{
			return info.GetCustomAttributes(true).Any(x => IgnoreAttributes.Contains(x.GetType()));			
		}

		public static IEnumerable<Attribute> GetAttributes<T>(T obj)
		{
			return (IEnumerable<Attribute>) typeof(T).GetCustomAttributes(true);
		}
	}
}