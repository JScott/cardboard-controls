using UnityEngine;
using System.Collections;

/**
* Dealing with raw input from a Cardboard device
*/
public class ParsedSensorData : MonoBehaviour {
  private Vector3 previousAcceleration;

  private float accelerationMagnitude;
  private float gyroRotationMagnitude;
  private float magneticFieldMagnitude;
  private float magneticFieldBaseLine;
  private float magneticFieldRatio;  

  private int slowImpulseFilter = 25;
  private int fastMagnetImpulseFilter = 3;
  private int fastRotationImpulseFilter = 3;
  private int fastAccelerationImpulseFilter = 5;  // Clicking the magnet tends to make a small, sharp spike in movement

  public void Start() {
    Input.compass.enabled = true;
    Input.gyro.enabled = true;
    accelerationMagnitude = Input.acceleration.magnitude;
    gyroRotationMagnitude = Input.gyro.rotationRate.magnitude;
    magneticFieldMagnitude = Input.compass.rawVector.magnitude;
    magneticFieldBaseLine = Input.compass.rawVector.magnitude;
  }
  
  public void Update() {
    Vector3 accelerationDelta = CalculateAccelerationDelta();
    Vector3 gyroRotation = Input.gyro.rotationRate;
    Vector3 magneticField = Input.compass.rawVector;

    FilterAndSetMagnitudes(accelerationDelta.magnitude, gyroRotation.magnitude, magneticField.magnitude);
    magneticFieldRatio = (magneticFieldMagnitude / magneticFieldBaseLine);
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
  private void FilterAndSetMagnitudes(float newAccelerationMagnitude, float newGyroRotationMagnitude, float newMagneticFieldMagnitude) {
    // Apply Finite Impulse Response (FIR) filters
    // If the tilt hasn't changed, but the compass has, then the magnetic field moved
    // without device this is the essence of a cardboard magnet click.
    accelerationMagnitude = ImpulseFilter(accelerationMagnitude, newAccelerationMagnitude, fastAccelerationImpulseFilter);
    gyroRotationMagnitude = ImpulseFilter(gyroRotationMagnitude, newGyroRotationMagnitude, fastRotationImpulseFilter);
    magneticFieldMagnitude = ImpulseFilter(magneticFieldMagnitude, newMagneticFieldMagnitude, fastMagnetImpulseFilter);
    magneticFieldBaseLine = ImpulseFilter(magneticFieldBaseLine, newMagneticFieldMagnitude, slowImpulseFilter);
  }

  public bool IsJostled() {
    return accelerationMagnitude >= 0.2;
  }

  public bool IsRotatedQuickly() {
    return gyroRotationMagnitude >= 3.0;
  }

  public bool IsDown() {
    return magneticFieldRatio > 1.11;
  }

  public bool IsUp() {
    return magneticFieldRatio < 0.97;
  }
}
