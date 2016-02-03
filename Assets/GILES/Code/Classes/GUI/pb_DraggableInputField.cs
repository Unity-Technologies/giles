using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace GILES.Interface
{
	/**
	 * An InputField override that adds the ability to intuitively drag and set the inspected value with a right click drag.
	 */
	public class pb_DraggableInputField : InputField
	{
		private bool isDraggingValue = false;
		private float value = 0f;

		public override void OnBeginDrag(PointerEventData eventData)
		{
			if( (contentType == ContentType.IntegerNumber || 
				contentType == ContentType.DecimalNumber) &&
				IsInteractable() &&
				(eventData.button == PointerEventData.InputButton.Right || (eventData.button == PointerEventData.InputButton.Left && Input.GetKey(KeyCode.LeftAlt))))
			{
				string v = m_TextComponent.text;

				if(!float.TryParse(v, out value))
					value = 0f;

				isDraggingValue = true;
			}
			else
			{
				base.OnBeginDrag(eventData);
			}
		}

		public override void OnDrag(PointerEventData eventData)
		{
			if(!isDraggingValue)
			{
				base.OnDrag(eventData);
			}
			else
			{
				float x = eventData.delta.x, y = eventData.delta.y;
				value += (Mathf.Abs(x) > Mathf.Abs(y) ? x : y) / 10f;
				text = contentType == ContentType.DecimalNumber ? value.ToString("g") : ((int)value).ToString();
			}
		}

		public override void OnEndDrag(PointerEventData eventData)
		{
			isDraggingValue = false;

			base.OnEndDrag(eventData);
		}
	}
}
