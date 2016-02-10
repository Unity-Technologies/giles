# Quick Start Tutorial

This guide will take you through the basic steps involved in getting **GILES** up and running in your project.

### Install

The first step to integrating **GILES** is to import the `giles.unitypackage` file into your Unity project.  If you have installed **GILES** from the Asset Store, this step should be taken care of for you.  If you've downloaded **GILES** from Github or the User Toolbox, you'll need to open your Unity project and drag the `.unitypackage` into your **Projet** pane.  Make sure that all the assets are selected in the Package Import window.

![Importing GILES](import.png)

### Set the .NET Profile

If after importing **GILES** there are now errors in the Console, don't fret.  This just means that your Api Compatibility Level is set to **.NET 2.0 Subset**.

Open *Edit > Project Settings > Player* and in the Inspector change **Api Compatibility Level** to **.NET 2.0**.

### Add the Level Editor

Next you'll need to make the **Level Editor** scene available for users to access.  For this tutorial we'll just open the Level Editor to a new level every time, though it is possible to set up a modal window to select an existing level to edit.  That scenario will be covered in a later tutorial (plus the default Level Editor scene includes a menu item to open a file browser).

The process is just like adding any other scene to your project:

1. Open *File / Build Settings*.
1. Add *GILES / Example / Level Editor* to the **Scenes In Build** list.

Now in your menu scene simply open the "Level Editor" scene.  Check out *GILES / Example / Main Menu* for a demonstration of this (specifically, in the hierarchy *Canvas/Panel_Background/Buttons/Btn_LoadMapEditor*).

```
// Unity 5.3 or greater
SceneManager.LoadScene("Level Editor");

// Or in Unity 5.2
Application.LoadLevel("Level Editor");
```

That's it!  The Level Editor is now accessible and ready to start building new levels.  The next step is to give the player some blocks to build with.

### Adding Resources

Now that the Level Editor is accessible and running, you'll need some building blocks to populate those levels. 

**GILES** provides 2 ways of making resoures available to the player.  The first (and easiest) method is to use the **Resources / Level Editor Prefabs** folder.

1. Create a new folder named **Resources**.
1. Inside that folder, add a new folder named **Level Editor Prefabs**.
1. Drag a prefab into the **Resources/Level Editor Prefabs** folder.  **GILES** will automatically recognize this as a new resource and tag it with a `pb_MetaDataComponent`.

![Resources / Level Editor Prefabs](resources_leveleditorprefabs.png)

If there is not a `pb_MetaDataComponent` on the prefab, make sure that the folder names are correct (capitalization matters) and that the object is a prefab (Assets are not allowed).
