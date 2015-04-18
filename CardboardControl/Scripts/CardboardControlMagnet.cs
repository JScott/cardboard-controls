using UnityEngine;
using System.Collections;
using CardboardInputDelegates;

/**
* Creating a vision raycast and handling the data from it
* Relies on Google Cardboard SDK API's
*/
public class CardboardControlMagnet : MonoBehaviour {  
  public float clickSpeedThreshold = 0.4f;
  public bool vibrateOnDown = false;
  public bool vibrateOnUp = false;
  public bool vibrateOnClick = true;
  public KeyCode debugKey = KeyCode.Space;

  private ParsedSensorData sensor;
  private enum MagnetState { Up, Down }
  private MagnetState currentMagnetState = MagnetState.Up;
  private float clickStartTime = 0f;

  public CardboardInputDelegate OnUp = delegate {};
  public CardboardInputDelegate OnDown = delegate {};
  public CardboardInputDelegate OnClick = delegate {};

  public void Start() {
    sensor = gameObject.GetComponent<ParsedSensorData>();
  }
  
  public void Update() {
    CheckMagnet();
  }

  private bool KeyFor(string direction) {
    if (direction == "down") {
      return Input.GetKeyDown(debugKey);
    } else {
      return Input.GetKeyUp(debugKey);
    }
  }

  private void CheckMagnet() {
    if (!sensor.IsJostled() && !sensor.IsRotatedQuickly()) {
      if (sensor.IsDown() || KeyFor("down")) ReportDown();
      if (sensor.IsUp() || KeyFor("up")) ReportUp();
    }
  }
  
  private void ReportDown() {
    if (currentMagnetState == MagnetState.Up) {
      currentMagnetState = MagnetState.Down;
      OnDown(this, new CardboardInputEvent());
      // if (debugNotificationsEnabled) Debug.Log(" *** Magnet Down *** ");
      if (vibrateOnDown) Handheld.Vibrate();
      clickStartTime = Time.time;
    }
  }
  
  private void ReportUp() {
    if (currentMagnetState == MagnetState.Down) {
      currentMagnetState = MagnetState.Up;
      OnUp(this, new CardboardInputEvent());
      // if (debugNotificationsEnabled) Debug.Log(" *** Magnet Up *** ");
      if (vibrateOnUp) Handheld.Vibrate();
      CheckForClick();
    }
  }
  
  private void CheckForClick() {
    bool withinClickThreshold = SecondsHeld() <= clickSpeedThreshold;
    clickStartTime = 0f;
    if (withinClickThreshold) ReportClick();
  }

  private void ReportClick() {
    OnClick(this, new CardboardInputEvent());
    // if (debugNotificationsEnabled) Debug.Log(" *** Magnet Click *** ");
    if (vibrateOnClick) Handheld.Vibrate();
  }

  public float SecondsHeld() {
    if (clickStartTime == 0f) return 0f;
    return Time.time - clickStartTime;
  }

  public bool IsHeld() {
    return (currentMagnetState == MagnetState.Down);
  }
}
