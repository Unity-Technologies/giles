using UnityEngine;
using System;

namespace GILES.Serialization
{
	/**
	 * Classes marked with this attribute will be ignored when serializing a level.
	 */
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	public class pb_JsonIgnoreAttribute : Attribute {}
}