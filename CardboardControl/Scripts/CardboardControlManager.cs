using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using CardboardInputDelegates;

/**
* Use state and delegates to report important metadata from CardboardInput
*/
public class CardboardControlManager : MonoBehaviour {
  public CardboardControl rawInput;
  [HideInInspector]
  public CardboardControlGaze gaze;
  public CardboardDebug debug;

  public float clickSpeedThreshold = 0.4f;

  // Magnet readings
  private enum MagnetState {
    Up,
    Down
  }
  private MagnetState currentMagnetState = MagnetState.Up;
  private bool tiltReported = false; // triggered at the start
  private float clickStartTime = 0f;

  // Vibration toggles
  public bool vibrateOnMagnetDown = false;
  public bool vibrateOnMagnetUp = false;
  public bool vibrateOnMagnetClicked = true;
  public bool vibrateOnOrientationTilt = true;

  // Debug
  public bool debugNotificationsEnabled = true;
  public bool debugChartsEnabled = false;
  public KeyCode debugMagnetKey = KeyCode.Space;
  public KeyCode debugOrientationKey = KeyCode.Tab;

  // Delegates
  public CardboardInputDelegate OnMagnetDown = delegate {};
  public CardboardInputDelegate OnMagnetUp = delegate {};
  public CardboardInputDelegate OnMagnetClicked = delegate {};
  public CardboardInputDelegate OnOrientationTilt = delegate {};

  public void Awake() {
    rawInput = new CardboardControl();
    gaze = gameObject.GetComponent<CardboardControlGaze>();
    debug = new CardboardDebug(debugMagnetKey, debugOrientationKey);
  }

  public void Update() {
    rawInput.Update();
    gaze.Update();

    CheckMagnetMovement();
    CheckOrientationTilt();

    if (debugChartsEnabled) PrintDebugCharts();
  }


  private void PrintDebugCharts() {
    string charts = debug.Charts(
      IsMagnetHeld(),
      tiltReported,
      rawInput.MagnetReadingsChart()
    );
    Debug.Log(charts);
  }


  private void CheckMagnetMovement() {
    if (!rawInput.Jostled() && !rawInput.RotatedQuickly()) {
      if (rawInput.MagnetMovedDown() || debug.KeyFor("magnetDown")) ReportDown();
      if (rawInput.MagnetMovedUp() || debug.KeyFor("magnetUp")) ReportUp();
    }
  }
    private void ReportDown() {
    if (currentMagnetState == MagnetState.Up) {
      currentMagnetState = MagnetState.Down;
      OnMagnetDown(this, new CardboardInputEvent());
      if (debugNotificationsEnabled) Debug.Log(" *** Magnet Down *** ");
      if (vibrateOnMagnetDown) Vibrate();
      clickStartTime = Time.time;
    }
  }
  private void ReportUp() {
    if (currentMagnetState == MagnetState.Down) {
      currentMagnetState = MagnetState.Up;
      OnMagnetUp(this, new CardboardInputEvent());
      if (debugNotificationsEnabled) Debug.Log(" *** Magnet Up *** ");
      if (vibrateOnMagnetUp) Vibrate();
      CheckForClick();
    }
  }
  private void CheckForClick() {
    bool withinClickThreshold = SecondsMagnetHeld() <= clickSpeedThreshold;
    clickStartTime = 0f;
    if (withinClickThreshold) ReportClick();
  }
  private void ReportClick() {
    OnMagnetClicked(this, new CardboardInputEvent());
    if (debugNotificationsEnabled) Debug.Log(" *** Magnet Click *** ");
    if (vibrateOnMagnetClicked) Vibrate();
  }


  private void CheckOrientationTilt() {
    if (rawInput.OrientationTilted() || debug.KeyFor("orientationTilt")) ReportTilt();
    else tiltReported = false;
  }
  private void ReportTilt() {
    if (!tiltReported) {
      OnOrientationTilt(this, new CardboardInputEvent());
      if (debugNotificationsEnabled) Debug.Log(" *** Orientation Tilt *** ");
      if (vibrateOnOrientationTilt) Vibrate();
      tiltReported = true;
    }
  }


  public void Vibrate() {
    Handheld.Vibrate();
  }
  public float SecondsMagnetHeld() {
    if (clickStartTime == 0f) return 0f;
    return Time.time - clickStartTime;
  }
  public bool IsMagnetHeld() {
    return (currentMagnetState == MagnetState.Down);
  }
}
