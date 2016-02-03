using UnityEngine;

namespace GILES
{
	/**
	 * Utility methods for working with UnityEngine.Transform and pb_Transform types.
	 */
	public static class pb_TransformExtension
	{
		/**
		 * Set a UnityEngine.Transform with a Runtime.pb_Transform.
		 */
		public static void SetTRS(this Transform transform, pb_Transform pbTransform)
		{
			transform.position = pbTransform.position;
			transform.localRotation = pbTransform.rotation;
			transform.localScale = pbTransform.scale;
		}
	}
}