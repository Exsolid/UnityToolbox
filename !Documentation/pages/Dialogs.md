@page Dialog Dialog

@section DialogOverview Overview

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

| Dialog Graph Directory Setup |
| :---- |
| @image html DialogPathSetup.png width=500px |

There are now two nodes which can be used. The default node, which can be created with the right click drop down and the root node, which can be created by converting the default node with the "To Root" button.\n
The Root node only differs by the input parameters used. They are not called by another node, but by an ID and an optional gamestate.\n
An ID can be used for the later mentioned scripts, to refer to a specific dialog. Therefore they must be unique and will be automatically renamed otherwise.\n
The gamestate field defines, at which gamestate the dialog should be called. If left empty, the dialog will always be called.\n
Multiple equal IDs are allowed if they are in combination with different gamestates.

| Root Node |
| :---- |
| @image html DialogRootNode.png width=300px |

The following fields are values used to be displayed within the UI. The only exception is the "Gamestate To Complete", which completes a given gamestate once the dialog node is called. \n
The "Avatar" is the texture used to display within the UI. This texture must also be stored within a "Resources" folder.\n
The "Dialog Text" is the description. The title is the first field found at the top of the node.\n
Lastly, the options are directly linked to the amount of next nodes. If only one next node is present, these fields are optional.\n
Multiple next nodes require a dialog option field for each node. These fields are choice/answer buttons which are displayed within the UI.\n

Looking at the following example, if "0" is clicked the node "Is Option: 0" will be called next.\n
If "1" is clicked the node "Is Option: 1" will be called next. Having no "Dialog Options" will result in a default button to be displayed.

| Node Connections |
| :---- |
| @image html DialogNodeConnection.png width=500px |

@section DialogSetup Dialog Setup