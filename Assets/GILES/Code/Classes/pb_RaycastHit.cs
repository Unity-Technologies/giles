using UnityEngine;

namespace GILES
{
	/**
	 * Describes the intersection point of a ray and mesh.
	 */
	public class pb_RaycastHit
	{
		public Vector3 point;
		public float distance;
		public Vector3 normal;
		public int[] triangle;
	}
}