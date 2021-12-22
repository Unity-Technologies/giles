﻿using UnityEngine;
using System.Collections;

namespace GILES
{
	public static class pb_ObjectUtility
	{
		///<summary>
		/// Add an empty gameObject as a child to `go`.
		///</summary>
		public static GameObject AddChild(this GameObject go)
		{
			GameObject child = new GameObject();
			child.transform.SetParent(go.transform);
			return child;
		}

		///<summary>
		/// Add an empty gameObject as a child to `trs`.
		///</summary>
		public static Transform AddChild(this Transform trs)
		{
			Transform go = new GameObject().GetComponent<Transform>();
			go.SetParent(trs);
			return go;
		}
		///<summary>
		///Calculates the min dist. to the bounds.
		///</summary>
		public static float CalcMinDistanceToBounds(Camera cam, Bounds bounds)
		{
			float frustumHeight = Mathf.Max(Mathf.Max(bounds.size.x, bounds.size.y), bounds.size.z);
			float distance = frustumHeight * .5f / Mathf.Tan(cam.fieldOfView * .5f * Mathf.Deg2Rad);

			return distance;
		}

		public static void Destroy<T>(T obj) where T : UnityEngine.Object
		{
			GameObject.Destroy(obj);
		}
	}
}
