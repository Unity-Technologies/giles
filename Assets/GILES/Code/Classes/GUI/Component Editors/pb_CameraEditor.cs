using UnityEngine;
using System.Collections;
using System.Reflection;
using GILES.Serialization;

namespace GILES.Interface
{
	public class pb_CameraEditor : pb_ComponentEditor
	{
		private Camera _camera;

		protected override void InitializeGUI()
		{
			_camera = (Camera) target;
			
			pb_GUIUtility.AddVerticalLayoutGroup(gameObject);

			pb_TypeInspector enabled_inspector = pb_InspectorResolver.GetInspector(typeof(bool));

			enabled_inspector.Initialize("Enabled", UpdateEnabled, OnSetEnabled);
			enabled_inspector.onValueBeginChange = () => { Undo.RegisterState( new UndoReflection(_camera, "enabled"), "Camera Enabled" ); };
			enabled_inspector.transform.SetParent(transform);
		}

		object UpdateEnabled()
		{
			return _camera.enabled;
		}

		void OnSetEnabled(object value)
		{
			_camera.enabled = (bool) value;
			pb_ComponentDiff.AddDiff(target, "enabled", _camera.enabled);
		}
	}
}