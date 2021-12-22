using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GILES.Example;

namespace GILES
{
    public class pb_TestButton : pb_ToolbarButton
    {
        public void Test()
        {
            pb_SceneLoader.LoadScene(pb_Scene.SaveLevel(), true);
        }
        public override string tooltip { get { return "Test Level"; } }
    }
}