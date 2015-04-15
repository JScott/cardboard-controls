using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

/**
* Use state and delegates to report important metadata from CardboardInput
*/
public class CardboardManager : MonoBehaviour {
  public CardboardInput rawInput;
  public CardboardRaycast raycast;

  public float clickSpeedThreshold = 0.4f;

  // Magnet readings
  private bool upReported = false;
  private bool downReported = true; // down is triggered once as it finds baselines
  private bool clickReported = false;
  private bool magnetWasDown = false;
  private bool tiltReported = false; // triggered at the start
  private bool magnetHeld = false;
  private float clickStartTime = 0f;

  // Raycast readings
  private GameObject recentlyFocusedObject;
  private float focusStartTime = 0f;

  // Vibration toggles
  public bool vibrateOnMagnetDown = false;
  public bool vibrateOnMagnetUp = false;
  public bool vibrateOnMagnetClicked = true;
  public bool vibrateOnOrientationTilt = true;
  public bool vibrateOnFocusChange = false;

  // Debug
  public bool debugChartsEnabled = false;
  public KeyCode debugMagnetKey = KeyCode.Space;
  public KeyCode debugOrientationKey = KeyCode.Tab;

  public delegate void CardboardAction(object sender, CardboardEvent cardboardEvent);
  public CardboardAction OnMagnetDown = delegate {};
  public CardboardAction OnMagnetUp = delegate {};
  public CardboardAction OnMagnetClicked = delegate {};
  public CardboardAction OnOrientationTilt = delegate {};
  public CardboardAction OnFocusChange = delegate {};

  public void Start() {
    rawInput = new CardboardInput();
    raycast = new CardboardRaycast();
  }

  private bool DebugKey(string forInput) {
    if (!Debug.isDebugBuild) return false;
    switch(forInput) {
      case "magnetDown":
        return Input.GetKeyDown(debugMagnetKey);
      case "magnetUp":
        return Input.GetKeyUp(debugMagnetKey);
      case "orientationTilt":
        return Input.GetKeyDown(debugOrientationKey);
      default:
        return false;
    }
  }

  public void Update() {
    rawInput.Update();
    raycast.Update();

    if (!rawInput.Jostled() && !rawInput.RotatedQuickly()) {
      if (rawInput.MagnetMovedDown() || DebugKey("magnetDown")) ReportDown();
      else downReported = false;

      if (rawInput.MagnetMovedUp() || DebugKey("magnetUp")) ReportUp();
      else upReported = false;
    } 

    if (rawInput.OrientationTilted() || DebugKey("orientationTilt")) ReportTilt();
    else tiltReported = false;

    if (recentlyFocusedObject != raycast.FocusedObject()) {
      ReportFocusChange();
    }
    recentlyFocusedObject = raycast.FocusedObject();

    if (Debug.isDebugBuild && debugChartsEnabled) {
      string charts = rawInput.MagnetReadingsChart() + "\n" + MagnetStateChart();
      Debug.Log(charts);
    }
  }

  private void ReportDown() {
    if (!downReported) {
      OnMagnetDown(this, new CardboardEvent());
      if (Debug.isDebugBuild) Debug.Log(" *** Magnet Down *** ");
      if (vibrateOnMagnetDown) Vibrate();
      SetMagnetFlagsForGoing("down");
    }
  }

  private void ReportUp() {
    if (!upReported) {
      OnMagnetUp(this, new CardboardEvent());
      if (Debug.isDebugBuild) Debug.Log(" *** Magnet Up *** ");
      if (vibrateOnMagnetUp) Vibrate();
      if (magnetWasDown) CheckForClick();
      SetMagnetFlagsForGoing("up");
    }
  }

  private void SetMagnetFlagsForGoing(string direction) {
    switch (direction) {
      case "down":
        downReported = true;
        magnetHeld = true;
        magnetWasDown = true;
        clickStartTime = Time.time;
        break;
      case "up":
        upReported = true;
        magnetHeld = false;
        magnetWasDown = false;
        break;
    }
  }

  private void CheckForClick() {
    bool withinClickThreshold = SecondsMagnetHeld() <= clickSpeedThreshold;
    clickStartTime = 0f;
    if (withinClickThreshold) ReportClick();
  }

  private void ReportClick() {
    OnMagnetClicked(this, new CardboardEvent());
    if (Debug.isDebugBuild) Debug.Log(" *** Magnet Click *** ");
    if (vibrateOnMagnetClicked) Vibrate();
  }

  private void ReportTilt() {
    if (!tiltReported) {
      OnOrientationTilt(this, new CardboardEvent());
      if (Debug.isDebugBuild) Debug.Log(" *** Orientation Tilt *** ");
      if (vibrateOnOrientationTilt) Vibrate();
      tiltReported = true;
    }
  }

  private void ReportFocusChange() {
    OnFocusChange(this, new CardboardEvent());
    //if (Debug.isDebugBuild) Debug.Log(" *** Focus Changed *** ");
    if (vibrateOnFocusChange) Vibrate();
    focusStartTime = Time.time;
  }

  public void Vibrate() {
    Handheld.Vibrate();
  }

  public float SecondsMagnetHeld() {
    if (clickStartTime == 0f) return 0f;
    return Time.time - clickStartTime;
  }

  public bool IsMagnetHeld() {
    return magnetHeld;
  }

  public RaycastHit Focus() {
    if (raycast.IsFocused()) {
      return raycast.Focus();
    } else {
      return new RaycastHit();
    }
  }

  public GameObject FocusedObject() {
    if (raycast.IsFocused()) {
      return raycast.FocusedObject();
    } else {
      return null;
    }
  }

  public float SecondsFocused() {
    if (focusStartTime == 0f) return 0f;
    return Time.time - focusStartTime;
  }

  public string MagnetStateChart() {
    string chart = "";
    chart += "Magnet State\n";
    chart += downReported ? "D " : "x ";
    chart += magnetWasDown ? "d " : "_ ";
    chart += upReported ? "U " : "x ";
    chart += clickReported ? "C " : "x ";
    chart += tiltReported ? "T " : "x ";
    chart += "\n";
    return chart;
  }
}
