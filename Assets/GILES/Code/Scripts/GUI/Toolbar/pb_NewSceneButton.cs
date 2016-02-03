using UnityEngine;
using System.Collections;

namespace GILES
{
	public class pb_NewSceneButton : pb_ToolbarButton
	{
		public override string tooltip { get { return "Discard changes and open new scene"; } }

		public void OpenNewScene()
		{
			pb_Scene.instance.Clear();
		}
	}
}