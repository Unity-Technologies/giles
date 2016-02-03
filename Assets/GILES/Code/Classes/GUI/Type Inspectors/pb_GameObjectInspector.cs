using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace GILES.Interface
{
	/**
	 * Field editor for GameObject types.
	 */
	[pb_TypeInspector(typeof(GameObject))]
	public class pb_GameObjectInspector : pb_TypeInspector
	{
		GameObject value;

		void OnGUIChanged()
		{
			SetValue(value);
		}

		public override void InitializeGUI()
		{
			value = GetValue<GameObject>();
		}

		protected override void OnUpdateGUI()
		{
		}

		public void OnValueChange(object val)
		{
		}
	}
}