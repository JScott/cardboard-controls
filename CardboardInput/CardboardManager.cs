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
  public CardboardInput input;

  public float clickSpeedThreshold = 0.4f;
  private float clickStartTime = 0f;

  // Magnet readings
  private bool upReported = false;
  private bool downReported = true; // down is triggered once as it finds baselines
  private bool clickReported = false;
  private bool magnetWasDown = false;
  private bool tiltReported = false; // triggered at the start

  private bool magnetHeld = false;

  public bool vibrateOnMagnetDown = false;
  public bool vibrateOnMagnetUp = false;
  public bool vibrateOnMagnetClicked = true;
  public bool vibrateOnOrientationTilt = true;

  public bool debugChartsEnabled = false;
  public KeyCode debugMagnetKey = KeyCode.Space;
  public KeyCode debugOrientationKey = KeyCode.Tab;

  public delegate void CardboardAction(object sender, CardboardEvent cardboardEvent);
  public CardboardAction OnMagnetDown = delegate {};
  public CardboardAction OnMagnetUp = delegate {};
  public CardboardAction OnMagnetClicked = delegate {};
  public CardboardAction OnOrientationTilt = delegate {};

  public void Start() {
    input = new CardboardInput();
  }

  public bool DebugKey(string forInput) {
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
    input.Update();

    if (!input.Jostled() && !input.RotatedQuickly()) {
      if (input.MagnetMovedDown() || DebugKey("magnetDown")) ReportDown();
      else downReported = false;

      if (input.MagnetMovedUp() || DebugKey("magnetUp")) ReportUp();
      else upReported = false;
    } 

    if (input.OrientationTilted() || DebugKey("orientationTilt")) ReportTilt();
    else tiltReported = false;

    if (Debug.isDebugBuild && debugChartsEnabled) {
      string charts = input.MagnetReadingsChart() + "\n" + MagnetStateChart();
      Debug.Log(charts);
    }
  }

  public void ReportDown() {
    if (!downReported) {
      OnMagnetDown(this, new CardboardEvent());
      if (Debug.isDebugBuild) Debug.Log(" *** Magnet Down *** ");
      if (vibrateOnMagnetDown) Vibrate();
      SetMagnetFlagsForGoing("down");
    }
  }

  public void ReportUp() {
    if (!upReported) {
      OnMagnetUp(this, new CardboardEvent());
      if (Debug.isDebugBuild) Debug.Log(" *** Magnet Up *** ");
      if (vibrateOnMagnetUp) Vibrate();
      if (magnetWasDown) CheckForClick();
      SetMagnetFlagsForGoing("up");
    }
  }

  public void SetMagnetFlagsForGoing(string direction) {
    switch (direction) {
      case "down":
        downReported = true;
        magnetHeld = true;
        clickStartTime = Time.time;
        magnetWasDown = true;
        break;
      case "up":
        upReported = true;
        magnetHeld = false;
        magnetWasDown = false;
        break;
    }
  }

  public void CheckForClick() {
    bool withinClickThreshold = SecondsMagnetHeld() <= clickSpeedThreshold;
    clickStartTime = 0f;
    if (withinClickThreshold) ReportClick();
  }

  public void ReportClick() {
    OnMagnetClicked(this, new CardboardEvent());
    if (Debug.isDebugBuild) Debug.Log(" *** Magnet Click *** ");
    if (vibrateOnMagnetClicked) Vibrate();
  }

  public void ReportTilt() {
    if (!tiltReported) {
      OnOrientationTilt(this, new CardboardEvent());
      if (Debug.isDebugBuild) Debug.Log(" *** Orientation Tilt *** ");
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
    return magnetHeld;
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