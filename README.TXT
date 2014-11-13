CardboardSDK handles the relevant device input for using a Google Cardboard.

See the comments in CardboardInput.cs for details on how to utilize the class.

-----

If you don't like reading then this should give a decent overview:

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