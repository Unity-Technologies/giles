using UnityEngine;
using System.Collections;

namespace GILES
{
	/**
	 * Draw a few arrows pointing in the direction that this light is facing.
	 */
	[pb_Gizmo(typeof(Camera))]
	public class pb_Gizmo_Camera : pb_Gizmo
	{
		void Start()
		{
			icon = pb_BuiltinResource.GetMaterial(pb_BuiltinResource.mat_CameraGizmo);
		}
	}
}