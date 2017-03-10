using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GILES;
using System.Collections;
using System.Collections.Generic;

namespace GILES.Interface
{

	public class pb_PrefabBrowserItemAssetBundlenButton : pb_PrefabBrowserItemButton
	{
		public pb_PrefabBrowser browser;
		public string bundleName;

		protected override void OnClick(){
			if (browser) {
				browser.openAssetBundle (bundleName);
				DestroyInstance ();
			}
		}

		public override void Initialize(){
			base.Initialize ();
			if (previewComponent != null) {
				GameObject go = previewComponent.gameObject;
				GameObject child = go.AddChild ();
				Text text = child.AddComponent<Text> ();
				text.font = pb_GUIUtility.DefaultFont ();
				text.alignment = TextAnchor.LowerCenter;
				text.text = asset.name;
			}
		}

		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter (eventData);
			Renderer renderer = instance.GetComponent<Renderer> ();
			renderer.enabled = false;
		}

		public override void OnPointerExit(PointerEventData eventData)
		{
			Renderer renderer = instance.GetComponent<Renderer> ();
			renderer.enabled = true;
			base.OnPointerExit (eventData);
		}


	}
}

