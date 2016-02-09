using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using GILES.Serialization;

namespace GILES
{
	/**
	 * Represents the root object of a scene graph.  When adding or removing objects from 
	 * the scene, make sure to register those changes with this class.
	 */
	[pb_JsonIgnore]
	[RequireComponent(typeof(pb_MetaDataComponent))]
	public class pb_Scene : pb_MonoBehaviourSingleton<pb_Scene>
	{	
#region Initialization & Singleton

		protected override void Awake()
		{
			base.Awake();
			this.name = "Level Editor SceneGraph Root";
		}

		void Start()
		{
			pb_MetaDataComponent md = gameObject.GetComponent<pb_MetaDataComponent>();
			if(md == null) md = gameObject.AddComponent<pb_MetaDataComponent>();
		}
#endregion

#region Members
#endregion

#region Delegate

		/**
		 * Event raised when an object is instantiated in the scene.  Passes the new 
		 * object as a parameter.
		 */
		public event Callback<GameObject> onObjectInstantiated;

		/**
		 * Event raised when a level is loaded.
		 */
		public event Callback onLevelLoaded;

		/**
		 * Event raised when a level cleared.
		 */
		public event Callback onLevelCleared;

		/**
		 * Notification when a new object is instantiated in the scene.
		 */
		public static void AddOnObjectInstantiatedListener(Callback<GameObject> listener)
		{
			if(instance.onObjectInstantiated == null)
				instance.onObjectInstantiated = listener;
			else
				instance.onObjectInstantiated += listener;
		}

		/**
		 * Add a callback to be notified when a pb_Scene is loaded from a file or JSON string.
		 */
		public static void AddOnLevelLoadedListener(Callback listener)
		{
			if(instance.onLevelLoaded == null)
				instance.onLevelLoaded = listener;
			else
				instance.onLevelLoaded += listener;
		}

		/**
		 * Add a callback to be notified when a pb_Scene is cleared.
		 */
		public static void AddOnLevelClearedListener(Callback listener)
		{
			if(instance.onLevelCleared == null)
				instance.onLevelCleared = listener;
			else
				instance.onLevelCleared += listener;
		}

#endregion

		/**
		 * Wrapper around UnityEngine.GameObject.Instantiate() that makes sure the new object is correctly added
		 * to the Level Editor scenegraph.  In order for objects to be saved and loaded properly, they must belong
		 * to the scenegraph.
		 */
		public static UnityEngine.GameObject Instantiate(UnityEngine.GameObject original)
		{
			GameObject go = (GameObject) GameObject.Instantiate(original);

			if(original.transform.parent != null)
				go.transform.SetParent(original.transform.parent);
			else
				go.transform.parent = instance.transform;

			pb_EditorComponentAttribute.StripEditorComponents(go);

			if( instance.onObjectInstantiated != null )
				instance.onObjectInstantiated(go);

			return go;
		}

		/**
		 * Wrapper around UnityEngine.GameObject.Instantiate() that makes sure the new object is correctly added
		 * to the Level Editor scenegraph.  In order for objects to be saved and loaded properly, they must belong
		 * to the scenegraph.
		 */
		public static UnityEngine.GameObject Instantiate(UnityEngine.GameObject original, Vector3 position, Quaternion rotation)
		{
			GameObject go = (GameObject) GameObject.Instantiate(original, position, rotation);

			if(original.transform.parent != null)
				go.transform.SetParent(original.transform.parent);
			else
				go.transform.parent = instance.transform;

			pb_EditorComponentAttribute.StripEditorComponents(go);

			if( instance.onObjectInstantiated != null )
				instance.onObjectInstantiated(go);

			return go;
		}

		/**
		 *	Save the current level.  Returns a JSON formatted string with the entire scene-graph serialized.
		 */
		public static string SaveLevel()
		{
			pb_SceneNode rootNode = new pb_SceneNode(instance.gameObject);
			string scenegraph = JsonConvert.SerializeObject(rootNode,
															Formatting.Indented,
															pb_Serialization.ConverterSettings);

			return scenegraph;
		}

		/**
		 *	Load a saved level into the scene.  This clears the currently open scene.
		 */
		public static void LoadLevel(string levelJson)
		{
			if(pb_Scene.nullableInstance != null)
				pb_Scene.instance.Clear();

			pb_Scene scene = pb_Scene.instance;

			pb_SceneNode root_node = (pb_SceneNode) JsonConvert.DeserializeObject<pb_SceneNode>(levelJson, pb_Serialization.ConverterSettings);

			GameObject root = root_node.ToGameObject();

			Transform[] children = new Transform[root.transform.childCount];

			int i = 0;
			foreach(Transform t in root.transform)
				children[i++] = t;

			foreach(Transform t in children)
				t.SetParent(scene.transform);

			pb_ObjectUtility.Destroy(root);

			if( instance.onLevelLoaded != null )
				instance.onLevelLoaded();
		}

		/**
		 * Destroy all children in the scene.
		 */
		public void Clear()
		{
			/// @todo don't reference pb_Selection in pb_Scene.
			pb_Selection.Clear();
			
			foreach(Transform t in transform)
				pb_ObjectUtility.Destroy(t.gameObject);

			if( onLevelCleared != null )
				onLevelCleared();
		}

		/**
		 * Recursively search a transform for children and return all of 'em as a list.
		 * Does not include the root transform in list.
		 */
		public static List<GameObject> Children()
		{
			return instance.GetChildren(instance.transform);
		}

		/**
		 * Recursively search a transform for children and return all of 'em as a list.
		 * Does not include the root transform in list.
		 */
		private List<GameObject> GetChildren(Transform transform)
		{
			List<GameObject> children = new List<GameObject>();
			
			foreach(Transform t in transform)
			{
				if(!t.gameObject.activeSelf)
					continue;

				children.Add(t.gameObject);
				children.AddRange( GetChildren(t) );
			}

			return children;
		}
	}
}
