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
  [HideInInspector]
  public CardboardControlMagnet magnet;
  [HideInInspector]
  public CardboardControlGaze gaze;
  public CardboardDebug debug;


  private bool tiltReported = false; // triggered at the start

  // Vibration toggles
  public bool vibrateOnOrientationTilt = true;

  // Debug
  public bool debugNotificationsEnabled = true;
  public bool debugChartsEnabled = false;

  public KeyCode debugOrientationKey = KeyCode.Tab;

  // Delegates
  public CardboardInputDelegate OnOrientationTilt = delegate {};

  public void Awake() {
    magnet = gameObject.GetComponent<CardboardControlMagnet>();
    gaze = gameObject.GetComponent<CardboardControlGaze>();
    //debug = new CardboardDebug(debugMagnetKey, debugOrientationKey);
  }

  public void Update() {
    magnet.Update();
    gaze.Update();

    CheckOrientationTilt();

    if (debugChartsEnabled) PrintDebugCharts();
  }


  private void PrintDebugCharts() {
    // string charts = debug.Charts(
    //   IsMagnetHeld(),
    //   tiltReported,
    //   magnet.Chart()
    // );
    // Debug.Log(charts);
  }



  private void CheckOrientationTilt() {
    if (/**/OrientationTilted() /* || KeyFor("orientationTilt")*/) ReportTilt();
    else tiltReported = false;
  }
  private void ReportTilt() {
    if (!tiltReported) {
      OnOrientationTilt(this, new CardboardInputEvent());
      //if (debugNotificationsEnabled) Debug.Log(" *** Orientation Tilt *** ");
      if (vibrateOnOrientationTilt) Vibrate();
      tiltReported = true;
    }
  }
  
  private DeviceOrientation tiltedOrientation = DeviceOrientation.Portrait;

  public bool OrientationTilted() {
    return Input.deviceOrientation == tiltedOrientation;
  }



  public void Vibrate() {
    Handheld.Vibrate();
  }
}
