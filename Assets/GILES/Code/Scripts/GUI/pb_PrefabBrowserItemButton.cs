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
	public abstract class pb_PrefabBrowserItemButton : Button
	{
		protected static int PREVIEW_LAYER = 31;
		protected static int PreviewWidth = 256;
		protected static int PreviewHeight = 256;
		public string prefabId = "";
		protected static readonly Quaternion CAMERA_VIEW_ANGLE = Quaternion.Euler(30f, -30f, 0f);
		public GameObject asset;
		protected Quaternion cameraRotation = CAMERA_VIEW_ANGLE;
		protected RawImage previewComponent;
		protected Texture2D previewImage;
		protected GameObject instance;
		protected Light[] sceneLights;
		protected bool[] lightWasEnabled = null;

		protected static Camera _previewCamera = null;
		protected static Camera previewCamera
		{
			get
			{
				if(_previewCamera == null)
				{
					_previewCamera = new GameObject().AddComponent<Camera>();
					_previewCamera.gameObject.name = "Prefab Browser Asset Preview Camera";
					_previewCamera.cullingMask = 1 << PREVIEW_LAYER;
					_previewCamera.transform.localRotation = CAMERA_VIEW_ANGLE;
					_previewCamera.gameObject.SetActive(false);
				}

				return _previewCamera;
			}
		}
		protected static RenderTexture _renderTexture;
		protected static RenderTexture renderTexture
		{
			get
			{
				if(_renderTexture == null)
				{
					_renderTexture = new RenderTexture(PreviewWidth, PreviewHeight, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
					_renderTexture.autoGenerateMips = false;
					_renderTexture.useMipMap = false;
				}

				return _renderTexture;
			}
		}
		protected static Light _previewLightA = null;
		protected static Light previewLightA
		{
			get
			{
				if(_previewLightA == null)
				{
					GameObject go = new GameObject();
					go.name = "Asset Preview Lighting";
					go.transform.localRotation = Quaternion.Euler(15f, 330f, 0f);
					_previewLightA = go.AddComponent<Light>();
					_previewLightA.type = LightType.Directional;
					_previewLightA.intensity = .5f;
				}

				return _previewLightA;
			}
		}

		protected static Light _previewLightB = null;
		protected static Light previewLightB
		{
			get
			{
				if(_previewLightB == null)
				{
					GameObject go = new GameObject();
					go.name = "Asset Preview Lighting";
					go.transform.localRotation = Quaternion.Euler(15f, 150f, 0f);
					_previewLightB = go.AddComponent<Light>();
					_previewLightB.type = LightType.Directional;
					_previewLightB.intensity = .5f;
				}

				return _previewLightB;
			}
		}

		/**
		 * When this button is instantiated, send event to base and then add the onClick listener.
		 **/
		protected override void Start()
		{
			base.Start();
			onClick.AddListener( OnClick );
		}

		public virtual void Initialize()
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
			}
			else
			{
				Text text = description.AddComponent<Text>();
				text.font = pb_GUIUtility.DefaultFont();
				text.alignment = TextAnchor.MiddleCenter;
				text.text = asset.name;
			}
		}

		/*
		 * Instantiate the inspected object in the scene.
		 */
		protected abstract void OnClick ();

		public override void OnPointerEnter(PointerEventData eventData)
		{
			if(previewComponent == null)
				return;

			if(!SetupPreviewRender())
				return;

			previewComponent.texture = renderTexture;
		}

		public override void OnPointerExit(PointerEventData eventData)
		{
			if(previewComponent == null)
				return;


			RenderPreview();

			previewImage.ReadPixels(new Rect(0,0,renderTexture.width,renderTexture.height), 0, 0);
			previewImage.Apply();

			previewComponent.texture = previewImage;

			RenderTexture.active = null;

			DestroyInstance ();
		}

		protected virtual bool SetupAndRenderPreview(Texture2D texture)
		{
			if(!SetupPreviewRender())
				return false;

			RenderPreview();

			texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
			texture.Apply();

			RenderTexture.active = null;

			DestroyInstance ();

			return true;
		}

		protected virtual bool SetupPreviewRender()
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

		protected virtual void RenderPreview()
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

		public virtual void DestroyInstance(){
			renderTexture.DiscardContents();
			renderTexture.Release();
			pb_ObjectUtility.Destroy(instance);
		}

	}
}
