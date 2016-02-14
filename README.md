# Cardboard Controls+

Cardboard Controls+ is all you need to develop the best Cardboard games in Unity. These scripts enhance [Google's official Cardboard SDK for Unity](https://developers.google.com/cardboard/unity/) with improvements such as discrete magnet control, raycast data, and event-driven architecture. Stop limiting your creative options!

[![Join the chat at https://gitter.im/JScott/cardboard-controls](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/JScott/cardboard-controls?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## Usage

- Remove existing Cardboard Controls+ files to ensure that they get updated properly
- Import [the latest unitypackage](https://github.com/JScott/cardboard-controls/releases/latest)
- Add `CardboardControl/Prefabs/CardboardControlManager` to the root of your scene
- Replace your Camera with `Cardboard/Prefabs/CardboardMain`
- Use the [API](API.md) which is explained in [the Demo Scene code comments](CardboardControl/DemoScenes/Scripts/ExampleCharacterController.cs).

You may experience problems if you import the code manually without using the provided unitypackage file.

## What's added?

Cardboard Controls+ adds vital functionality to the barebones Cardboard SDK that expands your options and makes common things easy. There are C# methods and delegates for:

- Treating trigger down and up as separate events from a click
- Identifying the objects players are looking or staring at
- Tilting the device, as used in official Cardboard apps to navigate menus
- Toggling and highlighting a reticle in the middle of the player's view

These expand your opportunities and save you from reimplementing boilerplate functionality. Instead of wasting time on that, Cardboard Controls+ helps you to focus on what makes your game cool.

## [Support](http://u3d.as/aeV)

If you like Cardboard Controls+ and want to show your support, [please check out the Asset Store page](http://u3d.as/aeV). It's an easy way to show your appreciation and get stable updates with Asset Store integration. I also love it when you leave a rating or review because it helps other people find the project.
