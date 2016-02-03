using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace GILES.Interface
{
	/**
	 * Given a pb_GUIStyle, this applies it to the current object.
	 */
	public class pb_GUIStyleApplier : MonoBehaviour
	{
		public bool ignoreStyle;
		public pb_GUIStyle style;

		void Awake()
		{
			if(!ignoreStyle)
				ApplyStyle();
		}

		public void ApplyStyle()
		{
			if(style == null)
				return;

			ApplyRecursive(gameObject);
		}

		private void ApplyRecursive(GameObject go)
		{
			foreach(Graphic graphic in go.GetComponents<Graphic>())
				style.Apply(graphic);

			foreach(Selectable selectable in go.GetComponents<Selectable>())
				style.Apply(selectable);

			foreach(Transform t in go.transform)
			{
				if(t.gameObject.GetComponent<pb_GUIStyleApplier>() != null)
					continue;

				ApplyRecursive(t.gameObject);
			}
		}
	}
}
