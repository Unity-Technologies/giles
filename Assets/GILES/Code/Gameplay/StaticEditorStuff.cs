using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GILES.Static
{
    public static class StaticEditorStuff
    {
        public static bool RunGame { get { return run; } }
        static bool run;

        public static void SetToIngame(bool r)
        {
            run = r;
        }
    }
}