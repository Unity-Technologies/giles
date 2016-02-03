using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GILES.Interface
{
	/**
	 * Controls the toggles in the scene that shows / hides panels.
	 */
	public class pb_PanelToggle : pb_ToolbarButton
	{
		/// The panel to enable / disable with this toggle.
		public GameObject panel;

		private Color 	onColor = new Color(1f, .68f, 55f/255f, 1f), 
						offColor = new Color(.26f, .26f, .26f, 1f);

		protected override void Start()
		{
			base.Start();
			
			onColor = selectable.colors.normalColor;
			offColor = selectable.colors.disabledColor;
		}

		public void DoToggle()
		{
			panel.SetActive(!panel.activeInHierarchy);

			ColorBlock block = selectable.colors;
			block.normalColor = panel.activeInHierarchy ? onColor : offColor;
			selectable.colors = block;
		}
	}
}
