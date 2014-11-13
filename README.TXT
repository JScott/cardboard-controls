CardboardSDK handles the relevant device input for using a Google Cardboard.

-----

See the comments in ExampleCharacterController.cs for details on how to utilize the class.

Here's a cheat-sheet:

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

-----

Roadmap:
- Better debug tools
- Tap/tilt input via accelerometer
- Screen compression filter