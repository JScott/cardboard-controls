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
    debug = new CardboardDebug(debugMagnetKey, debugOrientationKey);
  }
  public void Update() {
    rawInput.Update();
    raycast.Update();
    debug.Update( );

    CheckMagnetMovement();
    CheckOrientationTilt();
    CheckRaycastFocus();

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


  private void CheckOrientationTilt() {
    if (rawInput.OrientationTilted() || debug.KeyFor("orientationTilt")) ReportTilt();
    else tiltReported = false;
  }
  private void ReportTilt() {
    if (!tiltReported) {
      OnOrientationTilt(this, new CardboardEvent());
      if (debugNotificationsEnabled) Debug.Log(" *** Orientation Tilt *** ");
      if (vibrateOnOrientationTilt) Vibrate();
      tiltReported = true;
    }
  }


  private void CheckRaycastFocus() {
    if (recentlyFocusedObject != FocusedObject()) ReportFocusChange();
    recentlyFocusedObject = FocusedObject();
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
  public void SetRaycastDistance(float distance) {
    raycast.maxDistance = distance;
  }
  public void SetRaycastLayerMask(LayerMask layerMask) {
    raycast.layerMask = layerMask;
  }
}
