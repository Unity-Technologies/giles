using UnityEngine;

namespace GILES
{
	/**
	 * pb_InputManager maintains a static multicast delegate `mouseUsedDelegate` that
	 * classes can register with to indicate that they own the mouse at that point, 
	 * preventing pb_ISceneEditor from sending mouse updates to inheriting classes.
	 *
	 * Ex: Menu class has a rect reserved for buttons.  Menu adds a function to test
	 * if mousePosition is within reserved rect and registers as a delegate with 
	 * pb_InputManager so that mouse actions are ignored by the scene controls when
	 * mouse is in the menu.
	 */
	public delegate bool MouseInUse(Vector2 mousePosition);


	/**
	 * pb_InputManager will check with any delegates registered to keyUsedDelegate
	 * before sending key events to pb_ISceneEditor classes.
	 *
	 * Ex: pb_SceneCamera needs to control the sceneview when the alt key is pressed
	 * with the mouse buttons down.  It registers with pb_InputManager and returns true
	 * when alt key is pressed and mouse is down.
	 */
	public delegate bool KeyInUse();

	/**
	 * A simple void callback function.
	 */
	public delegate void Callback();

	/**
	 * A generic delegate with one parameter.
	 */
	public delegate void Callback<T>(T value);

	/**
	 * A generic delegate with one parameter.
	 */
	public delegate void Callback<T, S>(T value0, S value1);
}