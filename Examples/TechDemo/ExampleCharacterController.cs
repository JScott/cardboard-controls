using UnityEngine;
using System.Collections;

public class ExampleCharacterController : MonoBehaviour {
  private static CardboardManager cardboard;

  private Vector3 moveDirection = Vector3.zero;
  private CharacterController controller;

  private bool vibrateTriggered = false;

	void Start () {
    controller = GetComponent<CharacterController>();

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

    // Not shown here is the OnOrientationTilt delegate.
    // This is triggered on rotating the device to Portrait mode.
    // The Google Cardboard app refers to this gesture as a Tilt.
	}



  /*
  In this demo, we change sphere colours for each event triggered.
  The CardboardEvent will eventually pass useful data related to the event
  but it's currently just a placeholder.
  */
  public void CardboardDown(object sender, CardboardEvent cardboardEvent) {
    ChangeSphereColor("SphereDown");
  }

  public void CardboardUp(object sender, CardboardEvent cardboardEvent) {
    ChangeSphereColor("SphereUp");
  }

  public void CardboardClick(object sender, CardboardEvent cardboardEvent) {
    ChangeSphereColor("SphereClick");

    TextMesh textMesh = GameObject.Find("SphereClick/Counter").GetComponent<TextMesh>();
    int increment = int.Parse(textMesh.text) + 1;
    textMesh.text = increment.ToString();
  }

  public void ChangeSphereColor(string name) {
    GameObject sphere = GameObject.Find(name);
    sphere.renderer.material.color = new Color(Random.value, Random.value, Random.value);
  }



  /*
  During our game we can utilize data from CardboardInput.
  */
  void Update() {
    TextMesh textMesh = GameObject.Find("SphereDown/Counter").GetComponent<TextMesh>();

    // IsMagnetHeld is true when the magnet has gone down but not back up yet.    
    if (!cardboard.IsMagnetHeld()) {
      textMesh.renderer.enabled = Time.time % 1 < 0.5;
      vibrateTriggered = false;
    }
    else {
      textMesh.renderer.enabled = true;

      // SecondsMagnetHeld is the number of seconds we've held the magnet down.
      // It stops when when the magnet goes up and resets when the magnet goes down.
      textMesh.text = cardboard.SecondsMagnetHeld().ToString("#.##");

      // CardboardSDK has built-in triggers vibrations to provide feedback.
      // You can toggle them via the Unity Inspector or manually trigger your
      // own vibration events, as seen here.
      if (cardboard.SecondsMagnetHeld() > 2 && !vibrateTriggered) {
        cardboard.Vibrate();
        vibrateTriggered = true;
        // Unfortunately, magnet input is briefly ignored during vibrations.
        // This is because the small physical movement jostles the accelerometer.
        // You may want to limit usage to click-driven or non-magnet events.
      }
    }

    // Not shown here is the Vibrate methods which simply vibrates the device.
  }

  /*
  Be sure to unsubscribe before this object is destroyed
  so the garbage collector can clean up the object.
  */
  void OnDestroy() {
    cardboard.OnMagnetDown -= CardboardDown;
    cardboard.OnMagnetUp -= CardboardUp;
    cardboard.OnMagnetClicked -= CardboardClick;
  }
}
