using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace GILES.Interface
{
	/**
	 * GUI element that will resize the `window` rect when dragged.
	 */
	public class pb_WindowResizer : MonoBehaviour, IBeginDragHandler, IDragHandler
	{
		public RectTransform window;

		private Rect screenRect = new Rect(0,0,0,0);

		public Vector2 minSize = new Vector2(100, 100);
		public Vector2 maxSize = new Vector2(10000,10000);

		public void OnBeginDrag(PointerEventData eventData)
		{
			screenRect.width = Screen.width;
			screenRect.height = Screen.height;
		}

		public void OnDrag(PointerEventData eventData)
		{
			if(window == null)
			{
				Debug.LogWarning("Window parent is null, cannot drag a null window.");
				return;
			}

			window.position += (Vector3) eventData.delta * .5f;

			Vector2 flip = new Vector2(eventData.delta.x, -eventData.delta.y);

			window.sizeDelta += flip;

			Rect r = pb_GUIUtility.GetScreenRect(window);

			float w = window.rect.width, h = window.rect.height;

			if( w < minSize.x || w > maxSize.x || (r.x + r.width > screenRect.width) )
			{
				Vector2 size = window.sizeDelta;
				size.x -= flip.x;
				window.sizeDelta = size;

				Vector3 pos = window.position;
				pos.x -= eventData.delta.x * .5f;
				window.position = pos;
			}

			if( h < minSize.y || h > maxSize.y || (r.y - r.height < 0) )
			{
				Vector2 size = window.sizeDelta;
				size.y -= flip.y;
				window.sizeDelta = size;

				Vector3 pos = window.position;
				pos.y -= eventData.delta.y * .5f;
				window.position = pos;
			}

			foreach(pb_IOnResizeHandler handler in window.transform.GetComponentsInChildren<pb_IOnResizeHandler>())
				handler.OnResize();
		}
	}
}
