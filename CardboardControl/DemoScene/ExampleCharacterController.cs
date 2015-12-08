using UnityEngine;
using System.Collections;

public class ExampleCharacterController : MonoBehaviour {
  private static CardboardControl cardboard;

  void Start () {
    /*
    * Start by capturing the helper script on CardboardControlManager
    * This script has access to all the controls and their delegates
    *
    * Unity provides a good primer on delegates here:
    * http://unity3d.com/learn/tutorials/modules/intermediate/scripting/delegates
    */
    cardboard = GameObject.Find("CardboardControlManager").GetComponent<CardboardControl>();
    cardboard.trigger.OnDown += CardboardDown;  // When the trigger goes down
    cardboard.trigger.OnUp += CardboardUp;      // When the trigger comes back up

    // When the magnet or touch goes down and up within the "click threshold" time
    // That click speed threshold is configurable in the inspector
    cardboard.trigger.OnClick += CardboardClick;

    // When the thing we're looking at changes, determined by a gaze
    // The gaze distance and layer mask are public as configurable in the inspector
    cardboard.gaze.OnChange += CardboardFocus;

    // When we rotate the device into portrait mode
    cardboard.box.OnTilt += CardboardMagnetReset;

    // We also can trigger when we've been staring at an object
    // cardboard.gaze.OnStare += ...
  }



  /*
  * In this demo, we randomize object colours for triggered events
  */
  public void CardboardDown(object sender) {
    Debug.Log("Trigger went down");
    ChangeObjectColor("SphereDown");
  }

  public void CardboardUp(object sender) {
    Debug.Log("Trigger came up");
    ChangeObjectColor("SphereUp");
  }

  public void CardboardClick(object sender) {
    ChangeObjectColor("SphereClick");

    TextMesh textMesh = GameObject.Find("SphereClick/Counter").GetComponent<TextMesh>();
    int increment = int.Parse(textMesh.text) + 1;
    textMesh.text = increment.ToString();

    // With the cardboard object, we can grab information from various controls
    // If the raycast doesn't find anything then the focused object will be null
    string name = cardboard.gaze.Object() == null ? "nothing" : cardboard.gaze.Object().name;
    float count = cardboard.gaze.SecondsHeld();
    Debug.Log("We've focused on "+name+" for "+count+" seconds.");

    // If you need more raycast data from cardboard.gaze, the RaycastHit is exposed as gaze.Hit
  }

  public void CardboardFocus(object sender) {
    // For more event-driven code, you can grab the data from the sender
    CardboardControlGaze gaze = sender as CardboardControlGaze;
    // gaze.IsHeld will make sure the gaze.Object isn't null
    if (gaze.IsHeld() && gaze.Object().name.Contains("Cube")) {
      ChangeObjectColor(gaze.Object().name);
    }
  }

  public void CardboardMagnetReset(object sender) {
    // Resetting the magnet will reset the polarity if up and down are confused
    // This occasionally happens when the device is inserted into the enclosure
    // or if the magnetometer readings are weak enough to cut in and out
    Debug.Log("Device tilted");
    cardboard.trigger.ResetMagnetState();
    ResetSpheres();
  }

  public void ChangeObjectColor(string name) {
    GameObject obj = GameObject.Find(name);
    Color newColor = new Color(Random.value, Random.value, Random.value);
    obj.GetComponent<Renderer>().material.color = newColor;
  }

  public void ResetSpheres() {
    string[] spheres = { "SphereDown", "SphereUp", "SphereClick" };
    foreach (string sphere in spheres) {
      GameObject obj = GameObject.Find(sphere);
      obj.GetComponent<Renderer>().material.color = Color.white;
    }
  }



  /*
  * During our game we can utilize data from the CardboardControl API
  */
  void Update() {
    TextMesh textMesh = GameObject.Find("SphereDown/Counter").GetComponent<TextMesh>();

    // trigger.IsHeld is true when the trigger has gone down but not back up yet
    if (cardboard.trigger.IsHeld()) {
      textMesh.GetComponent<Renderer>().enabled = true;
      // trigger.SecondsHeld is the number of seconds we've held the trigger down
      textMesh.text = cardboard.trigger.SecondsHeld().ToString("#.##");
    }
    else {
      textMesh.GetComponent<Renderer>().enabled = Time.time % 1 < 0.5;
    }
  }

  /*
  * Be sure to unsubscribe before this object is destroyed
  * so the garbage collector can clean everything up
  */
  void OnDestroy() {
    cardboard.trigger.OnDown -= CardboardDown;
    cardboard.trigger.OnUp -= CardboardUp;
    cardboard.trigger.OnClick -= CardboardClick;
    cardboard.gaze.OnChange -= CardboardFocus;
  }
}
