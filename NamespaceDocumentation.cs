
using UnityToolbox.Events;
using UnityToolbox.Item;

/// <summary>
/// The main namespace for item usage.
/// All items are defined via the Item Manager Window found under the "UnityToolbox" dropdown.
/// To use custome made items, create a new class inheriting <see cref="ItemDefinition"/>. The definition can now be selected within the window.
/// All non static, public int, float, string, bool will define editable fields.
/// Using the <see cref="ItemDefinition"/> as a variable in a seperate <see cref="MonoBehaviour"/> will present a selection field for all defined items.
/// At runtime, the <see cref="ItemManager"/> is able to create item instances.
/// </summary>
namespace UnityToolbox.Item { }

/// <summary>
/// The namespace for everything related to item management. It is not built to be used within scripts.
/// </summary>
namespace UnityToolbox.Item.Management { }

/// <summary>
/// All event related scripts are found here. The heart module would be <see cref="EventAggregator"/>.
/// </summary>
namespace UnityToolbox.Events { }

/// <summary>
/// All scripts related to the achievment system.
/// </summary>
namespace UnityToolbox.Achievments { }

/// <summary>
/// All AI related scripts.
/// </summary>
namespace UnityToolbox.AI { }

/// <summary>
/// This namespace contains everything used for boids. (bird-oid objects/swarm intelligence)
/// </summary>
namespace UnityToolbox.AI.Boids { }