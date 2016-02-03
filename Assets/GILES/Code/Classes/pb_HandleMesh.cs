using UnityEngine;
using System.Collections.Generic;

namespace GILES
{
	/**
	 * Static methods to build meshes necessary for the handle gizmos.
	 */
	public static class pb_HandleMesh
	{
		static readonly Color red = new Color(.85f, .256f, .16f, 0f);
		static readonly Color green = new Color(.2f, .9f, .2f, 0f);
		static readonly Color blue = new Color(.26f, .56f, .85f, 0f);

		/**
		 * Create the line mesh for position and scale gizmos.
		 */
		public static Mesh CreatePositionLineMesh(ref Mesh mesh, Transform transform, Vector3 scale, Camera cam, float handleSize)
		{
			Vector3 viewDir = pb_HandleUtility.DirectionMask(transform, cam.transform.forward);

			List<Vector3> v = new List<Vector3>();
			List<Vector3> n = new List<Vector3>();
			List<Vector2> u = new List<Vector2>();
			List<Color> c = new List<Color>();
			List<int> t = new List<int>();

			// Right, up, forward lines
			v.AddRange( new Vector3[] {
				// Right Axis
				Vector3.zero,
				Vector3.right * scale.x,

				// Z Plane
				new Vector3( viewDir.x * 0f, viewDir.y * handleSize, 0f),
				new Vector3( viewDir.x * handleSize, viewDir.y * handleSize, 0f),
				new Vector3( viewDir.x * handleSize, viewDir.y * handleSize, 0f),
				new Vector3( viewDir.x * handleSize, viewDir.y * 0f, 0f),
				new Vector3( viewDir.x * handleSize, viewDir.y * 0f, 0f),
				Vector3.zero,

				// Up Axis
				Vector3.zero,
				Vector3.up * scale.y,

				new Vector3( viewDir.x * handleSize, 0f,  viewDir.z * 0f),
				new Vector3( viewDir.x * handleSize, 0f,  viewDir.z * handleSize),
				new Vector3( viewDir.x * handleSize, 0f,  viewDir.z * handleSize),
				new Vector3( viewDir.x * 0f, 0f, viewDir.z * handleSize),

				// Forward Axis
				Vector3.zero,
				Vector3.forward * scale.z,

				// X Plane
				Vector3.zero,
				new Vector3(0f, viewDir.y * 0f, viewDir.z * handleSize),
				new Vector3(0f, viewDir.y * 0f, viewDir.z * handleSize),
				new Vector3(0f, viewDir.y * handleSize, viewDir.z * handleSize),
				new Vector3(0f, viewDir.y * handleSize, viewDir.z * handleSize),
				new Vector3(0f, viewDir.y * handleSize, viewDir.z * 0f),
				new Vector3(0f, viewDir.y * handleSize, viewDir.z * 0f),
				Vector3.zero
				});

			c.AddRange( new Color[] {
				red,
				red,
				blue,
				blue,
				blue,
				blue,
				blue,
				blue,
				green,
				green,
				green,
				green,
				green,
				green,
				blue,
				blue,
				red,
				red,
				red,
				red,
				red,
				red,
				red,
				red
				});

			for(int i = 0; i < v.Count; i++)
			{
				n.Add(Vector3.up);
				u.Add(Vector2.zero);
				t.Add(i);
			}

			mesh.Clear();
			mesh.name = "pb_Handle_TRANSLATE";
			mesh.vertices = v.ToArray();
			mesh.uv = u.ToArray();
			mesh.subMeshCount = 1;
			mesh.SetIndices(t.ToArray(), MeshTopology.Lines, 0);
			mesh.colors = c.ToArray();
			mesh.normals = n.ToArray();

			return mesh;
		}

		/**
		 * Returns the vertices used to build a rotate mesh in a jagged array (y, z, x).
		 */
		public static Vector3[][] GetRotationVertices(int segments, float radius)
		{
			Vector3[] v_x = new Vector3[segments];
			Vector3[] v_y = new Vector3[segments];
			Vector3[] v_z = new Vector3[segments];

			for(int i = 0; i < segments; i++)
			{
				float rad = (i/(float)(segments-1)) * 360f * Mathf.Deg2Rad;

				v_x[i].x = Mathf.Cos( rad ) * radius;
				v_x[i].z = Mathf.Sin( rad ) * radius;
				v_y[i].x = Mathf.Cos( rad ) * radius;
				v_y[i].y = Mathf.Sin( rad ) * radius;
				v_z[i].z = Mathf.Cos( rad ) * radius;
				v_z[i].y = Mathf.Sin( rad ) * radius;
			}

			return new Vector3[3][] { v_x, v_y, v_z };
		}

		/**
		 * Create the line mesh used for rotate gizmo.
		 */
		public static Mesh CreateRotateMesh(ref Mesh mesh, int segments, float radius)
		{
			Vector3[][] verts = GetRotationVertices(segments, radius);

			Vector3[] ver 	= new Vector3[segments * 3];
			Vector3[] nrm 	= new Vector3[segments * 3];
			Color[] col		= new Color[segments * 3];
			int[] triA 		= new int[segments];
			int[] triB 		= new int[segments];
			int[] triC 		= new int[segments];

			int a = 0, b = segments, c = segments + segments;

			for(int i = 0; i < segments; i++)
			{
				a = i;
				b = i + segments;
				c = i + segments + segments;

				ver[a] = verts[0][i];
				col[a] = green;
				nrm[a].x = ver[a].x;
				nrm[a].z = ver[a].z;

				ver[b] = verts[1][i];
				col[b] = blue;
				nrm[b].x = ver[b].x;
				nrm[b].y = ver[b].y;

				ver[c] = verts[2][i];
				col[c] = red;
				nrm[c].z = ver[c].z;
				nrm[c].y = ver[c].y;

				triA[i] = a;
				triB[i] = b;
				triC[i] = c;
			}

			mesh.Clear();
			mesh.name = "pb_Handle_ROTATE";
			mesh.vertices = ver;
			mesh.uv = new Vector2[segments * 3];
			mesh.subMeshCount = 3;
			mesh.SetIndices(triA, MeshTopology.LineStrip, 0);
			mesh.SetIndices(triB, MeshTopology.LineStrip, 1);
			mesh.SetIndices(triC, MeshTopology.LineStrip, 2);
			mesh.colors = col;
			mesh.normals = nrm;

			return mesh;
		}

		/**
		 * Create a line mesh disc.
		 */
		public static Mesh CreateDiscMesh(ref Mesh mesh, int segments, float radius)
		{
			Vector3[] v = new Vector3[segments];
			Vector3[] n = new Vector3[segments];
			Vector2[] u = new Vector2[segments];
			int[] 	  t = new int[segments];

			for(int i = 0; i < segments; i++)
			{
				float rad = (i / (float) (segments - 1)) * 360f * Mathf.Deg2Rad;
				v[i] = new Vector3(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius, 0f);
				n[i] = v[i];
				u[i] = Vector2.zero;
				t[i] = i < segments - 1 ? i : 0;
			}

			mesh.Clear();
			mesh.name = "Disc";
			mesh.vertices = v;
			mesh.uv = u;
			mesh.normals = n;
			mesh.subMeshCount = 1;
			mesh.SetIndices(t, MeshTopology.LineStrip, 0);

			return mesh;
		}

		/**
		 * Create the little planar box and end caps using `cap`.
		 */
		public static Mesh CreateTriangleMesh(ref Mesh mesh, Transform transform, Vector3 scale, Camera cam, Mesh cap, float handleSize, float capSize)
		{		
			Vector3 viewDir = pb_HandleUtility.DirectionMask(transform, cam.transform.forward);

			List<Vector3> v = new List<Vector3>()
			{
				// X Axis
				new Vector3(0f, viewDir.y * 0f, viewDir.z * 0f),
				new Vector3(0f, viewDir.y * 0f, viewDir.z * handleSize),
				new Vector3(0f, viewDir.y * handleSize, viewDir.z * handleSize),
				new Vector3(0f,	viewDir.y * handleSize, viewDir.z * 0f),

				new Vector3(0f, 0f, 0f),
				new Vector3(viewDir.x * handleSize, 0f, 0f),
				new Vector3(viewDir.x * handleSize, viewDir.y * handleSize, 0f),
				new Vector3(0f, viewDir.y * handleSize, 0f),

				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, handleSize * viewDir.z),
				new Vector3(handleSize * viewDir.x, 0f, handleSize * viewDir.z),
				new Vector3(handleSize * viewDir.x, 0f, 0f),
			};

			List<Vector3> nrm = new List<Vector3>()
			{
				Vector3.right,
				Vector3.right,
				Vector3.right,
				Vector3.right,

				Vector3.forward,
				Vector3.forward,
				Vector3.forward,
				Vector3.forward,

				Vector3.up,
				Vector3.up,
				Vector3.up,
				Vector3.up
			};

			Color plane_red = red, plane_blue = blue, plane_green = green;
			plane_red.a = .25f;
			plane_green.a = .25f;
			plane_blue.a = .25f;

			List<Color> c = new List<Color>()
			{
				plane_red,
				plane_red,
				plane_red,
				plane_red,

				plane_blue,
				plane_blue,
				plane_blue,
				plane_blue,

				plane_green,
				plane_green,
				plane_green,
				plane_green
			};

			List<Vector2> u = new List<Vector2>();
			List<int> t = new List<int>();

			for(int n = 0; n < v.Count; n+=4)
			{	
				u.Add(Vector2.zero);
				u.Add(Vector2.zero);
				u.Add(Vector2.zero);
				u.Add(Vector2.zero);

				t.Add(n+0);
				t.Add(n+1);
				t.Add(n+3);
				t.Add(n+1);
				t.Add(n+2);
				t.Add(n+3);
			}

			// Now generate caps
			Vector3[] cv, cn;
			Vector2[] cu;
			Color[] cc;
			int[] ct;

			Matrix4x4 _coneRightMatrix 		= Matrix4x4.TRS(Vector3.right * scale.x, Quaternion.Euler(90f, 90f, 0f), Vector3.one * capSize);
			Matrix4x4 _coneUpMatrix 		= Matrix4x4.TRS(Vector3.up * scale.y, Quaternion.identity, Vector3.one * capSize);
			Matrix4x4 _coneForwardMatrix 	= Matrix4x4.TRS(Vector3.forward * scale.z, Quaternion.Euler(90f, 0f, 0f), Vector3.one * capSize);

			List<int> t2 = new List<int>();

			TransformMesh(cap, _coneRightMatrix, out cv, out cn, out cu, out cc, out ct, v.Count, red);
			v.AddRange(cv);
			c.AddRange(cc);
			nrm.AddRange(cn);
			u.AddRange(cu);
			t2.AddRange(ct);

			TransformMesh(cap, _coneUpMatrix, out cv, out cn, out cu, out cc, out ct, v.Count,green);
			v.AddRange(cv);
			c.AddRange(cc);
			nrm.AddRange(cn);
			u.AddRange(cu);
			t2.AddRange(ct);

			TransformMesh(cap, _coneForwardMatrix, out cv, out cn, out cu, out cc, out ct, v.Count,blue);
			v.AddRange(cv);
			c.AddRange(cc);
			nrm.AddRange(cn);
			u.AddRange(cu);
			t2.AddRange(ct);

			mesh.Clear();
			mesh.subMeshCount = 2;
			mesh.vertices = v.ToArray();
			mesh.normals = nrm.ToArray();
			mesh.uv = u.ToArray();
			mesh.SetTriangles(t.ToArray(), 0);
			mesh.SetTriangles(t2.ToArray(), 1);
			mesh.colors = c.ToArray();
			return mesh;
		}

		public static void TransformMesh(Mesh mesh, Matrix4x4 matrix, out Vector3[] v, out Vector3[] n, out Vector2[] u, out Color[] c, out int[] t, int indexOffset, Color color)
		{
			v = mesh.vertices;
			n = mesh.normals;
			u = mesh.uv;
			c = mesh.colors;
			t = mesh.triangles;

			color.a = 1f;

			for(int i = 0; i < v.Length; i++)
			{
				v[i] = (matrix * v[i]) + matrix.GetColumn(3);	// have to add the translation, since the model is at zer0 origin.
				n[i] = matrix * n[i];
				c[i] = color;
			}

			for(int i = 0; i < t.Length; i++)
				t[i] += indexOffset;
		}

	}
}