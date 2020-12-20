using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GILES;

namespace GILES.Interface
{
    public class pb_PrefabBrowserItemFolderIconButton : pb_PrefabBrowserItemButton
    {
        // Start is called before the first frame update
        public pb_PrefabBrowser browser;
        public string path;

        protected override void OnClick()
        {
            if (browser)
            {
                browser.openFolder(path);
                DestroyInstance();
            }
        }
        public override void Initialize()
        {
            base.Initialize();
            if (previewComponent != null)
            {
                GameObject go = previewComponent.gameObject;
                GameObject child = go.AddChild();
                Text text = child.AddComponent<Text>();
                text.font = pb_GUIUtility.DefaultFont();
                text.alignment = TextAnchor.LowerCenter;
                text.text = asset.name;
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (instance != null)
            {
                Renderer renderer = instance.GetComponent<Renderer>();
                renderer.enabled = false;
            }
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (instance != null)
            {
                Renderer render = instance.GetComponent<Renderer>();
                render.enabled = true;
            }
            base.OnPointerExit(eventData);
        }
    }
}