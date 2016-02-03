using UnityEngine;

namespace GILES
{
	/**
	 * Generates preview images of assets asynchronously.
	 */
	public static class pb_AssetPreview
	{
		private static Camera _previewCamera = null;

		private static Camera previewCamera
		{
			get
			{
				if(_previewCamera == null)
				{
					GameObject go = new GameObject();
					go.name = "Asset Preview Camera";
					go.transform.localRotation = Quaternion.Euler(30f, -30f, 0f);
					_previewCamera = go.AddComponent<Camera>();
					go.SetActive(false);
				}

				return _previewCamera;
			}
		}

		public static Texture2D GeneratePreview(Object obj, int width, int height)
		{
			Texture2D tex = null;
			GameObject go = obj as GameObject;

			if(PrepareCamera(previewCamera, go, width, height))
			{
				go = GameObject.Instantiate(go);
				go.transform.position = Vector3.zero;
				
				RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default, 1);
				RenderTexture.active = renderTexture;

				previewCamera.targetTexture = renderTexture;
				previewCamera.Render();

				tex = new Texture2D(width, height);
				tex.ReadPixels(new Rect(0,0,renderTexture.width,renderTexture.height), 0, 0);
				tex.Apply();

				RenderTexture.ReleaseTemporary(renderTexture);

				pb_ObjectUtility.Destroy(go);
			}

			return tex;
		}

		/**
		 * Set up a preview camera to look at a target.
		 */
		public static bool PrepareCamera(Camera cam, GameObject target, int width, int height)
		{
			if(target == null)
				return false;

			Bounds bounds;

			MeshFilter mf = target.GetComponent<MeshFilter>();
			
			if(mf != null && mf.sharedMesh != null)
			{
				bounds = mf.sharedMesh.bounds;
			}
			else
			{
				Renderer renderer = target.GetComponent<Renderer>();

				if(renderer == null)
					return false;

				bounds = renderer.bounds;
			}

			float dist = pb_ObjectUtility.CalcMinDistanceToBounds(cam, bounds);

			cam.transform.position = cam.transform.forward * -(2f + dist);

			return true;
		}
	}
}
