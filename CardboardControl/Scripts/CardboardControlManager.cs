using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using CardboardControlDelegates;

/**
* Bring all the control scripts together to provide a convenient API
*/
public class CardboardControlManager : MonoBehaviour {
  [HideInInspector]
  public CardboardControlMagnet magnet;
  [HideInInspector]
  public CardboardControlGaze gaze;
  [HideInInspector]
  public CardboardControlBox box;
  public CardboardDebug debug;


  // Debug
  public bool debugNotificationsEnabled = true;
  public bool debugChartsEnabled = false;


  // Delegates
  public CardboardControlDelegate OnOrientationTilt = delegate {};

  public void Awake() {
    magnet = gameObject.GetComponent<CardboardControlMagnet>();
    gaze = gameObject.GetComponent<CardboardControlGaze>();
    box = gameObject.GetComponent<CardboardControlBox>();
    //debug = new CardboardDebug(debugMagnetKey, debugOrientationKey);
  }

  public void Update() {
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

  public void Vibrate() {
    Handheld.Vibrate();
  }
}
