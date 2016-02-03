using UnityEngine;
using UnityEditor;
using GILES.Interface;
using UnityEditor.UI;

namespace GILES.UnityEditor
{
	/**
	 * Custom editor implementation for pb_InspectorLabelLayoutElement.
	 * 
	 * \notes Despite a valiant effort, AVFoundation remains the undisputed king of super-long class names.
	 */
	[CustomEditor(typeof(pb_InspectorLabelLayoutElement))]
	public class pb_InspectorLabelLayoutElementEditor : Editor
	{
		public override void OnInspectorGUI()
		{
		}
	}
}