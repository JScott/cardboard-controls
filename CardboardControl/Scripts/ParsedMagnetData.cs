using UnityEngine;
using System.Collections;

/**
* Dealing with raw magnet input from a Cardboard device
*/
public class ParsedMagnetData {
  private Vector3 previousAcceleration;

  private float accelerationMagnitude;
  private float gyroRotationMagnitude;
  private float magneticFieldMagnitude;
  private float magneticFieldBaseLine;
  private float magneticFieldRatio;

  private const int slowImpulseFilter = 25;
  private const int fastMagnetImpulseFilter = 3;
  private const int fastRotationImpulseFilter = 3;
  private const int fastAccelerationImpulseFilter = 5;  // Clicking the magnet tends to make a small, sharp spike in movement

  public ParsedMagnetData() {
    Input.compass.enabled = true;
    Input.gyro.enabled = true;
    accelerationMagnitude = Input.acceleration.magnitude;
    gyroRotationMagnitude = Input.gyro.rotationRate.magnitude;
    magneticFieldMagnitude = magneticFieldBaseLine = Input.compass.rawVector.magnitude;
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
    // If the tilt hasn't changed, but the compass has, then the magnetic field moved without the device
    // This is the essence of a cardboard magnet click
    accelerationMagnitude = ImpulseFilter(accelerationMagnitude, newAccelerationMagnitude, fastAccelerationImpulseFilter);
    gyroRotationMagnitude = ImpulseFilter(gyroRotationMagnitude, newGyroRotationMagnitude, fastRotationImpulseFilter);
    magneticFieldMagnitude = ImpulseFilter(magneticFieldMagnitude, newMagneticFieldMagnitude, fastMagnetImpulseFilter);
    magneticFieldBaseLine = ImpulseFilter(magneticFieldBaseLine, newMagneticFieldMagnitude, slowImpulseFilter);
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
    return magneticFieldRatio > 1.1;
  }

  public bool IsUp() {
    return magneticFieldRatio < 0.9;
  }
}
