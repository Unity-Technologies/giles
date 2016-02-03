using UnityEngine;
using System.Collections;

namespace GILES
{
	/**
	 * Static helper methods for rounding values.
	 */
	public static class pb_Snap
	{
		/**
		* Snap a value to the nearest increment.
		*/
		public static float Snap(float value, float increment)
		{
			return Mathf.Round(value / increment) * increment;
		}

		public static Vector2 Snap(Vector2 value, float increment)
		{
			return new Vector2(
				Snap(value.x, increment),  
				Snap(value.y, increment) );
		}

		public static Vector3 Snap(Vector3 value, float increment)
		{
			return new Vector3(
				Snap(value.x, increment), 
				Snap(value.y, increment), 
				Snap(value.z, increment) );
		}
	}
}