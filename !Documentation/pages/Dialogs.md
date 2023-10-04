@page Dialog Dialog

@section DialogOverview Overview

- @ref DialogCreation
	- @ref DialogLocalizedNodes
	- @ref DialogDefaultNodes
- @ref DialogSetup
	- @ref DialogSetupButtons
	
This feature adds a dialog node editor and scripts for their usage. 
The prerequisites for using this system are:

- The @ref GeneralSetupManagerSystem.
- The @ref MenuManagementMenuSetup

@section DialogCreation Dialog Creation

Like the other editor tools, the dialog graph can be found within the toolbar, under "UnityToolbox" -> "Dialog Graph":

| Dialog Graph Button |
| :---- |
| @image html DialogMenuButton.png width=200px |

When opening up the graph for the first time, one will be asked to input a directory. This directory must be within a "Resources" folder.\n
The directory is required for all data to be saved and included within the build.\n
When changing the directory via the "Directory Settings" button, data will be read from there and the view will be refreshed.\n
If the data within the old directory is required, one will need to manually copy the file to the new directory for it to be read.

| Dialog Graph Directory Setup |
| :---- |
| @image html DialogPathSetup.png width=500px |

@subsection DialogDefaultNodes Default Nodes

There are now two nodes which can be used. The default node, which can be created with the right click drop down and the root node, which can be created by converting the default node with the "To Root" button.\n
The Root node only differs by the input parameters used. They are not called by another node, but by an ID and an optional gamestate.\n
An ID can be used for the later mentioned scripts, to refer to a specific dialog. Therefore they must be unique and will be automatically renamed otherwise.\n


The gamestate field defines, at which gamestate the dialog should be called. If left empty, the dialog will always be called.\n
Multiple equal IDs are allowed if they are in combination with different gamestates.\n
For detailed usage of the gamestate management, please refer to the @ref GamestateManagement page.

| Root Node |
| :---- |
| @image html DialogRootNode.png width=300px |

The following fields are values used to be displayed within the UI. The only exception is the "Gamestate To Complete", which completes a given gamestate once the dialog node is called. \n
The "Avatar" is the texture used to display within the UI. This texture must also be stored within a "Resources" folder.\n
The "Dialog Text" is the description. The title is the first field found at the top of the node.\n
Lastly, the options are directly linked to the amount of next nodes. If only one next node is present, these fields are optional.\n
Multiple next nodes require a dialog option field for each node. These fields define the text for the choice buttons, which are displayed within the UI.\n

| Node Connections |
| :---- |
| @image html DialogNodeConnection.png width=500px |

@subsection DialogLocalizedNodes Localized Nodes

The localized nodes work similar to the default nodes, with the key difference that @ref UnityToolbox.UI.Localization.LocalizationID "LocalizationIDs" are used to define the text values.

| Localized Node |
| :---- |
| @image html DialogLocalizationNode.png width=200px |

For detailed usage of the localization tool, please refer to the @ref LocalizationManagement page.

@section DialogSetup Dialog Setup

The first step, based on the @ref MenuManagementMenuSetup, is to set up a canvas and register it as a @ref UnityToolbox.UI.Menus.Menu "Menu" within the @ref UnityToolbox.UI.Menus.MenuManager "MenuManager".\n
Secondly, two scripts will be required. The @ref UnityToolbox.UI.Dialog.DialogManager "DialogManager" which manages all created data by the dialog graph \n
and the @ref UnityToolbox.UI.Dialog.DisplayDialog "DisplayDialog" script, which manages all UI related elements.

| DialogManager & DisplayDialog |
| :---- |
| @image html DialogManager.png width=400px |

All values apart from the "Menu Type" and "Menu Of Type" selection are UI elements which are used within the dialog canvas. All values are optional.\n
The "Sprite To Show" is similar to the "Avatar" texture within the dialog graph, with the difference, that it is shown permanently and does not switch if the node changes.\n

@subsection DialogSetupButtons Dialog Buttons

For the dialog to be intractable, two scripts are to be used. The @ref UnityToolbox.UI.Dialog.UI.NextDialogButton "NextDialogButton", which is the button that is displayed if no options are registered for the node.\n
The button will always lead to the first next node and close the dialog if no node is found. An @ref UnityToolbox.Audio.AudioMixer "AudioMixer" can optionally be added to play on click.\n
The other script is the @ref UnityToolbox.UI.Dialog.UI.NextDialogOptionButton "NextDialogOptionButton", which also has an optional @ref UnityToolbox.Audio.AudioMixer "AudioMixer" field and an additional field "Option ID" field.\n
This field refers to the "Is Option: x" value within the node and is required to display the correct option text and next node once clicked. It will also close the dialog menu if no next node is found.\n


For further details of the audio management, please refer to the @ref AudioManagement page.