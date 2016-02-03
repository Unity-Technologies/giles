using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GILES.Interface;

namespace GILES
{
	public class pb_FullInspectorButton : pb_ToolbarButton
	{
		public override string tooltip { get { return "Show Full Inspector"; } }

		public pb_Inspector inspector;

		private Color 	onColor = new Color(1f, .68f, 55f/255f, 1f), 
						offColor = new Color(.26f, .26f, .26f, 1f);


		protected override void Start()
		{
			base.Start();
			
			onColor = selectable.colors.normalColor;
			offColor = selectable.colors.disabledColor;
			UpdateColors();
		}

		public void DoToggle()
		{
			inspector.showUnityComponents = !inspector.showUnityComponents;
			inspector.RebuildInspector(pb_Selection.activeGameObject);
			UpdateColors();
		}

		void UpdateColors()
		{
			ColorBlock block = selectable.colors;
			block.normalColor = inspector.showUnityComponents ? onColor : offColor;
			selectable.colors = block;
		}
	}
}