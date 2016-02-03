using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using GILES;

namespace GILES.Interface
{
	public class pb_PrefabBrowser : MonoBehaviour
	{
		List<GameObject> prefabs;

		void Start()
		{
			prefabs = pb_ResourceManager.LoadAll<GameObject>().ToList();

			foreach(GameObject go in prefabs)
			{
				GameObject icon = transform.gameObject.AddChild();
				
				pb_PrefabBrowserItemButton button = icon.AddComponent<pb_PrefabBrowserItemButton>();
				button.asset = go;
				button.Initialize();
			}
		}

	}
}