using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GILES
{
/**
 *	A selection handle for translation, rotation, and scale.
 *  @todo Document and clean up
 */
public class pb_SelectionHandle : pb_MonoBehaviourSingleton<pb_SelectionHandle>
{
#region Member

	public static float positionSnapValue = 1f;
	public static float scaleSnapValue = .1f;
	public static float rotationSnapValue = 15f;

	private Transform _trs;
	private Transform trs { get { if(_trs == null) _trs = gameObject.GetComponent<Transform>(); return _trs; } }
	private Camera _cam;
	private Camera cam { get { if(_cam == null) _cam = Camera.main; return _cam; } }

	const int MAX_DISTANCE_TO_HANDLE = 15;

	static Mesh _HandleLineMesh = null, _HandleTriangleMesh = null;

	public Mesh ConeMesh;	// Used as translation handle cone caps.
	public Mesh CubeMesh;	// Used for scale handle

	private Material HandleOpaqueMaterial
	{
		get
		{
			return pb_BuiltinResource.GetMaterial(pb_BuiltinResource.mat_HandleOpaque);
		}
	}

	private Material RotateLineMaterial 
	{
		get
		{
			return pb_BuiltinResource.GetMaterial(pb_BuiltinResource.mat_RotateHandle);
		}
	}

	private Material HandleTransparentMaterial
	{
		get
		{
			return pb_BuiltinResource.GetMaterial(pb_BuiltinResource.mat_HandleTransparent);
		}
	}

	private Mesh HandleLineMesh
	{
		get
		{
			if(_HandleLineMesh == null)
			{
				_HandleLineMesh = new Mesh();
				CreateHandleLineMesh(ref _HandleLineMesh, Vector3.one);
			}
			return _HandleLineMesh;
		}
	}

	private Mesh HandleTriangleMesh
	{
		get
		{
			if(_HandleTriangleMesh == null)
			{
				_HandleTriangleMesh = new Mesh();
				 CreateHandleTriangleMesh(ref _HandleTriangleMesh, Vector3.one);
			}
			return _HandleTriangleMesh;
		}
	}

	private Tool tool = Tool.Position;	// Current tool.
		
	private Mesh _coneRight, _coneUp, _coneForward;

	const float CAP_SIZE = .07f;

	public float HandleSize = 90f;

	public Callback onHandleTypeChanged = null;

	private Vector2 mouseOrigin = Vector2.zero;
	public bool draggingHandle { get; private set; }
	private int draggingAxes = 0;	// In how many directions is the handle able to move
	private Vector3 scale = Vector3.one;
	private pb_Transform handleOrigin = pb_Transform.identity;

	public bool isHidden { get; private set; }
	public bool InUse() { return draggingHandle; }

#endregion

#region Initialization

	protected override void Awake()
	{
		_trs = null;
		_cam = null;
		base.Awake();
	}

	void Start()
	{
		pb_SceneCamera.AddOnCameraMoveDelegate(this.OnCameraMove);

		SetIsHidden(true);

	}

	public void SetTRS(Vector3 position, Quaternion rotation, Vector3 scale)
	{
		trs.position = position;
		trs.localRotation = rotation;
		trs.localScale = scale;

		RebuildGizmoMatrix();
	}
#endregion

#region Delegate

	public delegate void OnHandleMoveEvent(pb_Transform transform);
	public event OnHandleMoveEvent OnHandleMove;

	public delegate void OnHandleBeginEvent(pb_Transform transform);
	public event OnHandleBeginEvent OnHandleBegin;

	public delegate void OnHandleFinishEvent();
	public event OnHandleFinishEvent OnHandleFinish;

	void OnCameraMove(pb_SceneCamera cam)
	{
		RebuildGizmoMesh(Vector3.one);
		RebuildGizmoMatrix();
	}
#endregion

#region Update

	class DragOrientation
	{
		public Vector3 origin;
		public Vector3 axis;
		public Vector3 mouse;
		public Vector3 cross;
		public Vector3 offset;
		public Plane plane;

		public DragOrientation()
		{
			origin	= Vector3.zero;
			axis	= Vector3.zero;
			mouse	= Vector3.zero;
			cross	= Vector3.zero;
			offset	= Vector3.zero;
			plane	= new Plane(Vector3.up, Vector3.zero);
		}
	}

	DragOrientation drag = new DragOrientation();
	
	void Update()
	{
		if( isHidden )
		{
			return;
		}

		if( Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftAlt))
		{
			OnMouseDown();
		}

		if( draggingHandle )
		{
			if( Input.GetKey(KeyCode.LeftAlt) )
				OnFinishHandleMovement();

#if DEBUG
			Vector3 dir = drag.axis * 2f;
			Color col = new Color(Mathf.Abs(drag.axis.x), Mathf.Abs(drag.axis.y), Mathf.Abs(drag.axis.z), 1f);
			float lineTime = 0f;
			Debug.DrawRay(drag.origin, dir * 1f, col, lineTime, false);
			Debug.DrawLine(drag.origin + dir, (drag.origin + dir * .9f) + (trs.up * .1f), col, lineTime, false);
			Debug.DrawLine(drag.origin + dir, (drag.origin + dir * .9f) + (trs.forward * .1f), col, lineTime, false);
			Debug.DrawLine(drag.origin + dir, (drag.origin + dir * .9f) + (trs.right * .1f), col, lineTime, false);
			Debug.DrawLine(drag.origin + dir, (drag.origin + dir * .9f) + (-trs.up * .1f), col, lineTime, false);
			Debug.DrawLine(drag.origin + dir, (drag.origin + dir * .9f) + (-trs.forward * .1f), col, lineTime, false);
			Debug.DrawLine(drag.origin + dir, (drag.origin + dir * .9f) + (-trs.right * .1f), col, lineTime, false);

			Debug.DrawLine(drag.origin, drag.origin + drag.mouse, Color.red, lineTime, false);
			Debug.DrawLine(drag.origin, drag.origin + drag.cross, Color.black, lineTime, false);
#endif
			}

		if( Input.GetMouseButton(0) && draggingHandle )
		{
			Vector3 a = Vector3.zero;

			bool valid = false;

			if( draggingAxes < 2 && tool != Tool.Rotate )
			{
				Vector3 b;
				valid = pb_HandleUtility.PointOnLine( new Ray(trs.position, drag.axis), cam.ScreenPointToRay(Input.mousePosition), out a, out b);
			}
			else
			{
				Ray ray = cam.ScreenPointToRay(Input.mousePosition);
				float hit = 0f;
				if( drag.plane.Raycast(ray, out hit) )
				{
					a = ray.GetPoint(hit);
					valid = true;
				}
			}

			if( valid )
			{
				drag.origin = trs.position;

				switch( tool )
				{
					case Tool.Position:
					{
						trs.position = pb_Snap.Snap(a - drag.offset, positionSnapValue);
					}
					break;

					case Tool.Rotate:
					{
						Vector2 delta = (Vector2)Input.mousePosition - mouseOrigin;
						mouseOrigin = Input.mousePosition;
						float sign = pb_HandleUtility.CalcMouseDeltaSignWithAxes(cam, drag.origin, drag.axis, drag.cross, delta);
						axisAngle += delta.magnitude * sign;
						trs.localRotation = Quaternion.AngleAxis(pb_Snap.Snap(axisAngle, rotationSnapValue), drag.axis) * handleOrigin.rotation;// trs.localRotation;
					}
					break;

					case Tool.Scale:
					{
						Vector3 v;

						if(draggingAxes > 1)
						{
							v = SetUniformMagnitude( ((a - drag.offset) - trs.position) );
						}
						else
						{
							v = Quaternion.Inverse(handleOrigin.rotation) * ((a - drag.offset) - trs.position);
						}

						v += Vector3.one;
						scale = pb_Snap.Snap(v, scaleSnapValue);
						RebuildGizmoMesh( scale );
					}
					break;
				}

				if( OnHandleMove != null )
					OnHandleMove( GetTransform() );

				RebuildGizmoMatrix();
			}
		}

		if( Input.GetMouseButtonUp(0) )
		{
			OnFinishHandleMovement();
		}
	}

	float axisAngle = 0f;

	/**
	 * Sets all the components of a vector to the component with the largest magnitude.
	 */
	Vector3 SetUniformMagnitude(Vector3 a)
	{
		float max = Mathf.Abs(a.x) > Mathf.Abs(a.y) && Mathf.Abs(a.x) > Mathf.Abs(a.z) ? a.x : Mathf.Abs(a.y) > Mathf.Abs(a.z) ? a.y : a.z;

		a.x = max;
		a.y = max;
		a.z = max;

		return a;
	}

	void OnMouseDown()
	{
		scale = Vector3.one;

		Vector3 a, b;
		drag.offset = Vector3.zero;		
		Axis plane;

		axisAngle = 0f;

		draggingHandle = CheckHandleActivated(Input.mousePosition, out plane);

		mouseOrigin = Input.mousePosition;
		handleOrigin.SetTRS(trs);

		drag.axis = Vector3.zero;
		draggingAxes = 0;

		if( draggingHandle )
		{
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);

			if( (plane & Axis.X) == Axis.X )
			{
				draggingAxes++;
				drag.axis = trs.right;
				drag.plane.SetNormalAndPosition(trs.right.normalized, trs.position);
			}

			if( (plane & Axis.Y) == Axis.Y )
			{
				draggingAxes++;
				if(draggingAxes > 1)
					drag.plane.SetNormalAndPosition(Vector3.Cross(drag.axis, trs.up).normalized, trs.position);
				else
					drag.plane.SetNormalAndPosition(trs.up.normalized, trs.position);
				drag.axis += trs.up;
			}

			if( (plane & Axis.Z) == Axis.Z )
			{
				draggingAxes++;
				if(draggingAxes > 1)
					drag.plane.SetNormalAndPosition(Vector3.Cross(drag.axis, trs.forward).normalized, trs.position);
				else
					drag.plane.SetNormalAndPosition(trs.forward.normalized, trs.position);
				drag.axis += trs.forward;
			}

			if( draggingAxes < 2 )
			{
				if( pb_HandleUtility.PointOnLine(new Ray(trs.position, drag.axis), ray, out a, out b) )
					drag.offset = a - trs.position;

				float hit = 0f;

				if( drag.plane.Raycast(ray, out hit) )
				{
					drag.mouse = (ray.GetPoint(hit) - trs.position).normalized;
					drag.cross = Vector3.Cross(drag.axis, drag.mouse);
				}
			}
			else
			{
				float hit = 0f;

				if( drag.plane.Raycast(ray, out hit) )
				{
					drag.offset = ray.GetPoint(hit) - trs.position;
					drag.mouse = (ray.GetPoint(hit) - trs.position).normalized;
					drag.cross = Vector3.Cross(drag.axis, drag.mouse);
				}
			}

			if( OnHandleBegin != null )
				OnHandleBegin( GetTransform() );
		}
	}

	void OnFinishHandleMovement()
	{
		RebuildGizmoMesh(Vector3.one);
		RebuildGizmoMatrix();

		if( OnHandleFinish != null )
			OnHandleFinish();

		StartCoroutine( SetDraggingFalse() );
	}

	IEnumerator SetDraggingFalse()
	{
		yield return new WaitForEndOfFrame();
		draggingHandle = false;
	}
#endregion

#region Interface

	Vector2 screenToGUIPoint(Vector2 v)
	{
		v.y = Screen.height - v.y;
		return v;
	}

	public pb_Transform GetTransform()
	{
		return new pb_Transform(
			trs.position,
			trs.localRotation,
			scale);
	}

	bool CheckHandleActivated(Vector2 mousePosition, out Axis plane)
	{
		plane = (Axis)0x0;

		if( tool == Tool.Position || tool == Tool.Scale )
		{
			float sceneHandleSize = pb_HandleUtility.GetHandleSize(trs.position);

			// cen
			Vector2 cen = cam.WorldToScreenPoint( trs.position );

			// up
			Vector2 up = cam.WorldToScreenPoint( (trs.position + (trs.up + trs.up * CAP_SIZE * 4f) * (sceneHandleSize * HandleSize)) );

			// right
			Vector2 right = cam.WorldToScreenPoint( (trs.position + (trs.right + trs.right * CAP_SIZE * 4f) * (sceneHandleSize * HandleSize)) );

			// forward
			Vector2 forward = cam.WorldToScreenPoint( (trs.position + (trs.forward + trs.forward * CAP_SIZE * 4f) * (sceneHandleSize * HandleSize)) );

			// First check if the plane boxes have been activated

			Vector3 cameraMask = pb_HandleUtility.DirectionMask(trs, cam.transform.forward);

			Vector2 p_right = (cen + ((right-cen) * cameraMask.x) * HANDLE_BOX_SIZE);
			Vector2 p_up = (cen + ((up-cen) * cameraMask.y) * HANDLE_BOX_SIZE);
			Vector2 p_forward = (cen + ((forward-cen) * cameraMask.z) * HANDLE_BOX_SIZE);

			// x plane
			if( pb_HandleUtility.PointInPolygon( new Vector2[] {
				cen, p_up,
				p_up, (p_up+p_forward) - cen,
				(p_up+p_forward) - cen, p_forward,
				p_forward, cen
				}, mousePosition ) )
				plane = Axis.Y | Axis.Z;
			// y plane
			else if( pb_HandleUtility.PointInPolygon( new Vector2[] {
				cen, p_right,
				p_right, (p_right+p_forward)-cen,
				(p_right+p_forward)-cen, p_forward,
				p_forward, cen
				}, mousePosition ) )
				plane = Axis.X | Axis.Z;
			// z plane
			else if( pb_HandleUtility.PointInPolygon( new Vector2[] {
				cen, p_up,
				p_up, (p_up + p_right) - cen,
				(p_up + p_right) - cen, p_right,
				p_right, cen
				}, mousePosition ) )
				plane = Axis.X | Axis.Y;
			else
			if(pb_HandleUtility.DistancePointLineSegment(mousePosition, cen, up) < MAX_DISTANCE_TO_HANDLE)
				plane = Axis.Y;
			else if(pb_HandleUtility.DistancePointLineSegment(mousePosition, cen, right) < MAX_DISTANCE_TO_HANDLE)
				plane = Axis.X;
			else if(pb_HandleUtility.DistancePointLineSegment(mousePosition, cen, forward) < MAX_DISTANCE_TO_HANDLE)
				plane = Axis.Z;
			else
				return false;

			return true;
		}
		else
		{
			Vector3[][] vertices = pb_HandleMesh.GetRotationVertices(16, 1f);

			float best = Mathf.Infinity;

			Vector2 cur, prev = Vector2.zero;
			plane = Axis.X;

			for(int i = 0; i < 3; i++)
			{
				cur = cam.WorldToScreenPoint(vertices[i][0]);

				for(int n = 0; n < vertices[i].Length-1; n++)
				{
					prev = cur;
					cur = cam.WorldToScreenPoint( handleMatrix.MultiplyPoint3x4(vertices[i][n+1]) );

					float dist = pb_HandleUtility.DistancePointLineSegment(mousePosition, prev, cur);

					if( dist < best && dist < MAX_DISTANCE_TO_HANDLE )
					{
						Vector3 viewDir = (handleMatrix.MultiplyPoint3x4((vertices[i][n] + vertices[i][n+1]) * .5f) - cam.transform.position).normalized;
						Vector3 nrm = transform.TransformDirection(vertices[i][n]).normalized;

						if(Vector3.Dot(nrm, viewDir) > .5f)
							continue;

						best = dist;

						switch(i)
						{
							case 0: // Y
								plane = Axis.Y; // Axis.X | Axis.Z;
								break;

							case 1:	// Z
								plane = Axis.Z;// Axis.X | Axis.Y;
								break;

							case 2:	// X
								plane = Axis.X;// Axis.Y | Axis.Z;
								break;
						}
					}
				}
			}

			if( best < MAX_DISTANCE_TO_HANDLE + .1f)
			{
				return true;
			}
		}

		return false;
	}
#endregion

#region Render

	private Matrix4x4 handleMatrix;

	void OnRenderObject()
	{	
		if(isHidden || Camera.current != cam)
			return;

		switch(tool)
		{
			case Tool.Position:
			case Tool.Scale:
				HandleOpaqueMaterial.SetPass(0);
				Graphics.DrawMeshNow(HandleLineMesh, handleMatrix);
				Graphics.DrawMeshNow(HandleTriangleMesh, handleMatrix, 1);	// Cones

				HandleTransparentMaterial.SetPass(0);
				Graphics.DrawMeshNow(HandleTriangleMesh, handleMatrix, 0);	// Box
				break;

			case Tool.Rotate:
				RotateLineMaterial.SetPass(0);
				Graphics.DrawMeshNow(HandleLineMesh, handleMatrix);
				break;
		}
	}

	void RebuildGizmoMatrix()
	{
		float handleSize = pb_HandleUtility.GetHandleSize(trs.position);
		Matrix4x4 scale = Matrix4x4.Scale(Vector3.one * handleSize * HandleSize);

 		handleMatrix = transform.localToWorldMatrix * scale;
 	}

	void RebuildGizmoMesh(Vector3 scale)
	{
		if(_HandleLineMesh == null)
			_HandleLineMesh = new Mesh();

		if(_HandleTriangleMesh == null)
			_HandleTriangleMesh = new Mesh();

		CreateHandleLineMesh(ref _HandleLineMesh, scale);
		CreateHandleTriangleMesh(ref _HandleTriangleMesh, scale);
	}
#endregion

#region Set Functionality

	public void SetTool(Tool tool)
	{
		if(this.tool != tool)	
		{
			this.tool = tool;
			RebuildGizmoMesh(Vector3.one);

			if(onHandleTypeChanged != null)
				onHandleTypeChanged();
		}
	}

	public Tool GetTool()
	{
		return tool;
	}

	public void SetIsHidden(bool isHidden)
	{
		draggingHandle = false;
		this.isHidden = isHidden;

		if(onHandleTypeChanged != null)
			onHandleTypeChanged();
	}

	public bool GetIsHidden()
	{
		return this.isHidden;
	}

#endregion

#region Mesh Generation

	const float HANDLE_BOX_SIZE = .25f;

	private void CreateHandleLineMesh(ref Mesh mesh, Vector3 scale)
	{
		switch(tool)
		{
			case Tool.Position:
			case Tool.Scale:
				pb_HandleMesh.CreatePositionLineMesh(ref mesh, trs, scale, cam, HANDLE_BOX_SIZE);
				break;

			case Tool.Rotate:
				pb_HandleMesh.CreateRotateMesh(ref mesh, 48, 1f);
				break;

			default:
				return;
		}
	}

	private void CreateHandleTriangleMesh(ref Mesh mesh, Vector3 scale)
	{
		if( tool == Tool.Position )
			pb_HandleMesh.CreateTriangleMesh(ref mesh, trs, scale, cam, ConeMesh, HANDLE_BOX_SIZE, CAP_SIZE);
		else if( tool == Tool.Scale )
			pb_HandleMesh.CreateTriangleMesh(ref mesh, trs, scale, cam, CubeMesh, HANDLE_BOX_SIZE, CAP_SIZE);
	}

#endregion
}
}
