using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace GILES.Interface
{
	public class pb_Window : UIBehaviour, IPointerDownHandler
	{	
		public virtual void OnPointerDown(PointerEventData eventData)
		{
			EventSystem.current.SetSelectedGameObject(gameObject, eventData);
			transform.SetAsLastSibling();
		}
	}
}