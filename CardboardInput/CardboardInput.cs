using UnityEngine;
using System.Collections;

/**
* Dealing with raw input from a Cardboard device
*/
public class CardboardInput {
  private Vector3 lastTiltVector;

  private float tiltOffsetMagnitude;
  private float rotationRateMagnitude;
  private float magneticFieldMagnitude;
  private float magneticFieldBaseLine;
  private float magneticFieldRatio;  

  public bool notJostled = false;
  public bool notRotatedQuickly = false;
  public bool magnetMovedDown = false;
  public bool magnetMovedUp = false;

  private int slowImpulseFilter = 25;
  private int fastMagnetImpulseFilter = 3;
  private int fastRotationImpulseFilter = 3;
  private int fastTiltImpulseFilter = 5;  // Clicking the magnet tends to tilt the device slightly.

  private DeviceOrientation tiltedOrientation = DeviceOrientation.Portrait;

  public CardboardInput() {
    Input.compass.enabled = true;
    Input.gyro.enabled = true;
    tiltOffsetMagnitude = Input.acceleration.magnitude; // TODO: rename tiltoffset for clarity
    rotationRateMagnitude = Input.gyro.rotationRate.magnitude;
    magneticFieldMagnitude = Input.compass.rawVector.magnitude;
    magneticFieldBaseLine = Input.compass.rawVector.magnitude;
  }
  
  public void Update() {
    Vector3 magneticField = Input.compass.rawVector;
    Vector3 rotationRate = Input.gyro.rotationRate; // TODO: shouldn't this be the offset, like tilt?
    Vector3 tiltNow = Input.acceleration;
    Vector3 tiltOffset = tiltNow - lastTiltVector;
    lastTiltVector = tiltNow;

    // TODO: "magnitude" isn't terribly descriptive
    FilterAndSetMagnitudes(tiltOffset.magnitude, rotationRate.magnitude, magneticField.magnitude);
    magneticFieldRatio = (magneticFieldMagnitude / magneticFieldBaseLine);
  }

  public float ImpulseFilter(float from_value, float to_value, int filter) {
    return ((filter-1) * from_value + to_value) / filter;
  }

  // TODO: man, dictionaries would be great around this...
  public void FilterAndSetMagnitudes(float newTiltOffsetMagnitude, float newRotationRateMagnitude, float newMagneticFieldMagnitude) {
    // Apply Finite Impulse Response (FIR) filters
    // If the tilt hasn't changed, but the compass has, then the magnetic field moved
    // without device this is the essence of a cardboard magnet click.
    tiltOffsetMagnitude = ImpulseFilter(tiltOffsetMagnitude, newTiltOffsetMagnitude, fastTiltImpulseFilter);
    rotationRateMagnitude = ImpulseFilter(rotationRateMagnitude, newRotationRateMagnitude, fastRotationImpulseFilter);
    magneticFieldMagnitude = ImpulseFilter(magneticFieldMagnitude, newMagneticFieldMagnitude, fastMagnetImpulseFilter);
    magneticFieldBaseLine = ImpulseFilter(magneticFieldBaseLine, newMagneticFieldMagnitude, slowImpulseFilter);
  }

  public bool Jostled() {
    return tiltOffsetMagnitude >= 0.2;
  }

  public bool RotatedQuickly() {
    return rotationRateMagnitude >= 3.0;
  }

  public bool MagnetMovedDown() {
    return magneticFieldRatio > 1.11;
  }

  public bool MagnetMovedUp() {
    return magneticFieldRatio < 0.97;
  }

  public bool OrientationTilted() {
    return Input.deviceOrientation == tiltedOrientation;
  }

  public string MagnetReadingsChart() {
    string chart = "";
    chart += "Magnet Readings\n";
    chart += !Jostled() ? "***** steady " : "!!!!! jostled ";
    if (!Jostled()) {
      chart += MagnetMovedDown() ? "vvv " : "    ";
      chart += MagnetMovedUp() ? "^^^ " : "    ";
    }
    return chart;
  }
}
