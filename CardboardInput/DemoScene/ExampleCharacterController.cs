using UnityEngine;
using System.Collections;

public class ExampleCharacterController : MonoBehaviour {
  private static CardboardManager cardboard;

  private bool vibrateTriggered = false;

  void Start () {
    /*
    Start by getting the script off CardboardInputManager.
    This is a good place to pass your methods to its delegates.
    
    Unity provides a good primer on delegates here:
    http://unity3d.com/learn/tutorials/modules/intermediate/scripting/delegates
    */
    cardboard = GameObject.Find("CardboardInputManager").GetComponent<CardboardManager>();
    cardboard.OnMagnetDown += CardboardDown;  // When the magnet goes down
    cardboard.OnMagnetUp += CardboardUp;      // When the magnet comes back up

    // When the magnet goes down and up within the "click threshold" time
    // That limit is public as cardboard.clickSpeedThreshold
    cardboard.OnMagnetClicked += CardboardClick;

    // TODO: documentation
    cardboard.OnFocusChange += CardboardFocus;

    // Not shown here is the OnOrientationTilt delegate.
    // This is triggered on rotating the device to Portrait mode.
    // The Google Cardboard app refers to this gesture as a Tilt.
  }



  /*
  In this demo, we change object colours for each event triggered.
  The CardboardEvent is currently just a placeholder but exists to
  pass useful information to events with a consistent API.
  */
  public void CardboardDown(object sender, CardboardEvent cardboardEvent) {
    ChangeObjectColor("SphereDown");
  }

  public void CardboardUp(object sender, CardboardEvent cardboardEvent) {
    ChangeObjectColor("SphereUp");
  }

  public void CardboardClick(object sender, CardboardEvent cardboardEvent) {
    ChangeObjectColor("SphereClick");

    TextMesh textMesh = GameObject.Find("SphereClick/Counter").GetComponent<TextMesh>();
    int increment = int.Parse(textMesh.text) + 1;
    textMesh.text = increment.ToString();
  }

  public void CardboardFocus(object sender, CardboardEvent cardboardEvent) {
    // FocusedObject will be null when IsFocused is false
    if (cardboard.IsFocused()) {
      ChangeObjectColor(cardboard.FocusedObject().name);
    }
    // Focus will return an empty RaycastHit if not focused
    // TODO: do something with Focus() ?

    // TODO: do something with SecondsFocused()
  }

  public void ChangeObjectColor(string name) {
    GameObject obj = GameObject.Find(name);
    Color newColor = new Color(Random.value, Random.value, Random.value);
    obj.GetComponent<Renderer>().material.color = newColor;
  }



  /*
  During our game we can utilize data from CardboardInput.
  */
  void Update() {
    TextMesh textMesh = GameObject.Find("SphereDown/Counter").GetComponent<TextMesh>();

    // IsMagnetHeld is true when the magnet has gone down but not back up yet.    
    if (!cardboard.IsMagnetHeld()) {
      textMesh.GetComponent<Renderer>().enabled = Time.time % 1 < 0.5;
      vibrateTriggered = false;
    }
    else {
      textMesh.GetComponent<Renderer>().enabled = true;

      // SecondsMagnetHeld is the number of seconds we've held the magnet down.
      // It stops when when the magnet goes up and resets when the magnet goes down.
      textMesh.text = cardboard.SecondsMagnetHeld().ToString("#.##");

      // CardboardSDK has built-in triggers vibrations to provide feedback.
      // You can toggle them via the Unity Inspector or manually trigger your
      // own vibration events, as seen here.
      if (cardboard.SecondsMagnetHeld() > 2 && !vibrateTriggered) {
        cardboard.Vibrate();
        vibrateTriggered = true;
      }
    }
  }

  /*
  Be sure to unsubscribe before this object is destroyed
  so the garbage collector can clean up the object.
  */
  void OnDestroy() {
    cardboard.OnMagnetDown -= CardboardDown;
    cardboard.OnMagnetUp -= CardboardUp;
    cardboard.OnMagnetClicked -= CardboardClick;
    cardboard.OnFocusChange -= CardboardFocus;
  }
}
