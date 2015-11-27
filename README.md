# Cardboard Controls+

Cardboard Controls+ is all you need to develop the best Cardboard games in Unity. These scripts enhance [Google's official Cardboard SDK for Unity](https://developers.google.com/cardboard/unity/) with improvements such as discrete magnet control, raycast data, and event-driven architecture. Stop limiting your creative options!

[![Join the chat at https://gitter.im/JScott/cardboard-controls](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/JScott/cardboard-controls?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## Usage

- Import [the latest unitypackage](https://github.com/JScott/cardboard-controls/releases/latest)
- Add the `CardboardControlManager` prefab to the root of your scene
- Use the [API](https://github.com/JScott/cardboard-controls/wiki/API) which is explained in [the DemoScene code](https://github.com/JScott/CardboardSDK-Unity/blob/master/CardboardControl/DemoScene/ExampleCharacterController.cs).

You may experience problems if you import the code manually without using the provided unitypackage file.

## What's added?

Google's SDK only has one hook into the trigger: `bool CardboardTriggered`. It's available for one frame that you have to waste cycles polling for. Instead of limiting yourself to that, you can now utilize C# methods and delegates for:

- Trigger down
- Trigger up
- Trigger click
- Boolean if the trigger is held
- Seconds trigger held for

This expands your opportunities, allowing interactions like holding the magnet to move and clicking to interact to provide a more natural experience.

There are also methods and delegates for:

- Tilting the device, as used in the official Cardboard app to navigate menus
- Gaze raycasting, capturing what you look at and for how long

These helpers let you focus on what makes your game cool instead of boilerplate code.

## Prerequisites

If you got this from the Asset Store then I've already packaged a version of the prerequisites for you.

If not then you'll need to import the base Cardboard SDK if you haven't already:
- [Download CardboardSDK.unitypackage](https://github.com/googlesamples/cardboard-unity/blob/master/CardboardSDKForUnity.unitypackage?raw=true) from [Google's repository](https://github.com/googlesamples/cardboard-unity).
- Import it in your scene
- Use the `CardboardMain` prefab for your camera

If you're having any trouble getting Google's Cardboard SDK set up, read through their excellent [guide for getting started](https://developers.google.com/cardboard/unity/get-started). Cardboard Controls+ has been tested against v0.5.2.
