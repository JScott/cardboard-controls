using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
* Dealing with raw magnet input from a Cardboard device
*/
public class ParsedMagnetData {
  private struct MagnetMoment {
  	public float deltaTime;
  	public float yMagnitude;
    public MagnetMoment(float deltaTime, float yMagnitude) {
      this.deltaTime = deltaTime;
      this.yMagnitude = yMagnitude;
    }
  }
  private struct MagnetWindowState {
    public float ratio;
    public float delta;
  }
  private List<MagnetMoment> magnetWindow;
  private MagnetWindowState currentMagnetWindow;
  private float MAX_WINDOW_SECONDS = 0.1f;
  #if UNITY_IOS
  private float MAGNET_RATIO_MIN_THRESHOLD = 0.005f;
  private float MAGNET_RATIO_MAX_THRESHOLD = 0.2f;
  private float MAGNET_MAGNITUDE_THRESHOLD = 800.0f;
  #else
  private float MAGNET_RATIO_MIN_THRESHOLD = 0.03f;
  private float MAGNET_RATIO_MAX_THRESHOLD = 0.2f;
  private float MAGNET_MAGNITUDE_THRESHOLD = 500.0f;
  #endif
  private float STABLE_RATIO_THRESHOLD = 0.001f;
  private float STABLE_DELTA_THRESHOLD = 2.0f;
  private float windowLength = 0.0f;

  enum TriggerState {
    Negative,
    Neutral,
    Positive
  };
  private TriggerState triggerState = TriggerState.Neutral;
  private bool isDown = false;
  private bool isStable = false;

  public ParsedMagnetData() {
    Input.compass.enabled = true;
    magnetWindow = new List<MagnetMoment>();
    windowLength = 0.0f;
  }

  public void Update() {
    TrimMagnetWindow();
    AddToMagnetWindow();
    currentMagnetWindow = CaptureMagnetWindow();

    TriggerState newTriggerState = CheckTriggerState();
    isStable = CheckStability();
    if (!isStable) ResetState();

    if (isStable && newTriggerState != TriggerState.Neutral && triggerState != newTriggerState) {
      isDown = !isDown;
      triggerState = newTriggerState;
    }
  }

  private bool CheckStability() {
    // Delta approximates how fast the device is moving relative to the magnet
    // Ratio approximates how still the device is over time
    if (MagnetAbsent()) return false;
    else if(currentMagnetWindow.delta < STABLE_DELTA_THRESHOLD &&
            currentMagnetWindow.ratio < 1f+STABLE_RATIO_THRESHOLD &&
            currentMagnetWindow.ratio > 1f-STABLE_RATIO_THRESHOLD) return true;
    return isStable;
  }

  private TriggerState CheckTriggerState() {
    if (IsNegative()) return TriggerState.Negative;
    if (IsPositive()) return TriggerState.Positive;
    return TriggerState.Neutral;
  }

  private bool MagnetAbsent() {
    return Input.compass.rawVector.magnitude < MAGNET_MAGNITUDE_THRESHOLD;
  }

  private bool IsNegative() {
    return (currentMagnetWindow.ratio < 1f-MAGNET_RATIO_MIN_THRESHOLD &&
            currentMagnetWindow.ratio > 1f-MAGNET_RATIO_MAX_THRESHOLD);
  }

  private bool IsPositive() {
    return (currentMagnetWindow.ratio > 1f+MAGNET_RATIO_MIN_THRESHOLD &&
            currentMagnetWindow.ratio < 1f+MAGNET_RATIO_MAX_THRESHOLD);
  }

  public void TrimMagnetWindow() {
    while (windowLength > MAX_WINDOW_SECONDS) {
      MagnetMoment moment = magnetWindow[0];
      magnetWindow.RemoveAt(0);
      windowLength -= moment.deltaTime;
    }
  }

  public void AddToMagnetWindow() {
    magnetWindow.Add(new MagnetMoment(Time.deltaTime, Input.compass.rawVector.magnitude));
    windowLength += Time.deltaTime;
  }

  private MagnetWindowState CaptureMagnetWindow() {
    MagnetWindowState newState = new MagnetWindowState();
    int middle = magnetWindow.Count / 2;
    List<MagnetMoment> firstHalf = magnetWindow.GetRange(0, middle);
    List<MagnetMoment> lastHalf = magnetWindow.GetRange(middle, magnetWindow.Count - middle);
    newState.ratio = Average(firstHalf) / Average(lastHalf);
    newState.delta = Mathf.Abs(magnetWindow[magnetWindow.Count-1].yMagnitude - magnetWindow[0].yMagnitude);
    return newState;
  }

  private float Average(List<MagnetMoment> moments) {
    if (moments.Count == 0) return 0.0f;
    float sum = 0.0f;
    for (int index = 0; index < moments.Count; index++) {
      sum += moments[index].yMagnitude;
    }
    return sum / moments.Count;
  }

  private bool IsMagnetGoingDown(float min, float max, float start) {
    float minDelta = Mathf.Abs(min - start);
    float maxDelta = Mathf.Abs(max - start);
    return minDelta > maxDelta;
  }

  public bool IsDown() {
    return triggerState != TriggerState.Neutral && isDown;
  }

  public bool IsUp() {
    return triggerState != TriggerState.Neutral && !isDown;
  }

  public void ResetState() {
    triggerState = TriggerState.Neutral;
    isDown = false;
  }

  public void PrintDebug() {
    Debug.Log("--- Magnetometer\nmagnitude: " + Input.compass.rawVector.magnitude +
              "\nratio: " + currentMagnetWindow.ratio +
              "\ndelta: " + currentMagnetWindow.delta + 
              "\nstable: " + isStable +
              "\nstate: " + triggerState);
  }
}
