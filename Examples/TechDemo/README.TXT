This stark demo will show off the various events and data that CardboardSDK allows you to plug into.

CardboardInput provides the raw data in a meaningful way to CardboardManager.

CardboardManager is your interpreter for that data. See below for a cheat sheet on the methods and delegates you can hook into.

---

  class CardboardManager
    delegate void CardboardAction(object sender, CardboardEvent cardboardEvent)
      OnMagnetDown
      OnMagnetUp
      OnMagnetClick
      OnOrientationTilt
    void Update()
    float SecondsMagnetHeld()
    bool IsMagnetHeld()
    void Vibrate()
