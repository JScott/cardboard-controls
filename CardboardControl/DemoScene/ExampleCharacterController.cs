using UnityEngine;
using System.Collections;

public class ExampleCharacterController : MonoBehaviour {
  private static CardboardControl cardboard;

  private bool vibrateTriggered = false;

  void Start () {
    /*
    Start by capturing the script on CardboardControlManager
    This script has the delegates that you'll be passing your methods to
    
    Unity provides a good primer on delegates here:
    http://unity3d.com/learn/tutorials/modules/intermediate/scripting/delegates
    */
    cardboard = GameObject.Find("CardboardControlManager").GetComponent<CardboardControl>();
    cardboard.magnet.OnDown += CardboardDown;  // When the magnet goes down
    cardboard.magnet.OnUp += CardboardUp;      // When the magnet comes back up

    // When the magnet goes down and up within the "click threshold" time
    // That slick speed threshold is configurable in the inspector
    cardboard.magnet.OnClick += CardboardClick;

    // When the thing we're looking at changes, determined by a gaze
    // The gaze distance and layer mask are public as configurable in the inspector
    cardboard.gaze.OnChange += CardboardFocus;

    // Not used here is the OnTilt delegate
    // This is triggered on rotating the device to Portrait mode
    // cardboard.box.OnTilt += ...
  }



  /*
  In this demo, we change object colours for each event triggered.
  */
  public void CardboardDown(object sender) {
    ChangeObjectColor("SphereDown");
  }

  public void CardboardUp(object sender) {
    ChangeObjectColor("SphereUp");
  }

  public void CardboardClick(object sender) {
    ChangeObjectColor("SphereClick");

    TextMesh textMesh = GameObject.Find("SphereClick/Counter").GetComponent<TextMesh>();
    int increment = int.Parse(textMesh.text) + 1;
    textMesh.text = increment.ToString();

    if (cardboard.gaze.IsHeld()) {
      Debug.Log("We've focused on this object for "+cardboard.gaze.SecondsHeld()+" seconds.");
    }
    
    // TODO: get something from gaze focus
  }

  public void CardboardFocus(object sender) {
    // If we're not focused, the focused object will be null
    if (cardboard.gaze.IsHeld()) {
      ChangeObjectColor(cardboard.gaze.Object().name);
    }
  }

  public void ChangeObjectColor(string name) {
    GameObject obj = GameObject.Find(name);
    Color newColor = new Color(Random.value, Random.value, Random.value);
    obj.GetComponent<Renderer>().material.color = newColor;
  }



  /*
  During our game we can utilize data from the CardboardControl API.
  */
  void Update() {
    TextMesh textMesh = GameObject.Find("SphereDown/Counter").GetComponent<TextMesh>();

    // IsMagnetHeld is true when the magnet has gone down but not back up yet.    
    if (!cardboard.magnet.IsHeld()) {
      textMesh.GetComponent<Renderer>().enabled = Time.time % 1 < 0.5;
      vibrateTriggered = false;
    }
    else {
      textMesh.GetComponent<Renderer>().enabled = true;

      // SecondsMagnetHeld is the number of seconds we've held the magnet down.
      // It stops when when the magnet goes up and resets when the magnet goes down.
      textMesh.text = cardboard.magnet.SecondsHeld().ToString("#.##");

      // CardboardSDK has built-in triggers vibrations to provide feedback.
      // You can toggle them via the Unity Inspector or manually trigger your
      // own vibration events, as seen here.
      if (cardboard.magnet.SecondsHeld() > 2 && !vibrateTriggered) {
        Handheld.Vibrate();
        vibrateTriggered = true;
      }
    }
  }

  /*
  Be sure to unsubscribe before this object is destroyed
  so the garbage collector can clean up the object.
  */
  void OnDestroy() {
    cardboard.magnet.OnDown -= CardboardDown;
    cardboard.magnet.OnUp -= CardboardUp;
    cardboard.magnet.OnClick -= CardboardClick;
    cardboard.gaze.OnChange -= CardboardFocus;
  }
}
