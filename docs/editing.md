# Engine vs content
Unity-based gameplay code should define only aspects unlikely to change or requiring high performance. Because of open and web based nature of the project, commonly changed and content-related scripts have to be editable from web browser. Prototyping, testing and publishing changed to them cannot require usage of local native build system. They include:
- characters,
- items,
- quests, 
- dialogues, 
- object interactions,
- gui elements.
There is currently no way to interpret C# code on the browser, and its compilation to WebAssembly requires complex properiarty build system. Because of this, mentioned scripts have to use another language which can either run in browser or which can be effectively run on top of C# compiled to WebAssembly. 

# JavaScript
JavaScript is trivial solution to this problem, especially taking into account that it can be compiled and ran directly from the browser thus suffering no significant performance penalty compared to interpreted approaches. Tooling is already ubiquitous and glue layer to engine api can be easily generated even at runtime using reflection.
In this approach, all content scripts are to be loaded as files from browser level in parallel with game engine. This didn't necessarily involve putting their entries directly in html. code - but at least some script based entry point listing their paths will be required. One solution would be to put there only scripts containing basic entry points from the engine, and from them register the rest. This way, many of them can be loaded lazily on background, speeding up initial load sequence. This makes moving there as much code as possible even more beneficial, as loading even empty Unity project takes at least few seconds on decent pc and it will scale up with amount of C# code.

# Meshes
Similarly to the scripts, meshes are another kind of content that has to be editable without running local build system and instead loaded lazily at runtime from the network. One of the most commonly supported 3d mesh formats is .obj, and there are many free runtime obj loaders for Unity.
To avoid need for dedicated textures for each mesh (which costs both additional work and lot of additional network traffic) and manual UV mapping (which again needs work and native standalone tooling) meshes will mostly use generic seamless shared textures and procedurally generated UV maps based on geometry.

Textures
Because game is going to be steamed from the network, it's important to avoid heavy resource files, and reasonably detailed textures generally fall into this category. Using vector graphics could partially stove this problem, but restrictions they impose make them unsuitable for visual style of the game. Moreover, they would anyway have to be rasterized locally with possibly significant resource usage. JPG compression seems to be rational compromise - may be efficiently decoded in the host browser and then passed as 2d array of pixels to the engine. Compression is lossy, but resulting file sizes are rather small. 




