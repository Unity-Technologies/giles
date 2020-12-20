using UnityEngine;

namespace GILES
{
	/**
	 * A series of helper methods for working with collections.
	 */
	public static class pb_CollectionUtil
	{
		
		///<summary>
		///Return an array filled with `value` and of length.
		///</summary>
		/// <returns>
		/// Returns an array of T
		/// </returns>
		/// <param name="T">The type.</param>
    		/// <param name="length">The lenght of the array.</param>
		public static T[] Fill<T>(T value, int length)
		{
			T[] arr = new T[length];
			for(int i = 0; i < length; i++)
				arr[i] = value;
			return arr;
		}
		
	}
}
