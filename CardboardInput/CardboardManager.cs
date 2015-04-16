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
  private enum MagnetState {
    Up,
    Down
  }
  private MagnetState currentMagnetState = MagnetState.Up;
  private bool tiltReported = false; // triggered at the start
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
  public bool debugNotificationsEnabled = true;
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
      if (rawInput.MagnetMovedUp() || DebugKey("magnetUp")) ReportUp();
    } 

    if (rawInput.OrientationTilted() || DebugKey("orientationTilt")) ReportTilt();
    else tiltReported = false;

    if (recentlyFocusedObject != FocusedObject()) ReportFocusChange();
    recentlyFocusedObject = FocusedObject();

    if (debugChartsEnabled) {
      string charts = rawInput.MagnetReadingsChart() + "\n" + MagnetStateChart();
      Debug.Log(charts);
    }
  }

  private void ReportDown() {
    if (currentMagnetState == MagnetState.Up) {
      currentMagnetState = MagnetState.Down;
      OnMagnetDown(this, new CardboardEvent());
      if (debugNotificationsEnabled) Debug.Log(" *** Magnet Down *** ");
      if (vibrateOnMagnetDown) Vibrate();
      clickStartTime = Time.time;
    }
  }

  private void ReportUp() {
    if (currentMagnetState == MagnetState.Down) {
      currentMagnetState = MagnetState.Up;
      OnMagnetUp(this, new CardboardEvent());
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
    OnMagnetClicked(this, new CardboardEvent());
    if (debugNotificationsEnabled) Debug.Log(" *** Magnet Click *** ");
    if (vibrateOnMagnetClicked) Vibrate();
  }

  private void ReportTilt() {
    if (!tiltReported) {
      OnOrientationTilt(this, new CardboardEvent());
      if (debugNotificationsEnabled) Debug.Log(" *** Orientation Tilt *** ");
      if (vibrateOnOrientationTilt) Vibrate();
      tiltReported = true;
    }
  }

  private void ReportFocusChange() {
    OnFocusChange(this, new CardboardEvent());
    if (debugNotificationsEnabled) Debug.Log(" *** Focus Changed *** ");
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
    return (currentMagnetState == MagnetState.Down);
  }

  public RaycastHit Focus() {
    return raycast.focus;
  }

  public GameObject FocusedObject() {
    if (IsFocused()) {
      return raycast.focus.transform.gameObject;
    } else {
      return null;
    }
  }

  public bool IsFocused() {
    return raycast.focused;
  }

  public float SecondsFocused() {
    if (focusStartTime == 0f) return 0f;
    return Time.time - focusStartTime;
  }

  public SetRaycastDistance(float distance) {
    raycast.maxDistance = distance;
  }

  public SetRaycastLayerMask(LayerMask layerMask) {
    raycast.layerMask = layerMask;
  }

  public string MagnetStateChart() {
    string chart = "";
    chart += "Magnet State\n";
    chart += (currentMagnetState == MagnetState.Down) ? "D " : "x ";
    chart += (currentMagnetState == MagnetState.Up) ? "U " : "x ";
    chart += tiltReported ? "T " : "x ";
    chart += "\n";
    return chart;
  }
}
