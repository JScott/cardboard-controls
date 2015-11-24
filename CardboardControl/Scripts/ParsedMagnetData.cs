using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
* Dealing with raw magnet input from a Cardboard device
*/
public class ParsedMagnetData {
  private Vector3 previousAcceleration;

  private float accelerationMagnitude;
  private float gyroRotationMagnitude;

  private const int slowImpulseFilter = 25;
  private const int fastMagnetImpulseFilter = 3;
  private const int fastRotationImpulseFilter = 3;
  private const int fastAccelerationImpulseFilter = 5;  // Clicking the magnet tends to make a small, sharp spike in movement

  private struct MagnetMoment {
  	public float deltaTime;
  	public float yMagnitude;
    public MagnetMoment(float deltaTime, float yMagnitude) {
      this.deltaTime = deltaTime;
      this.yMagnitude = yMagnitude;
    }
  }
  private struct MagnetWindowState {
    public float firstHalf;
    public float lastHalf;
    public float ratio; // TODO: do I only need this?
  }
  private List<MagnetMoment> magnetWindow;
  private MagnetWindowState currentMagnetWindow;
  private float MAX_WINDOW_SECONDS = 0.4f;
  private float MAGNET_RATIO_THRESHOLD = 0.1f;
  private float windowLength = 0.0f;

  public ParsedMagnetData() {
    Input.compass.enabled = true;
    Input.gyro.enabled = true;
    accelerationMagnitude = Input.acceleration.magnitude;
    gyroRotationMagnitude = Input.gyro.rotationRate.magnitude;
    magnetWindow = new List<MagnetMoment>();
    windowLength = 0.0f;
  }

  public void Update() {
    Vector3 accelerationDelta = CalculateAccelerationDelta();
    Vector3 gyroRotation = Input.gyro.rotationRate;

    FilterAndSetMagnitudes(accelerationDelta.magnitude, gyroRotation.magnitude);
    TrimMagnetWindow();
    AddToMagnetWindow();
    currentMagnetWindow = CaptureMagnetWindow();

    // Debug.Log(Input.compass.rawVector.x + "\n" +
    //           Input.compass.rawVector.y + "\n" +
    //           Input.compass.rawVector.z + "\n" +
    //           Input.compass.rawVector.magnitude + "\n");
    // Debug.Log(currentMagnetWindow.firstHalf + "\n" +
    //           currentMagnetWindow.lastHalf + "\n" +
    //           currentMagnetWindow.ratio);
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
    newState.firstHalf = Average(firstHalf);
    newState.lastHalf = Average(lastHalf);
    newState.ratio = newState.firstHalf / newState.lastHalf;
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

  private Vector3 CalculateAccelerationDelta() {
    Vector3 currentAcceleration = Input.acceleration;
    Vector3 accelerationDelta = currentAcceleration - previousAcceleration;
    previousAcceleration = currentAcceleration;
    return accelerationDelta;
  }

  private float ImpulseFilter(float from_value, float to_value, int filter) {
    return ((filter-1) * from_value + to_value) / filter;
  }

  // TODO: dictionaries could shorten a lot of these lines
  private void FilterAndSetMagnitudes(float newAccelerationMagnitude, float newGyroRotationMagnitude) {
    // Apply Finite Impulse Response (FIR) filters
    // If the tilt hasn't changed, but the compass has, then the magnetic field moved without the device
    // This is the essence of a cardboard magnet click
    accelerationMagnitude = ImpulseFilter(accelerationMagnitude, newAccelerationMagnitude, fastAccelerationImpulseFilter);
    gyroRotationMagnitude = ImpulseFilter(gyroRotationMagnitude, newGyroRotationMagnitude, fastRotationImpulseFilter);
  }

  public bool IsCalibrating() {
    return Time.time <= 1.5f;
  }

  public bool IsJostled() {
    return accelerationMagnitude >= 0.15;
  }

  public bool IsRotatedQuickly() {
    return gyroRotationMagnitude >= 1.5;
  }

  public bool IsDown() {
    return currentMagnetWindow.ratio > 1.0f+MAGNET_RATIO_THRESHOLD;
  }

  public bool IsUp() {
    return currentMagnetWindow.ratio < 1.0f-MAGNET_RATIO_THRESHOLD;
  }
}
