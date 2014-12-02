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
  private bool magnetWentDown = false;

  private bool magnetHeld = false;

  public bool vibrateOnMagnetDown = false;
  public bool vibrateOnMagnetUp = false;
  public bool vibrateOnMagnetClicked = true;


  public bool verboseDebug = false;

  public delegate void CardboardAction(object sender, CardboardEvent cardboardEvent);
  public CardboardAction OnMagnetDown = delegate {};
  public CardboardAction OnMagnetUp = delegate {};
  public CardboardAction OnMagnetClicked = delegate {};

  public void Start() {
    input = new CardboardInput();
  }

  public void Update() {
    input.Update();

    if (!input.Jostled()) { // && !input.RotatedQuickly()) {
      if (!input.MagnetMovedDown()) ReportDown();
      else downReported = false;

      if (!input.MagnetMovedUp()) ReportUp();
      else upReported = false;
    }

    if (Debug.isDebugBuild && verboseDebug) {
      string charts = input.MagnetReadingsChart() + "\n" + MagnetStateChart();
      Debug.Log(charts);
    }
  }

  public void ReportDown() {
    if (downReported == false) {
      if (Debug.isDebugBuild) Debug.Log(" *** Magnet Down *** ");
      downReported = true;
      magnetHeld = true;
      clickStartTime = Time.time;
      magnetWentDown = true;
      OnMagnetDown(this, new CardboardEvent());
      if (vibrateOnMagnetDown) Vibrate();
    }
  }

  public void ReportUp() {
    if (upReported == false) {
      if (Debug.isDebugBuild) Debug.Log(" *** Magnet Up *** ");
      upReported = true;
      magnetHeld = false;
      OnMagnetUp(this, new CardboardEvent());
      if (vibrateOnMagnetUp) Vibrate();
      if (magnetWentDown) CheckForClick();
      magnetWentDown = false;
    }
  }

  public void CheckForClick() {
    bool withinClickThreshold = SecondsMagnetHeld() <= clickSpeedThreshold;
    clickStartTime = 0f;
    if (withinClickThreshold) ReportClick();
    else clickReported = false;
  }

  public void ReportClick() {
    if(clickReported == false) {
      if (Debug.isDebugBuild) Debug.Log(" *** Magnet Click *** ");
      OnMagnetClicked(this, new CardboardEvent());
      if (vibrateOnMagnetClicked) Vibrate();
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
    chart += magnetWentDown ? "d " : "_ ";
    chart += upReported ? "U " : "x ";
    chart += clickReported ? "C " : "x ";
    chart += "\n";
    return chart;
  }
}