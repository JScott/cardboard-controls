# Cardboard Controls+

Cardboard Controls+ is all you need to develop the best Cardboard games in Unity. These scripts enhance [Google's official Cardboard SDK for Unity](https://developers.google.com/cardboard/unity/) with improvements such as discrete magnet control, raycast data, and event-driven architecture. Stop limiting your creative options!

[![Join the chat at https://gitter.im/JScott/cardboard-controls](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/JScott/cardboard-controls?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## Usage

- Import [the latest unitypackage](https://github.com/JScott/cardboard-controls/releases/latest)
- Add the `CardboardControlManager` prefab to the root of your scene
- Use the [API](API.md) which is explained in [the DemoScene's code comments](CardboardControl/DemoScene/ExampleCharacterController.cs).

You may experience problems if you import the code manually without using the provided unitypackage file.

## What's added?

Cardboard Controls+ adds vital functionality to the barebones Cardboard SDK that expands your options and makes common things easy. There are C# methods and delegates for:

- Treating trigger down and up as separate events from a click
- Identifying the objects players are looking or staring at
- Tilting the device, as used in official Cardboard apps to navigate menus
- Toggling and highlighting a reticle in the middle of the player's view

These expand your opportunities and save you from reimplementing boilerplate functionality. Instead of wasting time on that, Cardboard Controls+ helps you to focus on what makes your game cool.
