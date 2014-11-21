This stark demo will show off the various events and data that CardboardSDK allows you to plug into.

The functionality hinges on the CardboardInput class, a MonoBehaviour class for a single Manager object in the scene. ExampleCharacterController.cs is heavily documented to show you how to utilize CardboardInput.


---

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
