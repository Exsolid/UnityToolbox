@page Tutorials Tutorials

@section TutorialsFeatureOverview Feature Overview

Currently the toolbox includes:
- UI Management (Hierarchies, useful UI scripts)
- Localization system
- Dialog system
- Savegame management (Checkpoint data and saving of objects at runtime)
- Gamestate management
- Procedural terrain generation
- Player controls for 2D and 3D
- Settings management (Controls, volume and languages)
- Sound management
- Inventory system **Requires rework**
- Achievement system
- Event management

A few scripts underlay a naming convention. These are:
- ...Interaction.cs for player triggered interactions with GameObjects.
- ...Button.cs for any UI related buttons.
- ...Control.cs for any input related scripts, that execute on a pressed key.
- The context of the script is always within the name.

The documentation excludes private members and EditorWindow implementations.\n
**Currently the toolbox is not in a stable version and requires polishing, which is currently being worked on.**\n
**The same goes for the tutorials and non code related documentation.**

@section TutorialsTutorialOverview Tutorial Overview

Currently tutorials for the following features exist (with some having further sub pages):
- @subpage ItemManagement
- @subpage MenuManagement
- @subpage LocalizationManagement
- @subpage GamestateManagement
- @subpage AudioManagement
- @subpage Boids
- @subpage TerrainGeneration