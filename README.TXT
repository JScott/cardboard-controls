Cardboard SDK handles relevant device input for using a Google Cardboard. It gives you a wide range of events and data which expand your creative opportunities with this accessible VR platform.


----- Usage

To pull this into your existing project, copy over the following folders:
- Plugins
- Dive
- CardboardSDK

Then replace your main camera with the Dive/Dive_Camera prefab. Also be sure to set the default orientation to Landscape Left in the Player Settings.


----- Example Code

See the comments in ExampleCharacterController.cs for details on how to utilize the CardboardInput class. Here's a cheat-sheet:

  class CardboardInput
    float clickSpeedThreshold
    delegate void CardboardAction(object sender, CardboardEvent cardboardEvent)
      OnMagnetDown
      OnMagnetUp
      OnMagnetMoved
      OnMagnetClick
    void Update()
    float SecondsMagnetHeld()
    bool IsMagnetHeld()
    bool WasClicked()


----- Roadmap

In no particular order:
- Vibration feedback
- Better debug tools
- Event data
- Tap/tilt input via accelerometer
- Screen compression filter
- Camera input


----- Licensing

Read CONTRIBUTORS.TXT first as it contains an overview of the licenses for the libraries used. Dive in particular has an interesting requirement.