# giles
### A Runtime Level Editor for Unity3D

![giles](giles.png)

## What is it?

GILES is a runtime level editor for Unity games.  It is designed to be completely functional on it's own, but open to extensibility at every opportunity.

Out of the box here's what GILES provides:

- Selection manager
- Grid snapping
- Translate, rotate, and scale handles
- Scene save / load
	- Levels written to human-readable JSON.
	- Saves all objects in scene via reflection, no additional code required.
	- Writes only state deltas if prefabs are used.
	- Serialization process is customizable with both simple attributes or complete overloading.
- Undo/redo.

## Quick Start

- Install **Unity 5.2** or greater.
- Open **GILES** project.
- Open *GILES/Example/Level Editor*

## Contributing

Bug reports should be submitted to the Issues queue on Github.  Feature requests should be either posted on the [forums](http://www.protoolsforunity3d.com/forum/) or contributed via pull request.

## License

See [Unity Asset Store EULA](https://unity3d.com/legal/as_terms).

In short - you are free to ship your game using GILES, but do not distribute GILES as source code.

When GILES moves out of beta it will become a paid product.  You are however free to continue using GILES at it's current state (eg, when GILES becomes a paid product we won't revoke the license to use the version you're currently using; it simply means updates will no longer be free).
