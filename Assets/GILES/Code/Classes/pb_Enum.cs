namespace GILES
{
	enum HandleMovement
	{
		Up,
		Right,
		Forward,
		Plane
	}

	[System.Flags]
	enum Axis
	{
		None = 0x0,
		X = 0x1,
		Y = 0x2,
		Z = 0x4
	}

	/**
	 * Defines options for gizmo types and scene interaction.
	 */
	public enum Tool
	{
		None,
		Position,
		Rotate,
		Scale,
		View
	}

	/**
	 * Describes different camera manipulation types.
	 */
	public enum ViewTool
	{
		None,	// Camera is not in control of anything
		Orbit,	// Camera is spherically rotating around target
		Pan,	// Camera is moving right or left
		Dolly,	// Camera is moving forward or backwards
		Look 	// Camera is looking and possibly flying 
	}

	public enum ElementMode
	{
		Vertex,
		Edge,
		Face
	}

	public enum EditLevel
	{
		Object,
		Element,
		View
	}

	/**
	 * Describes different culling options.
	 */
	public enum Culling
	{
		Back = 0x1,
		Front = 0x2,
		FrontBack = 0x4
	}

	public enum AssetType
	{
		Resource,
		Bundle,
		Instance
	}

	public enum PathType
	{
		Null,
		File,
		Directory
	}
}