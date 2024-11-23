# Lethal Wardrobe API
### A modding API for Lethal Wardrobe, with an in-built API.

### Usage Instructions
Just ensure that you have the API as a dependancy in your project (not
the base mod) and place the following into your Plugin.cs awake method:
```csharp
LethalWardrobeApi.Instance.Initialize(this);
```
The API allows for:
- The Retrieval of all suits key data (Name, Material, id ,etc.)
- The Retrieval of suit data, based off suit id.
- [MORE FEATURES PLANNED]
