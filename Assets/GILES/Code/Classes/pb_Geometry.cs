using UnityEngine;

namespace GILES
{
	/**
	 * Static class with commonly used geometry functions.
	 */
	public static class pb_Geometry
	{
		/**
		 * Returns true if a raycast intersects a triangle.
		 * http://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm
		 * http://www.cs.virginia.edu/~gfx/Courses/2003/ImageSynthesis/papers/Acceleration/Fast%20MinimumStorage%20RayTriangle%20Intersection.pdf
		 */
		public static bool RayIntersectsTriangle(
			Ray InRay, 
			Vector3 InTriangleA,
			Vector3 InTriangleB,
			Vector3 InTriangleC,
			Culling cull,
			out float OutDistance,
			out Vector3 OutPoint)
		{
			OutDistance = 0f;
			OutPoint = Vector3.zero;
			
			Vector3 e1, e2;  //Edge1, Edge2
			Vector3 P, Q, T;
			float det, inv_det, u, v;
			float t;

			//Find vectors for two edges sharing V1
			e1 = InTriangleB - InTriangleA;
			e2 = InTriangleC - InTriangleA;

			//Begin calculating determinant - also used to calculate `u` parameter
			P = Vector3.Cross(InRay.direction, e2);
			
			//if determinant is near zero, ray lies in plane of triangle
			det = Vector3.Dot(e1, P);

			// NON-CULLING
			if( (cull == Culling.Front && det < Mathf.Epsilon) || (det > -Mathf.Epsilon && det < Mathf.Epsilon) )
				return false;

			inv_det = 1f / det;

			//calculate distance from V1 to ray origin
			T = InRay.origin - InTriangleA;

			// Calculate u parameter and test bound
			u = Vector3.Dot(T, P) * inv_det;

			//The intersection lies outside of the triangle
			if(u < 0f || u > 1f)
				return false;

			//Prepare to test v parameter
			Q = Vector3.Cross(T, e1);

			//Calculate V parameter and test bound
			v = Vector3.Dot(InRay.direction, Q) * inv_det;

			//The intersection lies outside of the triangle
			if(v < 0f || u + v  > 1f)
				return false;

			t = Vector3.Dot(e2, Q) * inv_det;

			if(t > Mathf.Epsilon)
			{ 
				//ray intersection
				OutDistance = t;

				OutPoint.x = (u * InTriangleB.x + v * InTriangleC.x + (1-(u+v)) * InTriangleA.x);
				OutPoint.y = (u * InTriangleB.y + v * InTriangleC.y + (1-(u+v)) * InTriangleA.y);
				OutPoint.z = (u * InTriangleB.z + v * InTriangleC.z + (1-(u+v)) * InTriangleA.z);

				return true;
			}

			return false;
		}
	}
}