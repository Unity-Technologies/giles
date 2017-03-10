using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GILES;
using System.Collections;
using System.Collections.Generic;

namespace GILES.Interface
{
	/**
	 * Button implementation that shows a preview of an inspector prefab object.
	 */
	public class pb_PrefabBrowserItemGameObjectsButton  : pb_PrefabBrowserItemButton
	{
		public float cameraRotateSpeed = 50f;
		private bool doSpin = false;


		public override void Initialize()
		{
			prefabId = asset.DemandComponent<pb_MetaDataComponent>().GetFileId();

			previewImage = new Texture2D(PreviewWidth, PreviewHeight);

			if(!SetupAndRenderPreview(previewImage))
			{
				pb_ObjectUtility.Destroy(previewImage);
				previewImage = null;
			}

			gameObject.AddComponent<Mask>();
			gameObject.AddComponent<VerticalLayoutGroup>();
			Image image = gameObject.DemandComponent<Image>();
			image.color = pb_GUIUtility.ITEM_BACKGROUND_COLOR;
			image.sprite = null;

			GameObject description = gameObject.AddChild();

			if(previewImage != null)
			{
				previewComponent = description.AddComponent<RawImage>();
				previewComponent.texture = previewImage;
				/*if (displayNameEnabled) {
					GameObject child = description.AddChild ();
					Text text = child.AddComponent<Text> ();
					text.font = pb_GUIUtility.DefaultFont ();
					text.alignment = TextAnchor.LowerCenter;
					text.text = asset.name;
				}*/
			}
			else
			{
				Text text = description.AddComponent<Text>();
				text.font = pb_GUIUtility.DefaultFont();
				text.alignment = TextAnchor.MiddleCenter;
				text.text = asset.name;
			}
		}

		void Update()
		{
			if(doSpin && previewImage)
			{
				previewCamera.transform.RotateAround(Vector3.zero, Vector3.up, cameraRotateSpeed * Time.deltaTime);
				RenderPreview();
			}
		}

		/**
		 * Instantiate the inspected object in the scene.
		 */
		protected override void OnClick()
		{
			Camera cam = Camera.main;
			GameObject go;

			Vector3 org = pb_Selection.activeGameObject == null ? Vector3.zero : pb_Selection.activeGameObject.transform.position;
			Vector3 nrm = pb_Selection.activeGameObject == null ? Vector3.up : pb_Selection.activeGameObject.transform.localRotation * Vector3.up;

			Plane plane = new Plane(nrm, org);

			Ray ray = new Ray(cam.transform.position, cam.transform.forward);

			float hit = 0f;

			if( plane.Raycast(ray, out hit))
				go = (GameObject) pb_Scene.Instantiate(asset, pb_Snap.Snap(ray.GetPoint(hit), .25f), Quaternion.identity);
			else
				go = (GameObject) pb_Scene.Instantiate(asset, pb_Snap.Snap(ray.GetPoint(10f), .25f), Quaternion.identity);	

			Undo.RegisterStates(new List<IUndo>() { new UndoSelection(), new UndoInstantiate(go) }, "Create new object");
			
			pb_Selection.SetSelection(go);
			
			bool curSelection = pb_Selection.activeGameObject != null;

			if(!curSelection)
				pb_SceneCamera.Focus(go);
		}

		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter (eventData);

			doSpin = true;
		}

		public override void OnPointerExit(PointerEventData eventData)
		{
			if(previewComponent == null)
				return;

			doSpin = false;

			cameraRotation = previewCamera.transform.localRotation;

			RenderPreview();

			previewImage.ReadPixels(new Rect(0,0,renderTexture.width,renderTexture.height), 0, 0);
			previewImage.Apply();

			previewComponent.texture = previewImage;

			RenderTexture.active = null;

			renderTexture.DiscardContents();
			renderTexture.Release();

			pb_ObjectUtility.Destroy(instance);
		}

		protected override bool SetupAndRenderPreview(Texture2D texture)
		{
			if(!SetupPreviewRender())
				return false;

			RenderPreview();

			texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
			texture.Apply();

			RenderTexture.active = null;

			renderTexture.DiscardContents();
			renderTexture.Release();

			pb_ObjectUtility.Destroy(instance);

			return true;
		}

		protected override bool SetupPreviewRender()
		{
			if(asset.GetComponent<Renderer>() == null) return false;

			sceneLights = Object.FindObjectsOfType<Light>();
			lightWasEnabled = new bool[sceneLights.Length];

			instance = (GameObject) GameObject.Instantiate(asset, Vector3.zero, Quaternion.identity);

			Renderer renderer = instance.GetComponent<Renderer>();

			instance.transform.position = -renderer.bounds.center;
			
			instance.layer = PREVIEW_LAYER;

			previewCamera.transform.localRotation = cameraRotation;

			/// can return false if no renderer is available, but since we already check that there's not need to here
			pb_AssetPreview.PrepareCamera(previewCamera, instance, PreviewWidth, PreviewHeight);

			previewCamera.targetTexture = renderTexture;

			return true;
		}

		protected override void RenderPreview()
		{
			for(int i = 0; i < sceneLights.Length; i++)
			{
				lightWasEnabled[i] = sceneLights[i].enabled;
				sceneLights[i].enabled = false;
			}

			RenderTexture.active = renderTexture;

			instance.SetActive(true);

			previewLightA.gameObject.SetActive(true);
			previewLightB.gameObject.SetActive(true);

			previewCamera.Render();

			instance.SetActive(false);

			previewLightA.gameObject.SetActive(false);
			previewLightB.gameObject.SetActive(false);

			for(int i = 0; i < sceneLights.Length; i++)
			{
				sceneLights[i].enabled = lightWasEnabled[i];
			}
		}
	}
}
