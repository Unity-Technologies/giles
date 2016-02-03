using UnityEngine;
using System.Collections;

namespace GILES
{
	/**
	 * Draw a few arrows pointing in the direction that this light is facing.
	 */
	[pb_Gizmo(typeof(Light))]
	public class pb_Gizmo_Light : pb_Gizmo
	{
		public Material lightRayMaterial;
		private Light lightComponent;
		private Mesh _lightMesh;

		private readonly Color yellow = new Color(1f, 1f, 0f, .5f);

		Matrix4x4 gizmoMatrix = Matrix4x4.identity;

		private Mesh lightMesh
		{
			get
			{
				if(_lightMesh == null && lightComponent != null)
				{
					switch(lightComponent.type)
					{
						case LightType.Directional:
							_lightMesh = DirectionalLightMesh();
							lightRayMaterial = pb_BuiltinResource.GetMaterial(pb_BuiltinResource.mat_UnlitVertexColorWavy);
							break;

						case LightType.Spot:
							_lightMesh = SpotLightMesh();
							lightRayMaterial = pb_BuiltinResource.GetMaterial(pb_BuiltinResource.mat_UnlitVertexColor);
							break;

						case LightType.Point:
							_lightMesh = PointLightMesh();
							lightRayMaterial = pb_BuiltinResource.GetMaterial(pb_BuiltinResource.mat_UnlitVertexColor);
							break;

						case LightType.Area:
							_lightMesh = AreaLightMesh();
							lightRayMaterial = pb_BuiltinResource.GetMaterial(pb_BuiltinResource.mat_UnlitVertexColorWavy);
							break;

						default:
							_lightMesh = null;
							break;
					}
				}
		
				return _lightMesh;
			}
		}

		private Mesh pointLightDisc;

		private void RebuildGizmos()
		{
			if(_lightMesh != null)
			{
				pb_ObjectUtility.Destroy(lightMesh);
				_lightMesh = null;
			}

			if(pointLightDisc == null)
			{
				pointLightDisc = new Mesh();
				pb_HandleMesh.CreateDiscMesh(ref pointLightDisc, 64, 1.1f);
				pointLightDisc.colors = pb_CollectionUtil.Fill<Color>(yellow, pointLightDisc.vertexCount);
			}
		}

		private void Start()
		{
			icon = pb_BuiltinResource.GetMaterial(pb_BuiltinResource.mat_LightGizmo);
			lightComponent = GetComponentInChildren<Light>();
			RebuildGizmos();
		}

		public override void Update()
		{
			base.Update();

			if(!isSelected)
				return;

			if(lightMesh == null || lightComponent == null)
				return;

			switch(lightComponent.type)
			{
				case LightType.Point:

					gizmoMatrix.SetTRS(trs.position, cam.localRotation, Vector3.one * lightComponent.range);		
					Graphics.DrawMesh(pointLightDisc, gizmoMatrix, lightRayMaterial, 0, null, 0, null, false, false);
					gizmoMatrix.SetTRS(trs.position, Quaternion.identity, Vector3.one * lightComponent.range);		
					break;

				default:
					gizmoMatrix.SetTRS(trs.position, trs.localRotation, Vector3.one);
					break;
			}

			for(int i = 0; i < lightMesh.subMeshCount; i++)
				Graphics.DrawMesh(lightMesh, gizmoMatrix, lightRayMaterial, 0, null, i, null, false, false);
		}

		public override void OnComponentModified()
		{
			RebuildGizmos();
		}

		private Mesh PointLightMesh()
		{
			Mesh m = new Mesh();
			
			pb_HandleMesh.CreateRotateMesh(ref m, 64, 1f);

			Color light_yellow = yellow;
			light_yellow.a = .2f;
			m.colors = pb_CollectionUtil.Fill<Color>(light_yellow, m.vertexCount);

			return m;
		}

		private Mesh DirectionalLightMesh()
		{
			Mesh m = new Mesh();

			const float EXTENTS = 4f;
			const int segments = 32;

			Vector3[] v = new Vector3[segments * 3];
			Vector2[] u = new Vector2[segments * 3];
			int[] t = new int[segments * 3 * 2];

			int n = 0, c = 0;
			for(int i = 0; i < segments; i++)
			{
				float dist = i/(float)segments;

				v[n+0] = new Vector3(-.6f, -.5f, dist * EXTENTS);
				v[n+1] = new Vector3( .6f, -.5f, dist * EXTENTS);
				v[n+2] = new Vector3(  0f,  .2f, dist * EXTENTS);

				u[n+0] = new Vector2(dist, (1-dist) + .0f);
				u[n+1] = new Vector2(dist, (1-dist) + .23f);
				u[n+2] = new Vector2(dist, (1-dist) + .34f);

				if(i < segments - 1)
				{
					t[c++] = n+0;
					t[c++] = n+3;

					t[c++] = n+1;
					t[c++] = n+4;

					t[c++] = n+2;
					t[c++] = n+5;
				}

				n += 3;
			}

			m.vertices = v;
			m.uv = u;
			m.colors = pb_CollectionUtil.Fill<Color>(Color.yellow, m.vertexCount);
			m.normals = pb_CollectionUtil.Fill<Vector3>(Vector3.up, m.vertexCount);

			m.subMeshCount = 1;
			m.SetIndices(t, MeshTopology.Lines, 0);

			return m;
		}

		private Mesh SpotLightMesh()
		{
			Mesh m = new Mesh();

			float r = lightComponent.range * Mathf.Tan( Mathf.Deg2Rad * (lightComponent.spotAngle / 2f) );

			const int RADIUS_INC = 32;

			Vector3[] v = new Vector3[RADIUS_INC + 1];
			int[] tris = new int[RADIUS_INC * 2 + 8];

			int n = 0;

			for(int i = 0; i < RADIUS_INC; i++)
			{
				float p = (i/(float)RADIUS_INC) * 360f * Mathf.Deg2Rad;
				v[i] = new Vector3( Mathf.Cos(p) * r, Mathf.Sin(p) * r, lightComponent.range );
				tris[n++] = i;
				tris[n++] = i < (RADIUS_INC - 1) ? i + 1 : 0;
			}

			v[RADIUS_INC] = Vector3.zero;

			tris[n++] = RADIUS_INC;
			tris[n++] = 0;
			tris[n++] = RADIUS_INC;
			tris[n++] = RADIUS_INC / 4;
			tris[n++] = RADIUS_INC;
			tris[n++] = RADIUS_INC / 2;
			tris[n++] = RADIUS_INC;
			tris[n++] = (RADIUS_INC / 4) * 3;
			
			m.vertices = v;
			m.normals = pb_CollectionUtil.Fill<Vector3>(Vector3.up, v.Length);
			m.colors = pb_CollectionUtil.Fill<Color>(yellow, v.Length);

			m.subMeshCount = 1;
			m.SetIndices(tris, MeshTopology.Lines, 0);

			return m;
		}

		private Mesh AreaLightMesh()
		{
			Mesh m = new Mesh();

			// Vector3 areaSize = new Vector3(light.areaSize.x, light.areaSize.y, 0f);
			Vector3 areaSize = Vector3.one;

			m.vertices = new Vector3[]
			{
				Vector3.Scale(new Vector3(-.5f, -.5f, 0f), areaSize),
				Vector3.Scale(new Vector3( .5f, -.5f, 0f), areaSize),
				Vector3.Scale(new Vector3( .5f,  .5f, 0f), areaSize),
				Vector3.Scale(new Vector3(-.5f,  .5f, 0f), areaSize),

				Vector3.Scale(new Vector3(-.4f, -.4f, 0f), areaSize),
				Vector3.Scale(new Vector3( .4f, -.4f, 0f), areaSize),
				Vector3.Scale(new Vector3( .4f,  .4f, 0f), areaSize),
				Vector3.Scale(new Vector3(-.4f,  .4f, 0f), areaSize),

				Vector3.Scale(new Vector3(-.4f, -.4f, 0f), areaSize) + Vector3.forward,
				Vector3.Scale(new Vector3( .4f, -.4f, 0f), areaSize) + Vector3.forward,
				Vector3.Scale(new Vector3( .4f,  .4f, 0f), areaSize) + Vector3.forward,
				Vector3.Scale(new Vector3(-.4f,  .4f, 0f), areaSize) + Vector3.forward
			};

			m.subMeshCount = 1;
			m.SetIndices( new int[] {
					0, 1,
					1, 2,
					2, 3,
					3, 0,
					4, 8,
					5, 9,
					6, 10,
					7, 11
				},
				MeshTopology.Lines,
				0 );

			m.colors = pb_CollectionUtil.Fill<Color>(yellow, m.vertexCount);

			return m;
		}
	}
}