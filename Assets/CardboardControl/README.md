# Cardboard Controls+

## Usage

- Remove existing Cardboard Controls+ files to ensure that they get updated properly
- Import [the latest unitypackage](https://github.com/JScott/cardboard-controls/releases/latest)
- Add `CardboardControl/Prefabs/CardboardControlManager` and `CardboardControl/Prefabs/Player` to the root of your scene
- Use the [API](API.md) which is explained in [the Demo Scene code comments](DemoScenes/Scripts/ExampleCharacterController.cs).

You may experience problems if you import the code manually without using the provided unitypackage file.

## What's added?

Cardboard Controls+ adds vital functionality to the barebones Cardboard SDK that expands your options and makes common things easy. There are C# methods and delegates for:

- Treating trigger down and up as separate events from a click
- Identifying the objects players are looking or staring at
- Tilting the device, as used in official Cardboard apps to navigate menus
- Toggling and highlighting a reticle in the middle of the player's view
- Simple prefabs to quickly get you using GoogleVR

These expand your opportunities and save you from reimplementing boilerplate functionality. Instead of wasting time on that, Cardboard Controls+ helps you to focus on what makes your game cool.
