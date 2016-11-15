using UnityEngine;
using System;

namespace GILES.Interface
{
	/**
	 * Classes or members marked with [pb_InspectorIgnore] will not have their serialized values shown in the Inspector window.
	 */
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class pb_InspectorIgnoreAttribute : Attribute
	{

	}
}