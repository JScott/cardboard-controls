using UnityEngine;
using System.Collections;
using CardboardControlDelegates;

/**
* Creating a vision raycast and handling the data from it
* Relies on Google Cardboard SDK API's
*/
public class CardboardControlTrigger : MonoBehaviour {
  public float clickSpeedThreshold = 0.4f;
  public bool vibrateOnDown = false;
  public bool vibrateOnUp = false;
  public bool vibrateOnClick = true;
  public KeyCode triggerKey = KeyCode.Space;

  private ParsedMagnetData magnet;
  private ParsedTouchData touch;
  private enum TriggerState { Up, Down }
  private TriggerState currentTriggerState = TriggerState.Up;
  private float clickStartTime = 0f;

  public CardboardControlDelegate OnUp = delegate {};
  public CardboardControlDelegate OnDown = delegate {};
  public CardboardControlDelegate OnClick = delegate {};

  public void Start() {
    magnet = new ParsedMagnetData();
    touch = new ParsedTouchData();
  }

  public void Update() {
    magnet.Update();
    touch.Update();
    CheckTouch();
    CheckMagnet();
    CheckKey();
  }

  private bool KeyFor(string direction) {
    switch(direction) {
      case "down":
        return Input.GetKeyDown(triggerKey);
      case "up":
        return Input.GetKeyUp(triggerKey);
      default:
        return false;
    }
  }

  private void CheckKey() {
    if (KeyFor("down")) ReportDown();
    if (KeyFor("up")) ReportUp();
  }

  private void CheckMagnet() {
    if (IsMagnetReady()) {
      if (magnet.IsDown()) ReportDown();
      if (magnet.IsUp()) ReportUp();
    }
  }

  private void CheckTouch() {
    if (touch.IsDown()) ReportDown();
    if (touch.IsUp()) ReportUp();
  }

  private bool IsMagnetReady() {
    return !(magnet.IsCalibrating() || magnet.IsJostled() || magnet.IsRotatedQuickly());
  }

  private bool IsTouching() {
    return Input.touchCount > 0;
  }

  private void ReportDown() {
    if (currentTriggerState == TriggerState.Up) {
      currentTriggerState = TriggerState.Down;
      OnDown(this);
      if (vibrateOnDown) Handheld.Vibrate();
      clickStartTime = Time.time;
    }
  }

  private void ReportUp() {
    if (currentTriggerState == TriggerState.Down) {
      currentTriggerState = TriggerState.Up;
      OnUp(this);
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
    OnClick(this);
    if (vibrateOnClick) Handheld.Vibrate();
  }

  public float SecondsHeld() {
    return Time.time - clickStartTime;
  }

  public bool IsHeld() {
    return (currentTriggerState == TriggerState.Down);
  }

  public string SensorChart() {
    return MagnetChart() + "\n\n" + TouchChart();
  }

  public string MagnetChart() {
    string chart = "Magnet Readings\n";
    chart += !magnet.IsJostled() ? "***** steady " : "!!!!! jostled ";
    if (!magnet.IsJostled()) {
      chart += magnet.IsDown() ? "vvv " : "    ";
      chart += magnet.IsUp() ? "^^^ " : "    ";
    }
    return chart;
  }

  public string TouchChart() {
    string chart = "Touch Reading: ";
    chart += IsTouching() ? "touching" : "--";
    return chart;
  }

  public string StateChart() {
    string chart = "";
    chart += "Trigger State: ";
    chart += IsHeld() ? "down" : "up";
    return chart;
  }
}
