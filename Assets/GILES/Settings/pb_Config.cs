using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GILES
{
	/**
	 * General editor settings.
	 */
	public static class pb_Config
	{
		public const int ASSET_MENU_ORDER = 800;

		/**
		 * When saving and loading levels using the Resources folder, the following subfolders will
		 * searched for assets.
		 */
		public static readonly string[] Resource_Folder_Paths = new string[]
		{
			"Level Editor Prefabs"
		};

		/**
		 * When saving an loading levels using AssetBundles, these are the names that will be automatically
		 * added to the pb_AssetBundles available bundles list.  You may add additional asset bundles at
		 * runtime using `pb_AssetBundles.RegisterAssetBundle()`.
		 */
		public static readonly string[] AssetBundle_Names = new string[]
		{
			"TestAssets"
		};

		/**
		 * When loading AssetBundle_Names, these are the directories that will be scanned for matching files.
		 */
		public static readonly string[] AssetBundle_SearchDirectories = new string[]
		{
			"Assets/AssetBundles/"
		};

		/**
		 * By default, ignore all default Unity components (save lights and transform)
		 */
		public static readonly HashSet<Type> IgnoredComponentsInInspector = new HashSet<Type>()
		{
			typeof(UnityEngine.OcclusionArea),
			typeof(UnityEngine.OcclusionPortal),
			typeof(UnityEngine.MeshFilter),
			typeof(UnityEngine.SkinnedMeshRenderer),
			typeof(UnityEngine.LensFlare),
			typeof(UnityEngine.Renderer),
			typeof(UnityEngine.Projector),
			typeof(UnityEngine.Skybox),
			typeof(UnityEngine.TrailRenderer),
			typeof(UnityEngine.LineRenderer),
			typeof(UnityEngine.MeshRenderer),
			typeof(UnityEngine.GUIElement),
			typeof(UnityEngine.UI.Image),
			//typeof(UnityEngine.GUITexture),
			typeof(UnityEngine.GUILayer),
			typeof(UnityEngine.ReflectionProbe),
			typeof(UnityEngine.LODGroup),
			typeof(UnityEngine.FlareLayer),
			typeof(UnityEngine.LightProbeGroup),
			typeof(UnityEngine.NetworkView),
			typeof(UnityEngine.RectTransform),
			typeof(UnityEngine.SpriteRenderer),
			typeof(UnityEngine.Behaviour),
			typeof(UnityEngine.Camera),
			typeof(UnityEngine.MonoBehaviour),
			typeof(UnityEngine.Component),
			// typeof(UnityEngine.Light),
			// typeof(UnityEngine.Transform),
			typeof(UnityEngine.BillboardRenderer),
			typeof(UnityEngine.Playables.PlayableDirector),
			typeof(UnityEngine.WindZone),
			typeof(UnityEngine.ParticleSystem),
			typeof(UnityEngine.ParticleSystemRenderer),
#if UNITY_5_2 || UNITY_5_3
			typeof(UnityEngine.ParticleEmitter),
			typeof(UnityEngine.EllipsoidParticleEmitter),
#endif
			typeof(UnityEngine.MeshParticleEmitter),
			typeof(UnityEngine.ParticleAnimator),
			typeof(UnityEngine.ParticleRenderer),
			typeof(UnityEngine.Rigidbody),
			typeof(UnityEngine.Joint),
			typeof(UnityEngine.HingeJoint),
			typeof(UnityEngine.SpringJoint),
			typeof(UnityEngine.FixedJoint),
			typeof(UnityEngine.CharacterJoint),
			typeof(UnityEngine.ConfigurableJoint),
			typeof(UnityEngine.ConstantForce),
			typeof(UnityEngine.Collider),
			typeof(UnityEngine.BoxCollider),
			typeof(UnityEngine.SphereCollider),
			typeof(UnityEngine.MeshCollider),
			typeof(UnityEngine.CapsuleCollider),
			typeof(UnityEngine.WheelCollider),
			typeof(UnityEngine.CharacterController),
			typeof(UnityEngine.Cloth),
			typeof(UnityEngine.Rigidbody2D),
			typeof(UnityEngine.Collider2D),
			typeof(UnityEngine.CircleCollider2D),
			typeof(UnityEngine.BoxCollider2D),
			typeof(UnityEngine.EdgeCollider2D),
			typeof(UnityEngine.PolygonCollider2D),
			typeof(UnityEngine.Joint2D),
			typeof(UnityEngine.AnchoredJoint2D),
			typeof(UnityEngine.SpringJoint2D),
			typeof(UnityEngine.DistanceJoint2D),
			typeof(UnityEngine.HingeJoint2D),
			typeof(UnityEngine.SliderJoint2D),
			typeof(UnityEngine.WheelJoint2D),
			typeof(UnityEngine.PhysicsUpdateBehaviour2D),
			typeof(UnityEngine.ConstantForce2D),
			typeof(UnityEngine.Effector2D),
			typeof(UnityEngine.AreaEffector2D),
			typeof(UnityEngine.PointEffector2D),
			typeof(UnityEngine.PlatformEffector2D),
			typeof(UnityEngine.SurfaceEffector2D),
			typeof(UnityEngine.AI.NavMeshAgent),
			typeof(UnityEngine.AI.NavMeshObstacle),
			typeof(UnityEngine.AI.OffMeshLink),
			typeof(UnityEngine.AudioListener),
			typeof(UnityEngine.AudioSource),
			typeof(UnityEngine.AudioReverbZone),
			typeof(UnityEngine.AudioLowPassFilter),
			typeof(UnityEngine.AudioHighPassFilter),
			typeof(UnityEngine.AudioDistortionFilter),
			typeof(UnityEngine.AudioEchoFilter),
			typeof(UnityEngine.AudioChorusFilter),
			typeof(UnityEngine.AudioReverbFilter),
			typeof(UnityEngine.Animation),
			typeof(UnityEngine.Animator),
			typeof(UnityEngine.Terrain),
			typeof(UnityEngine.Tree),
			typeof(UnityEngine.GUIText),
			typeof(UnityEngine.TextMesh),
			typeof(UnityEngine.Canvas),
			typeof(UnityEngine.CanvasGroup),
			typeof(UnityEngine.CanvasRenderer),
			typeof(UnityEngine.TerrainCollider),
			typeof(UnityEngine.Networking.Match.NetworkMatch),
			typeof(UnityEngine.Networking.NetworkAnimator),
			typeof(UnityEngine.Networking.NetworkBehaviour),
			typeof(UnityEngine.Networking.NetworkDiscovery),
			typeof(UnityEngine.Networking.NetworkIdentity),
			typeof(UnityEngine.Networking.NetworkLobbyManager),
			typeof(UnityEngine.Networking.NetworkLobbyPlayer),
			typeof(UnityEngine.Networking.NetworkManager),
			typeof(UnityEngine.Networking.NetworkManagerHUD),
			typeof(UnityEngine.Networking.NetworkProximityChecker),
			typeof(UnityEngine.Networking.NetworkStartPosition),
			typeof(UnityEngine.Networking.NetworkTransformChild),
			typeof(UnityEngine.Networking.NetworkTransform),
			typeof(UnityEngine.Networking.NetworkTransformVisualizer),
			typeof(UnityEngine.EventSystems.EventSystem),
			typeof(UnityEngine.EventSystems.EventTrigger),
			typeof(UnityEngine.EventSystems.UIBehaviour),
			typeof(UnityEngine.EventSystems.BaseInputModule),
			typeof(UnityEngine.EventSystems.PointerInputModule),
			typeof(UnityEngine.EventSystems.StandaloneInputModule),
#if UNITY_5_2
			// @todo drop unity 5.2 support
			typeof(UnityEngine.EventSystems.TouchInputModule),
#endif
			typeof(UnityEngine.EventSystems.BaseRaycaster),
			typeof(UnityEngine.EventSystems.Physics2DRaycaster),
			typeof(UnityEngine.EventSystems.PhysicsRaycaster),
			typeof(UnityEngine.UI.Button),
			typeof(UnityEngine.UI.Dropdown),
			typeof(UnityEngine.UI.Graphic),
			typeof(UnityEngine.UI.GraphicRaycaster),
			typeof(UnityEngine.UI.Image),
			typeof(UnityEngine.UI.InputField),
			typeof(UnityEngine.UI.Mask),
			typeof(UnityEngine.UI.MaskableGraphic),
			typeof(UnityEngine.UI.RawImage),
			typeof(UnityEngine.UI.RectMask2D),
			typeof(UnityEngine.UI.Scrollbar),
			typeof(UnityEngine.UI.ScrollRect),
			typeof(UnityEngine.UI.Selectable),
			typeof(UnityEngine.UI.Slider),
			typeof(UnityEngine.UI.Text),
			typeof(UnityEngine.UI.Toggle),
			typeof(UnityEngine.UI.ToggleGroup),
			typeof(UnityEngine.UI.AspectRatioFitter),
			typeof(UnityEngine.UI.CanvasScaler),
			typeof(UnityEngine.UI.ContentSizeFitter),
			typeof(UnityEngine.UI.GridLayoutGroup),
			typeof(UnityEngine.UI.HorizontalLayoutGroup),
			typeof(UnityEngine.UI.HorizontalOrVerticalLayoutGroup),
			typeof(UnityEngine.UI.LayoutElement),
			typeof(UnityEngine.UI.LayoutGroup),
			typeof(UnityEngine.UI.VerticalLayoutGroup),
			typeof(UnityEngine.UI.BaseMeshEffect),
			typeof(UnityEngine.UI.Outline),
			typeof(UnityEngine.UI.PositionAsUV1),
			typeof(UnityEngine.UI.Shadow),

			/// misc additional
//			typeof(pb_Object),
//			typeof(pb_Entity)
		};
	}
}
