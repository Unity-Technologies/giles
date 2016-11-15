using UnityEngine;

namespace GILES
{
	/**
	 * A series of helper methods for working with collections.
	 */
	public static class pb_CollectionUtil
	{
		/**
		 * Return an array filled with `value` and of length.
		 */
		public static T[] Fill<T>(T value, int length)
		{
			T[] arr = new T[length];
			for(int i = 0; i < length; i++)
				arr[i] = value;
			return arr;
		}
		
	}
}