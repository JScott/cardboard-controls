This Cardboard SDK is all you need to develop the best Cardboard games in Unity. These scripts enhance [Google's official Cardboard SDK for Unity](https://developers.google.com/cardboard/unity/) with improvements such as discrete magnet control, orientation tilting, and event-driven architecture. Stop limiting your creative options!


# What's added?

In addition to everything from Google's impressive SDK, you get better interactions to improve the experience for developers and players alike.

## Discrete Magnet Control

Google's SDK only has one hook into the magnet: `bool CardboardTriggered`. It's available for one frame that you have to waste cycles polling for.

Now you can utilize C# Delegates to expose:

- Magnet down
- Magnet up
- Magnet click
- Boolean if the magnet is held
- Seconds magnet held for

This expands your opportunities, allowing interactions like holding the magnet to move and clicking to interact to provide a more natural experience.

## Standard Gestures

Google's SDK has nothing for the gesture of tilting the Cardboard vertically. This is used in the official Cardboard app as an excellent way to navigate menus. Now you don't need to code it yourself.

## Inspector Configuration

The Inspector for the Manager object exposes:

- Toggles for vibrations on each event
- Debug charts to visualize input for development and debugging
- Click speed tolerance for your application


# Prerequisites

If you got this from the Asset Store then I've already packaged a version of the prerequisites for you.

If not then you'll need to import the base Cardboard SDK if you haven't already:
- [Download CardboardSDK.unitypackage](https://github.com/googlesamples/cardboard-unity/blob/master/CardboardSDKForUnity.unitypackage?raw=true) from [Google's repository](https://github.com/googlesamples/cardboard-unity).
- Import it in your scene
- Use the `CardboardMain` prefab for your camera

I don't recommend it but you could use the Durovis Dive instead:
- download the "Plugin Package" from https://www.durovis.com/sdk.html
- Import it in your scene
- Use the `Dive_Camera` prefab for your camera

Dive has many technical issues but supports iOS devices.


# Usage

When you import this package, just add the `CardboardInputManager` to the root of your scene. Now you can start using the API which is thoroughly documented in [the DemoScene code](https://github.com/JScott/CardboardSDK-Unity/blob/master/CardboardInput/DemoScene/ExampleCharacterController.cs).
