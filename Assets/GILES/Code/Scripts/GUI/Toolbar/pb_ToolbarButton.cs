using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using GILES.Interface;

namespace GILES
{
	/**
	 * Base class for toolbar selectable actions.
	 */
	public class pb_ToolbarButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
		protected Selectable selectable;

		public virtual string tooltip { get { return ""; } }

		public bool interactable
		{
			get
			{
				return selectable.interactable;
			}

			set
			{
				selectable.interactable = value;
			}
		}

		protected virtual void Start()
		{
			selectable = GetComponent<Selectable>();
		}

		protected GameObject tooltip_label = null;

		public virtual void OnPointerClick(PointerEventData eventData) 
		{
			UpdateTooltip();
		}

		public virtual void OnPointerEnter(PointerEventData eventData) 
		{
			ShowTooltip();
		}

		public virtual void OnPointerExit(PointerEventData eventData) 
		{
			HideTooltip();
		}

		public void ShowTooltip()
		{
			string description = tooltip;

			if( !string.IsNullOrEmpty(description) )
			{
				tooltip_label = pb_GUIUtility.CreateLabel(description);
				float width = tooltip_label.GetComponent<Text>().preferredWidth;
				tooltip_label.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 30f);
				tooltip_label.transform.SetParent( GameObject.FindObjectOfType<Canvas>().transform );

				if(transform.position.x + width < Screen.width)
					tooltip_label.transform.position = new Vector3(transform.position.x + width * .5f, transform.position.y - 30f, 0f);
				else
					tooltip_label.transform.position = new Vector3(transform.position.x - width * .5f, transform.position.y - 30f, 0f);
			}
		}

		public void UpdateTooltip()
		{
			if(tooltip_label != null)
			{
				if( string.IsNullOrEmpty(tooltip) )
				{
					GameObject.Destroy(tooltip_label);
				}
				else
				{
					tooltip_label.GetComponent<Text>().text = tooltip;
				}
			}
		}

		public void HideTooltip()
		{
			if(tooltip_label != null)
				GameObject.Destroy(tooltip_label);
		}
	}
}