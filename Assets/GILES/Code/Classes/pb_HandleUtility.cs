using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GILES
{

	/**
	 * Collection of static methods commonly used when working with scene handles.
	 */
	public class pb_HandleUtility
	{

		/**
		 * Returns the nearest point on each line to the other line.
		 *
		 * http://wiki.unity3d.com/index.php?title=3d_Math_functions
		 * Two non-parallel lines which may or may not touch each other have a point on each line which are closest
		 * to each other. This function finds those two points. If the lines are not parallel, the function
		 * outputs true, otherwise false.
		 */
		public static bool ClosestPointsOnTwoLines(Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2, out Vector3 closestPointLine1, out Vector3 closestPointLine2)
		{
			closestPointLine1 = Vector3.zero;
			closestPointLine2 = Vector3.zero;

			float a = Vector3.Dot(lineVec1, lineVec1);
			float b = Vector3.Dot(lineVec1, lineVec2);
			float e = Vector3.Dot(lineVec2, lineVec2);

			float d = a*e - b*b;

			//lines are not parallel
			if(d != 0.0f){

				Vector3 r = linePoint1 - linePoint2;
				float c = Vector3.Dot(lineVec1, r);
				float f = Vector3.Dot(lineVec2, r);

				float s = (b*f - c*e) / d;
				float t = (a*f - c*b) / d;

				closestPointLine1 = linePoint1 + lineVec1 * s;
				closestPointLine2 = linePoint2 + lineVec2 * t;

				return true;
			}
			else
			{
				return false;
			}
		}

		/**
		 * Return the nearest point on each ray to the other ray.
		 * \sa ClosestPointOnTwoLines
		 */
		public static bool PointOnLine(Ray InLineA, Ray InLineB, out Vector3 OutPointA, out Vector3 OutPointB)
		{
			return ClosestPointsOnTwoLines(InLineA.origin, InLineA.direction, InLineB.origin, InLineB.direction, out OutPointA, out OutPointB);
		}

		/**
		 * Return the point on a plane where a ray intersects.
		 */
		public static bool PointOnPlane(Ray ray, Vector3 planePosition, Vector3 planeNormal, out Vector3 hit)
		{
			return PointOnPlane(ray, new Plane(planeNormal, planePosition), out hit);
		}

		/**
		 * Return the point on a plane where a ray intersects.
		 */
		public static bool PointOnPlane(Ray ray, Plane plane, out Vector3 hit)
		{
			float distance;

			if(plane.Raycast(ray, out distance))
			{
				hit = ray.GetPoint(distance);
				return true;
			}
			else
			{
				hit = Vector3.zero;
				return false;
			}
		}

		private static Vector3 Mask(Vector3 vec)
		{
			return new Vector3(	vec.x > 0f ? 1f : -1f,
								vec.y > 0f ? 1f : -1f,
								vec.z > 0f ? 1f : -1f);
		}

		private static float Mask(float val)
		{
			return val > 0f ? 1f : -1f;
		}

		public static Vector3 DirectionMask(Transform target, Vector3 rayDirection)
		{
			Vector3 viewDir = -Mask(new Vector3(Vector3.Dot(rayDirection, target.right),
												Vector3.Dot(rayDirection, target.up),
												Vector3.Dot(rayDirection, target.forward)));
			return viewDir;
		}

		/**
		 * When dragging in 3d space, this returns the signed delta based on handle orientation.
		 */
		public static float CalcMouseDeltaSignWithAxes(Camera cam, Vector3 origin, Vector3 upDir, Vector3 rightDir, Vector2 mouseDelta)
		{
			if( Mathf.Abs(mouseDelta.magnitude) < .0001f)
				return 1f;

			Vector2 or = cam.WorldToScreenPoint(origin);
			Vector2 ud = cam.WorldToScreenPoint(origin + upDir);
			Vector2 rd = cam.WorldToScreenPoint(origin + rightDir);

			float mouseDotUp = Vector2.Dot(mouseDelta, ud - or);
			float mouseDotRight = Vector2.Dot(mouseDelta, rd - or);

			if( Mathf.Abs(mouseDotUp) > Mathf.Abs(mouseDotRight))
				return Mathf.Sign(mouseDotUp);
			else
				return Mathf.Sign(mouseDotRight);
		}

		/**
		 * Calculates a signed float delta from a current and previous mouse position.
		 * @param lhs Current mouse position.
		 * @param rhs Previous mouse position.
		 */
		public static float CalcSignedMouseDelta(Vector2 lhs, Vector2 rhs)
		{
			float delta = Vector2.Distance(lhs, rhs);
			float scale = 1f / Mathf.Min(Screen.width, Screen.height);

			// If horizontal movement is greater than vertical movement, use the X axis for sign.
			if( Mathf.Abs(lhs.x - rhs.x) > Mathf.Abs(lhs.y - rhs.y) )
				return delta * scale * ( (lhs.x-rhs.x) > 0f ? 1f : -1f );
			else
				return delta * scale * ( (lhs.y-rhs.y) > 0f ? 1f : -1f );
		}

		/**
		 * Return the screen to world space ratio for this point in world space.
		 */
		public static float GetHandleSize(Vector3 position)
		{
			Camera cam = Camera.main;
			if(!cam) return 1f;
			Transform t = cam.transform;
			float z = Vector3.Dot(position-t.position, cam.transform.forward);
			Vector3 lhs = cam.WorldToScreenPoint(t.position + (t.forward * z));
			Vector3 rhs = cam.WorldToScreenPoint(t.position + (t.right + t.forward * z));
			return 1f/(lhs-rhs).magnitude;
		}

		/**
		 * Transform world ray into model space.
		 */
		public static Ray TransformRay(Ray ray, Transform transform)
		{
			Matrix4x4 m = transform.worldToLocalMatrix;
			Ray local = new Ray(m.MultiplyPoint(ray.origin), m.MultiplyVector(ray.direction));

			return local;
		}

		/**
		 * Return the nearest hit object in scene.  Does not require a collider.
		 */
		public static GameObject ObjectRaycast(Ray ray, IEnumerable<GameObject> objects)
		{
			Renderer renderer;
			float distance = Mathf.Infinity;
			float best = Mathf.Infinity;
			GameObject obj = null;
			Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

			pb_RaycastHit hit;

			foreach(GameObject go in objects)
			{
				Ray localRay = TransformRay(ray, go.transform);

				renderer = go.GetComponent<Renderer>();

				if( renderer != null )
				{
					if( renderer.bounds.IntersectRay(ray, out distance) )
					{
						MeshFilter mf = go.GetComponent<MeshFilter>();

						if( mf != null && mf.sharedMesh != null && MeshRaycast(mf.sharedMesh, localRay, out hit))
						{
							if(hit.distance < best)
							{
								best = hit.distance;
								obj = go;
							}
						}
					}
				}
				else
				{
					bounds.center = go.transform.position;

					if(bounds.IntersectRay(ray, out distance))
					{
						if( distance < best )
						{
							best = distance;
							obj = go;
						}
					}
				}
			}

			return obj;
		}

		public static bool MeshRaycast(Mesh mesh, Ray ray, out pb_RaycastHit hit)
		{
			Vector3[] vertices = mesh.vertices;
			int[] triangles = mesh.triangles;

			float dist = Mathf.Infinity;
			Vector3 point = Vector3.zero;
			Vector3 a, b, c;

			for(int i = 0; i < triangles.Length; i += 3)
			{
				a = vertices[triangles[i+0]];
				b = vertices[triangles[i+1]];
				c = vertices[triangles[i+2]];

				if(pb_Geometry.RayIntersectsTriangle(ray, a, b, c, Culling.Front, out dist, out point))
				{
					hit = new pb_RaycastHit();
					hit.point = point;
					hit.distance = Vector3.Distance(hit.point, ray.origin);
					hit.normal = Vector3.Cross(b-a, c-a);
					hit.triangle = new int[] { triangles[i], triangles[i+1], triangles[i+2] };
					return true;
				}
			}

			hit = null;
			return false;
		}

		/**
		 * Return the signed distance from a mouse position to a line in screen space.
		 */
		public static float DistancePoint2DToLine(Camera cam, Vector2 mousePosition, Vector3 worldPosition1, Vector3 worldPosition2)
		{
			Vector2 v0 = cam.WorldToScreenPoint(worldPosition1);
			Vector2 v1 = cam.WorldToScreenPoint(worldPosition2);

			return DistancePointLineSegment(mousePosition, v0, v1);
		}

		/**
		 * 	Get the distance between a point and a finite line segment.
		 * http://stackoverflow.com/questions/849211/shortest-distance-between-a-point-and-a-line-segment
		 */
		public static float DistancePointLineSegment(Vector2 p, Vector2 v, Vector2 w)
		{
			// lineStart = v
			// lineEnd = w
			// point = p

			// Return minimum distance between line segment vw and point p
			float l2 = ((v.x - w.x)*(v.x - w.x)) + ((v.y - w.y)*(v.y - w.y));  // i.e. |w-v|^2 -  avoid a sqrt

			if (l2 == 0.0f) return Vector2.Distance(p, v);   // v == w case

			// Consider the line extending the segment, parameterized as v + t (w - v).
			// We find projection of point p onto the line.
			// It falls where t = [(p-v) . (w-v)] / |w-v|^2
			float t = Vector2.Dot(p - v, w - v) / l2;

			if (t < 0.0)
				return Vector2.Distance(p, v);       		// Beyond the 'v' end of the segment
			else if (t > 1.0)
				return Vector2.Distance(p, w);  			// Beyond the 'w' end of the segment

			Vector2 projection = v + t * (w - v);  	// Projection falls on the segment

			return Vector2.Distance(p, projection);
		}

		/**
		 * Returns true if the polygon contains point.  False otherwise.
		 * Casts a ray from outside the bounds to the polygon and checks how
		 * many edges are hit.
		 * @param polygon A series of individual edges composing a polygon.  polygon length *must* be divisible by 2.
		 */
		public static bool PointInPolygon(Vector2[] polygon, Vector2 point)
		{
			float xmin = Mathf.Infinity, xmax = -Mathf.Infinity, ymin = Mathf.Infinity, ymax = -Mathf.Infinity;

			for(int i = 0; i < polygon.Length; i++)
			{
				if(polygon[i].x < xmin)
					xmin = polygon[i].x;
				else if(polygon[i].x > xmax)
					xmax = polygon[i].x;

				if(polygon[i].y < ymin)
					ymin = polygon[i].y;
				else if(polygon[i].y > ymax)
					ymax = polygon[i].y;
			}

			if(point.x < xmin || point.x > xmax || point.y < ymin || point.y > ymax)
				return false;

			Vector2 rayStart = new Vector2(xmin - 1f, ymax + 1f);

			int collisions = 0;

			for(int i = 0; i < polygon.Length; i += 2)
			{
				if( GetLineSegmentIntersect(rayStart, point, polygon[i], polygon[i+1]) )
					collisions++;
			}

			return collisions % 2 != 0;
		}

		/**
		 * True or false lines intersect.
		 */
		public static bool GetLineSegmentIntersect(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
		{
			Vector2 s1, s2;
			s1.x = p1.x - p0.x;     s1.y = p1.y - p0.y;
			s2.x = p3.x - p2.x;     s2.y = p3.y - p2.y;

			float s, t;
			s = (-s1.y * (p0.x - p2.x) + s1.x * (p0.y - p2.y)) / (-s2.x * s1.y + s1.x * s2.y);
			t = ( s2.x * (p0.y - p2.y) - s2.y * (p0.x - p2.x)) / (-s2.x * s1.y + s1.x * s2.y);

			return (s >= 0 && s <= 1 && t >= 0 && t <= 1);
		}
	}
}
