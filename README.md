# UnityToolbox
A toolbox with scripts that can be used in unity.
Currently this includes:
- UI Management (Hierarchies, useful UI scripts) | See MenuManager
- Localisation system | See Localizer and the toolbar of Unity
- Dialog system | See DialogManager and the toolbar of Unity
- Savegame management (Checkpoint data and saving of objects at runtime) | See SaveGameManager
- Gamestate management | See GamestateManager
- Procedual terrain generation | See TerrainGenerator
- Player controls for 2D and 3D | See MovementBase
- Settings management (Controls, volume and languages) | See SettingsManager
- Sound management | See AudioMixer
- Inventory system (To be reworked soon)

The documentation for the inventory system will be added once reworked.

A few scripts underlay a naming convetion. These are:
- ...Interaction.cs for player triggered interactions with GameObjects.
- ...Button.cs for any UI related buttons.
- ...Control.cs for any input related scripts, that execute on a pressed key.

The documentation excludes private members and EditorWindow implementations.

Dokumentation link: https://exsolid.github.io/UnityToolbox/
