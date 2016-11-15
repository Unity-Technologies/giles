// using System.Reflection;
// using UnityEngine;

// namespace GILES
// {
// 	/**
// 	 * An object representing either a property or field reflected from an object.  Essentially a union { PropertyInfo, FieldInfo }
// 	 * with get/set for both depending on the enclosed type, with a strong type.
// 	 */
// 	public class pb_ReflectedInfo
// 	{
// 		public PropertyInfo property;
// 		public FieldInfo field;

// 		public pb_ReflectedValue(PropertyInfo property)
// 		{
// 			this.propertyInfo = property;
// 			this.fieldInfo = null;
// 		}

// 		public pb_ReflectedValue(FieldInfo field)
// 		{
// 			this.propertyInfo = null;
// 			this.fieldInfo = field;
// 		}

// 		public object GetValue()
// 		{
// 			if(property != null)
// 				return property.GetValue(target, null);
// 			else
// 				return field.GetValue(target);
// 		}

// 		public void SetValue(object value)
// 		{
// 		}
// 	}
// }