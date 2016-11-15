using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System;
using System.Linq;

namespace GILES.Serialization
{
	/**
	 * Json.NET converter override for UnityEngine.Mesh type.
	 */
	public class pb_MeshConverter : pb_UnityTypeConverter<Mesh>
	{
		public override void WriteObjectJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			JObject o = new JObject();

			Mesh m = (UnityEngine.Mesh)value;

			o.Add("$type", value.GetType().AssemblyQualifiedName);

			o.Add("vertices", JArray.FromObject(m.vertices, serializer));
			o.Add("bindposes", JArray.FromObject(m.bindposes, serializer));
			o.Add("boneWeights", JArray.FromObject(m.boneWeights, serializer));
			o.Add("colors", JArray.FromObject(m.colors, serializer));
			o.Add("colors32", JArray.FromObject(m.colors32, serializer));
			o.Add("normals", JArray.FromObject(m.normals, serializer));
			o.Add("tangents", JArray.FromObject(m.tangents, serializer));
			o.Add("uv", JArray.FromObject(m.uv, serializer));
			o.Add("uv2", JArray.FromObject(m.uv2, serializer));
			o.Add("uv3", JArray.FromObject(m.uv3, serializer));
			o.Add("uv4", JArray.FromObject(m.uv4, serializer));

			o.Add("subMeshCount", m.subMeshCount);

			for(int i = 0; i < m.subMeshCount; i++)
			{
				o.Add("meshTopology" + i, (int) m.GetTopology(i));
				o.Add("submesh" + i, JArray.FromObject(m.GetTriangles(i)));
			}

			// inherited
			o.Add("hideFlags", (int) m.hideFlags);
			o.Add("name", m.name);
			o.WriteTo(writer, serializer.Converters.ToArray());
		}

		public override object ReadJsonObject(JObject o, Type objectType, object existingValue, JsonSerializer serializer)
		{
			Mesh m = new Mesh();
			
			Matrix4x4[] bindposes		= o.GetValue("bindposes").ToObject<Matrix4x4[]>();
			BoneWeight[] boneWeights	= o.GetValue("boneWeights").ToObject<BoneWeight[]>();
			Color[] colors				= o.GetValue("colors").ToObject<Color[]>();
			Color32[] colors32			= o.GetValue("colors32").ToObject<Color32[]>();
			Vector3[] normals			= o.GetValue("normals").ToObject<Vector3[]>();
			Vector4[] tangents			= o.GetValue("tangents").ToObject<Vector4[]>();
			Vector2[] uv				= o.GetValue("uv").ToObject<Vector2[]>();
			Vector2[] uv2				= o.GetValue("uv2").ToObject<Vector2[]>();
			Vector2[] uv3				= o.GetValue("uv3").ToObject<Vector2[]>();
			Vector2[] uv4				= o.GetValue("uv4").ToObject<Vector2[]>();
			Vector3[] vertices			= o.GetValue("vertices").ToObject<Vector3[]>();

			m.vertices					= vertices;
			m.bindposes					= bindposes;
			m.boneWeights				= boneWeights;
			m.colors					= colors;
			m.colors32					= colors32;
			m.normals					= normals;
			m.tangents					= tangents;
			m.uv						= uv;
			m.uv2						= uv2;
			m.uv3						= uv3;
			m.uv4						= uv4;

			int subMeshCount = (int) o.GetValue("subMeshCount");
			m.subMeshCount = subMeshCount;

			for(int i = 0; i < subMeshCount; i++)
			{
				int[] indices = (int[]) o.GetValue("submesh" + i).ToObject<int[]>();
				MeshTopology topo = (MeshTopology) (int) o.GetValue("meshTopology" + i);
				m.SetIndices(indices, topo, i);
			}

			m.hideFlags = (HideFlags) (int) o.GetValue("hideFlags");
			m.name = o.GetValue("name").ToObject<string>();

			return m;
		}
	}
}