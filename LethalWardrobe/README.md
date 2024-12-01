# Lethal Wardrobe
### A refactoring of the More-Suits mod, with an in-built API.
This mod was designed to take the core functionality of More-Suits, and put it into a form that will allow for easier addition of new features. 

**Current Features:**
- An API that allows for the retreival of all currently loaded suit's key data (such as their ID, UnlockableObject Name and Material / Textures.)

**Planned features:**

- An independent persistence system, to ensure that the Lethal Company's vanilla unlockable object logic does not interfere with, or be interfered by, the mod's custom suits. 
- Allowing for in-game effects to be applied to those wearing specific suits. 
- An in-game terminal interface for buying suits (independent of the normal shop rotation)
## More-Suits Info
_For legacy purposes, please see below the original README.md for the More-Suits Mod._
### Instructions: 
After launching the game with the mod once, a config file is generated. In this file you can disable individual suits from being loaded, as well as ignore any ```!less-suits.txt``` file and attempt to load all suits (which is useful if you have another mod that helps manage lots of suits).

### Config File
After launching the game with the mod once, a config file is generated. In this file you can disable individual suits from being loaded, as well as ignore any ```!less-suits.txt``` file and attempt to load all suits (which is useful if you have another mod that helps manage lots of suits).

### Customize
You can add .png files to the ```moresuits``` folder to add new suits as long as both the host and clients have the same files.

### Advanced
You can add a .json file in the ```advanced``` folder with the same name as your .png file in the ```moresuits``` folder to enable additional features like emission. Place additional texture maps in the ```advanced``` folder.

For a list of supported features, see:
https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@14.0/manual/Lit-Shader.html

### Add Suits to Store
Add a "PRICE" key to your advanced .json to put a suit in the store rotation. See ```Glow.json``` for an example of adding a suit with emission that must be purchased from the store.

### Making your own More Suits mod
Upload your own package with a ```BepInEx/plugins/moresuits``` folder in it (do not include the MoreSuits.dll file) and add ```x753-More_Suits-1.4.0``` as a dependency, and this mod will automatically load your .png files as suits. If you don't want some or all of the suits that originally come with my mod, adjust the config file ```BepInEx\config\x753.More_Suits.cfg```. Include a ```!less-suits.txt``` file in your ```moresuits``` folder to disable all the default suits that come with this mod.

## Lethal Wardrobe Features:

### Modding API
Lethal Wardrobe comes with its own Modding api. Just ensure that you have the API as a dependancy in your project (not
the base mod) and place the following into your Plugin.cs awake method:
```csharp
LethalWardrobeApi.Instance.Initialize(this);
```

