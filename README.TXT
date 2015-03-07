Cardboard SDK handles relevant device input for using a Google Cardboard. It gives you a wide range of events and data which expand your creative opportunities with this accessible VR platform.

I don't yet have my own solution for stereoscopic rendering and head tracking so this library relies on Durovis Drive for that. Unfortunately, the license prevents me from including it's SDK so I'll provide instructions on downloading it yourself.


----- Usage

To pull this into your existing project:
- Import the CardboardSDK package
- use the CardboardInputManager prefab in your scene

To add Durovis Dive:
- download the "Plugin Package" from https://www.durovis.com/sdk.html
- replace your main camera with the included Dive_Camera prefab

For debug purposes, Space triggers the magnet and Tab triggers the orientation tilt.


----- Example Code

TechDemo has a scene that shows off the capabilities and use of CardboardSDK. It's README and code explain the API.

I've also included techdemo.apk with Dive built in so you can see it in action.
