# UnityToolbox
A toolbox with scripts that can be used in unity.
Currently this includes:
- Menu management (Hierarchies, useful UI scripts)
- Localisation system
- Dialog system
- Inventory system (To be reworked soon)
- Savegame management (Checkpoint data and saving of objects at runtime)
- Procedual terrain generation
- Player controls for 2D and 3D
- Settings management (Controls, volume and languages)
- Sound management

The documentation for the inventory system will be added once reworked.

A few scripts underlay a naming convetion. These are:
- ...Interaction.cs for player triggered interactions with GameObjects.
- ...Button.cs for any UI related buttons.
- ...Control.cs for any input related scripts, that execute on a pressed key.

The documentation excludes private members and EditorWindow implementations.