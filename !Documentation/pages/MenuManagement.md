@page MenuManagement Menu Management Tutorial

@section MenuManagementOverview Overview

The namespace @ref UnityToolbox.UI includes everything that can be used within a canvas.\n
In the following the @ref UnityToolbox.UI.Menus.MenuManager "MenuManager" and each sub namespace of @ref UnityToolbox.UI will be explained.\n
The scripts found here are developed to ease the creation of menus and their structures.

The prerequisites for using this system are:

- @ref GeneralSetupManagerSystem.
- An EventSystem component somewhere in the scene.

@section MenuManagementMenuUsage Menu Usage

The menu system consists of two parts. The @ref UnityToolbox.UI.Menus.Menu "Menu" script which is placed on a canvas, and the @ref UnityToolbox.UI.Menus.MenuManager "MenuManager" which sorts them.\n
First let us create a main canvas, which can be set up whatever. The actual @ref UnityToolbox.UI.Menus.Menu "Menu" scripts are now placed on a child canvas as following:

| Menus |
| :---- |
| @image html MenuManagementMenu.png width=400px |

With the menus being created, one can now create the @ref UnityToolbox.UI.Menus.MenuManager "MenuManager" script.\n
First the names of the menu types need to be defined. With them in place, one can add the menu type to the menu list.\n
In this example, we have a pause menu, which consists of two separate canvases, the main pause menu and a settings menu. \n
One can also define an optional overlay. The overlay will always be opened, if no other menu is open. One can enable that the overlay to be hidden, if other menus are open.

| Menu Manager |
| :---- |
| @image html MenuManagementMenuManager.png width=400px |

The @ref UnityToolbox.UI.Menus.MenuManager "MenuManager" can now be used to toggle all defined menus.\n

@section MenuManagementButtons Button Usage

Apart from the actual methods @ref UnityToolbox.UI.Menus.MenuManager.SetActiveMenu "MenuManager.SetActiveMenu(..)" and @ref UnityToolbox.UI.Menus.MenuManager.ToggleMenu "MenuManager.ToggleMenu(..)" the toolbox provides button scripts for switching between menus.\n
For them and the other button scripts to work, one is required to have an EventSystem component in the scene. Create one as following:

| Event System |
| :---- |
| @image html MenuManagementEventSystem.png width=400px |

If the current Input Module which comes with the object says to replace it with a different one, do that. It should look like the following:

| New Input Module |
| :---- |
| @image html MenuManagementInputModule.png width=400px |

@subsection MenuManagementSwitchingMenus Switching Menus

First, let us look at two scripts for switching between menus.

The @ref UnityToolbox.UI.Menus.FlatMenu.ToggleMenuButton "ToggleMenuButton" is used to open and close menus by clicking a UI element on a canvas.\n
This script can only toggle a menu, if the current menu is of the same menu type as the one which should be toggled. If no menu or the overlay is currently open, the menu will displayed.\n
If the menu to be opened and the current open menu are the same, the menu will be closed.\n
The menu to be opened can be defined by a drop down. An @ref UnityToolbox.Audio.AudioMixer "AudioMixer" can also be added for click sounds.

| Toggle Button |
| :---- |
| @image html MenuManagementToggle.png width=400px |

The other script @ref UnityToolbox.UI.Menus.FlatMenu.GotoMenuButton "GotoMenuButton" is similar, but requires the actual @ref UnityToolbox.UI.Menus.Menu "Menu" object and ignores the hierarchy restrictions.\n
Although the restrictions are ignored, the current menu type will still be set to the respective type of the menu, defined within the manager.

@subsection MenuManagementOtherMenuScripts Other Menu Scripts

There are other basic buttons, which are self explanatory. The code documentation will also give a brief description.

- The @ref UnityToolbox.UI.Menus.FlatMenu.QuitButton "QuitButton" script closes the application.
- The @ref UnityToolbox.UI.Menus.FlatMenu.GotoSceneButton "GotoSceneButton" script switches scenes.
- The @ref UnityToolbox.UI.Menus.FlatMenu.ResetSavegameButton "ResetSavegameButton" script resets the save game. (A SaveGameManager needs to be setup for this)

Other than this, the @ref UnityToolbox.UI.Menus.FlatMenu.LockMovement "LockMovement" can lock movement for all @ref UnityToolbox.PlayerControls.MovementBase "MovementBase" implementations.