using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

namespace GILES.Interface
{
	/**
	 * A singleton window instance that takes control of the screen when initialized.
	 */
	public class pb_ModalWindow : pb_MonoBehaviourSingleton<pb_ModalWindow>
	{	
		/// Where this window's content belongs.
		public GameObject contents;

		/// The title shown in the header bar.
		public Text windowTitle;

		public static void SetTitle(string title)
		{
			instance.windowTitle.text = title;
		}

		public static void Show()
		{
			foreach(Transform t in instance.transform)
			{
				t.gameObject.SetActive(true);
			}

			instance.transform.SetAsLastSibling();
		}

		public static bool IsVisible()
		{
			foreach(Transform t in instance.transform)
				if(t.gameObject.activeSelf)
					return true;
			return false;
		}

		public static void Hide()
		{
			foreach(Transform t in instance.transform)
				t.gameObject.SetActive(false);
		}

		/**
		 * Add an already instantiated prefab to the contents of this window.
		 */
		public static void SetContent(GameObject prefab)
		{
			foreach(Transform t in instance.contents.transform)
				pb_ObjectUtility.Destroy(t.gameObject);

			prefab.transform.SetParent(instance.contents.transform, false);
		}
	}
}