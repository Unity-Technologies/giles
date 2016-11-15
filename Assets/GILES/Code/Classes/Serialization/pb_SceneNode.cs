using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using GILES;

namespace GILES.Serialization
{
	/**
	 * Represents a GameObject in a scene.
	 * GameObjects and components are not implemented as JsonConverters because
	 * of the way they're reconstructed when deserializing - if they were converters
	 * it would require serializing the parents and children as properties which
	 * would be messy when rebuilding.
	 */
	[System.Serializable()]
	public class pb_SceneNode : ISerializable
	{
		public string name;
		public pb_Transform transform;
		public List<pb_SceneNode> children;

		public pb_MetaData metadata;
		public List<pb_ISerializable> components;

		/**
		 * Parameterless constructor.  Use only for deserialization.
		 */
		public pb_SceneNode() {}

		/**
		 * Deserialization constructor.
		 */
		public pb_SceneNode(SerializationInfo info, StreamingContext context)
		{
			name 		= (string) info.GetValue("name", typeof(string));
			transform 	= (pb_Transform) info.GetValue("transform", typeof(pb_Transform));
			children 	= (List<pb_SceneNode>) info.GetValue("children", typeof(List<pb_SceneNode>));
			metadata 	= (pb_MetaData) info.GetValue("metadata", typeof(pb_MetaData));

			if( metadata.assetType == AssetType.Instance)
				components 	= (List<pb_ISerializable>) info.GetValue("components", typeof(List<pb_ISerializable>));
		}

		/**
		 * Serialize object data.
		 */
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("name", name, typeof(string));
			info.AddValue("transform", transform, typeof(pb_Transform));
			info.AddValue("children", children, typeof(List<pb_SceneNode>));
			info.AddValue("metadata", metadata, typeof(pb_MetaData));

			if( metadata == null || metadata.assetType == AssetType.Instance )
				info.AddValue("components", components, typeof(List<pb_SerializableObject<Component>>));	
		}

		/**
		 * Recursively build a scene graph using `root` as the root node.
		 */
		public pb_SceneNode(GameObject root)
		{
			name = root.name;

			components = new List<pb_ISerializable>();
			
			pb_MetaDataComponent metadata_component = root.GetComponent<pb_MetaDataComponent>();

			if( metadata_component == null )
				metadata_component = root.AddComponent<pb_MetaDataComponent>();

			metadata = metadata_component.metadata;

			if( metadata.assetType == AssetType.Instance )
			{
				foreach(Component c in root.GetComponents<Component>())
				{
					if( c == null ||
						c is Transform ||
						c.GetType().GetCustomAttributes(true).Any(x => x is pb_JsonIgnoreAttribute))
						continue;

					components.Add( pb_Serialization.CreateSerializableObject<Component>(c) );
				}
			}
			
			// avoid calling constructor which automatically rebuilds the matrix
			transform = new pb_Transform();
			transform.SetTRS(root.transform);

			children = new List<pb_SceneNode>();

			foreach(Transform t in root.transform)
			{
				if(!t.gameObject.activeSelf)
					continue;

				children.Add( new pb_SceneNode(t.gameObject) );
			}
		}

		/**
		 * Convert this node into a GameObject.
		 */
		public GameObject ToGameObject()
		{
			GameObject go;

			if(metadata.assetType == AssetType.Instance)
			{
				go = new GameObject();

				foreach(pb_ISerializable serializedObject in components)
					go.AddComponent(serializedObject);
			}
			else
			{
				GameObject prefab = pb_ResourceManager.LoadPrefabWithMetadata(metadata);

				if(prefab != null)
				{
					go = GameObject.Instantiate( prefab );
					metadata.componentDiff.ApplyPatch(go);
				}
				else
				{
					go = new GameObject();
				}
			}

			pb_MetaDataComponent mc = go.DemandComponent<pb_MetaDataComponent>();
			mc.metadata = metadata;
	
			go.name = name;
			go.transform.SetTRS(transform);

			foreach(pb_SceneNode childNode in children)
			{
				GameObject child = childNode.ToGameObject();
				child.transform.SetParent(go.transform);
			}

			return go;
		}

		public static explicit operator GameObject(pb_SceneNode node)
		{
			return node.ToGameObject();
		}
	}
}