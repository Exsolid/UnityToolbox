
/// <summary>
/// This namespace contains all audio related scripts.
/// </summary>
namespace UnityToolbox.Audio { }

/// <summary>
/// This namespace contains all features which can be used to enhance the gameplay.
/// </summary>
namespace UnityToolbox.GameplayFeatures { }

/// <summary>
/// The main namespace for item usage.
/// All items are defined via the Item Manager Window found under the "UnityToolbox" dropdown.
/// To use custome made items, create a new class inheriting <see cref="ItemDefinition"/>. The definition can now be selected within the window.
/// All non static, public int, float, string, bool will define editable fields.
/// Using the <see cref="ItemDefinition"/> as a variable in a seperate <see cref="MonoBehaviour"/> will present a selection field for all defined items.
/// At runtime, the <see cref="ItemManager"/> is able to create item instances.
/// </summary>
namespace UnityToolbox.GameplayFeatures.Items { }

/// <summary>
/// The namespace for everything related to item management. It is not built to be used within scripts.
/// </summary>
namespace UnityToolbox.GameplayFeatures.Items.Management { }

/// <summary>
/// The namespace for everything related to item management required for the editor. It is not built to be used within scripts.
/// </summary>
namespace UnityToolbox.GameplayFeatures.Items.Management.Editor { }

/// <summary>
/// The namespace for everything related to inventories.
/// </summary>
namespace UnityToolbox.GameplayFeatures.Items.Inventory { }

/// <summary>
/// The namespace for all inventory managers.
/// </summary>
namespace UnityToolbox.GameplayFeatures.Items.Inventory.Managers { }

/// <summary>
/// The namespace for all inventory related UI scripts.
/// </summary>
namespace UnityToolbox.GameplayFeatures.Items.Inventory.UI { }

/// <summary>
/// The namespace for all inventory types.
/// </summary>
namespace UnityToolbox.GameplayFeatures.Items.Inventory.Types { }

/// <summary>
/// All scripts related to the Achievement system.
/// </summary>
namespace UnityToolbox.GameplayFeatures.Achievements { }

/// <summary>
/// All scripts related to the Achievement system required for the editor.
/// </summary>
namespace UnityToolbox.GameplayFeatures.Achievements.Editor { }

/// <summary>
/// All AI related scripts.
/// </summary>
namespace UnityToolbox.GameplayFeatures.AI { }

/// <summary>
/// This namespace contains everything used for boids. (bird-oid objects/swarm intelligence)
/// </summary>
namespace UnityToolbox.GameplayFeatures.AI.Boids { }

/// <summary>
/// This namespace contains everything used for gamestates. 
/// All gamestates are defined via the Gamestate Manager Window found under the "UnityToolbox" dropdown.
/// </summary>
namespace UnityToolbox.GameplayFeatures.Gamestates { }

/// <summary>
/// This namespace contains everything used for gamestates on the editor side.
/// </summary>
namespace UnityToolbox.GameplayFeatures.Gamestates.Editor { }

/// <summary>
/// This namespace contains everything used for Procedural generation.
/// </summary>
namespace UnityToolbox.GameplayFeatures.ProceduralGeneration { }

/// <summary>
/// This namespace contains everything used for Procedural generation on the editor side.
/// </summary>
namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Editor { }

/// <summary>
/// This namespace contains all enums used for Procedural generation.
/// </summary>
namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Enums { }

/// <summary>
/// This namespace contains all scripts used for Procedural terrain generation.
/// </summary>
namespace UnityToolbox.GameplayFeatures.ProceduralGeneration.Terrain { }

/// <summary>
/// This namespace contains all scripts used for save games.
/// </summary>
namespace UnityToolbox.GameplayFeatures.SaveGame { }

/// <summary>
/// This namespace contains all scripts used for save games on the editor side.
/// </summary>
namespace UnityToolbox.GameplayFeatures.SaveGame.Editor { }

/// <summary>
/// This namespace contains all classes being used for serialising data.
/// </summary>
namespace UnityToolbox.GameplayFeatures.SerializationData { }

/// <summary>
/// All UI related scripts can be found here.
/// </summary>
namespace UnityToolbox.UI { }

/// <summary>
/// All scripts which are used to display and interact with game settings are found here.
/// </summary>
namespace UnityToolbox.UI.Settings { }

/// <summary>
/// All scripts which are used to define controls are found here.
/// </summary>
namespace UnityToolbox.UI.Settings.Controls { }

/// <summary>
/// All scripts which are used to define the gameplay language are found here. (also see <see cref="UnityToolbox.UI.Localization"/>.)
/// </summary>
namespace UnityToolbox.UI.Settings.Language { }

/// <summary>
/// This namespace includes all scripts that are used for silder based settings.
/// </summary>
namespace UnityToolbox.UI.Settings.Sliders { }

/// <summary>
/// This namespace includes all scripts that are used for general interactable menus.
/// </summary>
namespace UnityToolbox.UI.Menus { }

/// <summary>
/// This namespace includes all scripts that are used for a world space menu wheel.
/// </summary>
namespace UnityToolbox.UI.Menus.MenuWheel { }

/// <summary>
/// This namespace includes all scripts that are used for general flat interactable menus.
/// </summary>
namespace UnityToolbox.UI.Menus.FlatMenu { }

/// <summary>
/// This namespace includes all scripts that are used for on hover functionality menus.
/// </summary>
namespace UnityToolbox.UI.Menus.FlatMenu.OnHover { }

/// <summary>
/// All dialog related UI scripts can be found here.
/// </summary>
namespace UnityToolbox.UI.Dialog { }

/// <summary>
/// All dialog related UI scripts, which are required for the UI interaction can be found here.
/// </summary>
namespace UnityToolbox.UI.Dialog.UI { }

/// <summary>
/// All dialog related UI scripts, which are required for the editor can be found here.
/// </summary>
namespace UnityToolbox.UI.Dialog.Editor { }

/// <summary>
/// All Localization related UI scripts can be found here.
/// All Localizations are defined via the Localization Manager Window found under the "UnityToolbox" dropdown.
/// </summary>
namespace UnityToolbox.UI.Localization { }

/// <summary>
/// All Localization related UI scripts can be found here.
/// </summary>
namespace UnityToolbox.UI.Localization.Editor { }

/// <summary>
/// This namespace includes all scripts that are used for player controls, such as movement or interactions.
/// </summary>
namespace UnityToolbox.PlayerControls { }

/// <summary>
/// This namespace includes all scripts that are used for 2D player controls.
/// </summary>
namespace UnityToolbox.PlayerControls.TwoD { }

/// <summary>
/// This namespace includes all scripts that are used for 3D player controls.
/// </summary>
namespace UnityToolbox.PlayerControls.ThreeD { }

/// <summary>
/// This namespace includes all scripts that are used for UI interactions via controls.
/// </summary>
namespace UnityToolbox.PlayerControls.UI { }

/// <summary>
/// This namespace includes core implementations which are either the foundation of the toolbox or can be used in many cases.
/// </summary>
namespace UnityToolbox.General { }

/// <summary>
/// All general common algorithms can be found here.
/// </summary>
namespace UnityToolbox.General.Algorithms { }

/// <summary>
/// General attributes can be found here.
/// </summary>
namespace UnityToolbox.General.Attributes { }

/// <summary>
/// Scripts which make up the attributes on the editor side can be found here.
/// </summary>
namespace UnityToolbox.General.Attributes.Editor { }

/// <summary>
/// All scripts which make the foundation of management of the toolbox can be found here.
/// </summary>
namespace UnityToolbox.General.Management { }

/// <summary>
/// All scripts which are required for a master scene can be found here.
/// </summary>
namespace UnityToolbox.General.MasterScene { }

/// <summary>
/// All scripts which are required for a master scene on the editor side can be found here.
/// </summary>
namespace UnityToolbox.General.MasterScene.Editor { }

/// <summary>
/// All scripts which are required for preference usage can be found here.
/// </summary>
namespace UnityToolbox.General.Preferences { }

/// <summary>
/// This namespace includes all scripts that are used for subscription events.
/// </summary>
namespace UnityToolbox.General.SubEvents { }