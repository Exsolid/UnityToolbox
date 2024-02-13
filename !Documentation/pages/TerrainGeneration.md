@page TerrainGeneration Terrain Generation

@image html TerrainGenerationExample.png width=1000px 

@section TerrainGenerationOverview Overview

- @ref TerrainGenerationGeneral
- @ref TerrainGenerationSettings
	- @ref TerrainGenerationGeneralHeightData
- @ref TerrainGenerationTypes
	- @ref TerrainGenerationCA
- @ref TerrainGenerationMeshTypes
	- @ref TerrainGenerationLayered

The prerequisites for using this system are:
- The @ref GeneralSetupManagerSystem.

@section TerrainGenerationSettings Settings

To use the terrain generation, one can open the Terrain Generation window via the menu bar "UnityToolbox" -> "Terrain Generation":

| Menu Bar |
| :---- |
| @image html TerrainGenerationMenu.png width=200px |

Here you will be prompted to enter a directory, which will be used to store the required data.\n
This directory must be within a "Resource" folder and can be changed via the **Settings** tab:

| Settings Tab |
| :---- |
| @image html TerrainGenerationSettings.png width=500px |

@section TerrainGenerationGeneral General Usage

The terrain generation is built to combine different procedural generation algorithms with different mesh generation algorithms.\n
The generation tab contains all general information of the created generator. Here selected generators can be deleted with **-** and added with **+**.\n
To change the name of a generator, use the same text field and press **Save/Rename** at the bottom of the window.\n


Additionally, the @ref TerrainGenerationTypes can be set to define the basis of the generation.\n
The settings for the generation type will then appear below.

| Generation Tab |
| :---- |
| @image html TerrainGenerationGenTab.png width=500px |

The mesh tab offers a selection of @ref TerrainGenerationMeshTypes. The settings of the selected type will then also appear below.\n
The generation settings can be tested beforehand with the **Generate Example With Static Size** button. \n
As the name suggests, the any size related values are static account.
 
| Mesh Tab |
| :---- |
| @image html TerrainGenerationMeshTab.png width=500px |

When creating terrain within the editor, the shader data is serialized to the set "Resources" folder.\n
If it is created at runtime, no data will be saved. Therefore, if the scene is reloaded and no data is created beforehand, the terrain will be displayed incorrectly.


To create terrain, the script @ref UnityToolbox.GameplayFeatures.ProceduralGeneration.Terrain.TerrainGeneration "Terrain Generation" must be added to a game object.\n
It requires a material, which runs on the **TerrainShader**. The following texture data is explained under following sub section.\n

| Terrain Generation Script |
| :---- |
| @image html TerrainGenerationScript.png width=300px |

The terrain object parents the created game objects. Setting the ground layer is required for the asset placement to work.\n
Now the **Generate Terrain** button can be clicked, or @ref UnityToolbox.GameplayFeatures.ProceduralGeneration.Terrain.TerrainGeneration "TerrainGeneration.GenerateTerrain()" can be executed to create the terrain with set data.

@subsection TerrainGenerationGeneralHeightData Height Data

Lastly, each mesh generation type contains settings to define height data. This data contains information on how the terrain shader should render the mesh.\n
The layers are adjusted by height and have various settings for the texturing.\n

| Height Data Window |
| :---- |
| @image html TerrainGenerationHeightData.png width=500px |

All textured used here must also be saved within a resources folder. Once deleted or moved outside of a resources folder, an error window will pop up, notifying you of the missing data.\n
The error window can only be closed once data is reestablished and saved.

| Error Window |
| :---- |
| @image html TerrainGenerationErrorWindow.png width=500px |

Additionally these textures must be set to enable **Read/Write** and have their **Max Size** equal to the size set within the @ref UnityToolbox.GameplayFeatures.ProceduralGeneration.Terrain.TerrainGeneration "Terrain Generation" script.\n
A low texture size value is recommended if many textures are used, as the shader might run out of memory, resulting in wrong looking terrain otherwise.

| Texture Settings |
| :---- |
| @image html TerrainGenerationScriptTextureSettings.png width=300px |
| @image html TerrainGenerationScriptTexture.png width=300px |

The texture format can mostly be set to anything corresponding to the texture, but it is recommended to use the format **RGBA32**.\n

@section TerrainGenerationTypes Procedural Generation Types

Currently the following generation types exist. They define the base structure of the then generated mesh.

@subsection TerrainGenerationCA Cellular Automata

The [Cellular Automata](https://en.wikipedia.org/wiki/Cellular_automaton) defines a set of rules for each cell within the grid.\n
The rules can be adjusted with the following values.

| Cellular Automata Example Values |
| :---- |
| @image html TerrainGenerationCA.png width=500px |

The **Iteration Count** defines how often the algorithm should run through the grid with defined **Size for X & Y**.\n
Initially the grid is filled with random values, dependent on the **Fill Percentage**.

@section TerrainGenerationMeshTypes Mesh Generation Types

Currently the following mesh generation types exist. They generate on basis of the grid defined beforehand.\n

@subsection TerrainGenerationLayered Layered Mesh Generation

The layered mesh generation uses the floor layer of the generated grid as a basis. The wall values will be stacked with layers until either no space or layer is left.\n
Different layers can be chosen to define how the layers are generated, although currently only a default ground layer can be selected.\n
Each layer defines height data for the terrain shader, and asset data, clustered and single. Additionally general settings can be found for the generation type and layers.

| Layered Header |
| :---- |
| @image html TerrainGenerationLayeredHeader.png width=500px |

The **Size Between Vertices** defines how much space should be between each generated vertex. Higher values result in a more stretched out mesh.\n
The **Vertex Multiplier** adds filler values between each generated value by the grid generation. Higher values result in a more defined mesh.\n
The **Noise For Asset Position** adds randomness to the asset placement. The randomness cannot exceed above or below the other vertices.
Other than the **Enable Asset Placement** option, which is useful to adjust the mesh itself without removing asset placement settings, the other settings are options for the height data.\n
These options are usually within the height data itself, but are generalized for the layered generation.\n
Below layers can be added and defined.

| Layered Layer |
| :---- |
| @image html TerrainGenerationLayeredLayerHeader.png width=500px |

The **Cliff Height** defines how high the layer should be, calculated from the last layer. The Base Layer cannot be changed.\n
The **Noise Ground** value defines how much noise the ground should have. Higher values result in rougher terrain.\n
The **Height Colors** define the height data for the layer. Although it is set for the layer, the blend factor will blend into the layer below.\n
Additionally setting the first height layer to a non zero starting height will lead to the previous height data layer to go above its own ground layer.\n


Lastly the assets can be set. There are two different kinds of asset spawns. The default **Single** asset placement, which places an asset for each vertex depending on the odds, and the **Clustered** asset placement.\n
The last will also place **Single** assets going for its original position and spreading out, resulting in a cluster of assets.\n
The **Percentage For No Asset** defines whether an asset should be selected at all for a given vertex.

| Single Asset |
| :---- |
| @image html TerrainGenerationLayeredSingleAsset.png width=500px |

The **Single** asset contains the prefab which will be placed.\n
If **Check For Space On Placement** is enabled the asset will only be placed if no other asset with the same setting is present.\n
If **Disable Raycast Placement** is enabled, then the asset will simply be placed with its offset. \n
The raycast placement places the smaller asset with rotation based on the ground levels around it and otherwise on the lowest level.\n
Additionally, if the prefab contains children, all children will be parented to the mesh, disbanding the prefab structure.\n
The **Pre Iterate** option will place assets in an earlier iteration of the asset placement. This is useful for ground coverage like grass. \n
The **Height Offset** will place the asset with an offset. \n
The **Position on Layer** will define whether the asset will be placed on the ground or cliff side of a layer.\n 
The **Odds For Spawn** will adjust the odds for the spawn. These odds are not percentages.\n 
The selection of a asset will depend on the **Position on Layer** of each asset. If only one asset is set to spawn on cliffs, this asset will be selected 100% of the time, if a selection takes place.\n
If only one asset is set to **Pre Iterate**, this asset will also be selected 100% of the time, if a selection takes place.\n 
The odds for each case, will be added up. Two ground placed assets with the odds 1 and 6, will result in the odds 1/7 and 6/7.\n
If the odds need to be toned down for only one case, instead of for all via the **Percentage For No Asset**, then a **Single** asset placement can be created containing an empty prefab.\n

| Empty Asset |
| :---- |
| @image html TerrainGenerationLayeredEmptyAsset.png width=500px |

The **Clustered** asset placement can contain multiple **Single** assets, which are placed in a cluster.\n
The **Odds for Spawn** of the contained assets will only be internal to the cluster.\n
If a cluster is selected, at least one asset of the cluster will always be spawned.\n
For the surrounding vertices, the odds are reduced by the **Spawn Percentage Decay**. This is repeated until all odds are gone.

| Clustered Asset |
| :---- |
| @image html TerrainGenerationLayeredClusteredAsset.png width=500px |

One must be warned, as this placement is recursive, that if the placement odds are not adjusted or high in general, the script will run a long time.\n
Having only one **Clustered** asset placement with a **Percentage For No Asset** of 0 and mesh size of 100 vertices on X & Y, will lead to a runtime of (100 * 100)^x, with x being dependent on the **Spawn Percentage Decay**!  