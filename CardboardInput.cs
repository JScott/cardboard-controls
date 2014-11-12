/*     -- MIT/X11 like license --

Copyright (c) 2014 Paramita ltd, (Secret Ingredient Games) & Justin Scott

Permission is hereby granted, free of charge, to any person obtaining a
copy of this software and associated documentation files (the "Software"),
to deal in the Software without restriction, including without limitation the
rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the
Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included
in all copies or substantial portions of the Software source.

Users download a free copy of Secret Ingredient Games':
https://play.google.com/store/apps/details?id=com.secretingredientgames.tiltGolf

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

//
//  Google Cardboard click code in C# for Unity.
//  Author: Andrew Whyte & Justin Scott
//
using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

//public static XmlDocument XmlDoc;
//public static XmlNodeList xnl;
//public TextAsset TA;

public class CardboardInput {
  //  Concept: two FIR filters,  running on Magnetics and tilt.
  //  If the tilt hasn't changed, but the compass has, then the magnetic field moved
  //  without device this is the essence of a cardboard magnet click.
  private Vector3 lastTiltVector;
  public float tiltOffsetBaseLine = 0f;
  public float compassBaseLine = 0f;

  public float tiltOffsetMagnitude = 0f;
  public float compassMagnitude = 0f;

  // Finite Impulse Response filters
  private int slowImpulseFilter = 25;
  private int fastCompassImpulseFilter = 3;
  private int fastTiltOffsetImpulseFilter = 5;  // Clicking the magnet tends to tilt the device slightly.


  public float clickSpeedThreshold = 0.4f;
  private float clickStartTime = 0f;

  bool upReported = false;
  bool downReported = true; // down is triggered once as it finds baselines
  bool clickReported = false;
  bool click = false;

  private bool magnetHeld = false;

  public delegate void MagnetAction(object sender, CardboardEvent cardboardEvent);

  public MagnetAction OnMagnetDown = delegate {};
  public MagnetAction OnMagnetUp = delegate {};
  public MagnetAction OnMagnetMoved = delegate {};
  public MagnetAction OnMagnetClicked = delegate {};

  public CardboardInput() {
    Input.compass.enabled = true;

    // Note that init is platform specific to unity.
    compassMagnitude = Input.compass.rawVector.magnitude;
    compassBaseLine = Input.compass.rawVector.magnitude;
    tiltOffsetBaseLine = Input.acceleration.magnitude;
    tiltOffsetMagnitude = Input.acceleration.magnitude;
  }

  public float ImpulseFilter(float from_value, float to_value, int filter) {
    return ((filter-1) * from_value + to_value) / filter;
  }

  public void Update(Vector3 acc,  Vector3 compass) {
    // We are interested in the change of the tilt, not the actual tilt
    Vector3 TiltNow = acc;
    Vector3 motionVec3 = TiltNow - lastTiltVector;
    lastTiltVector = TiltNow;

    // Update our magnitudes and baselines through the appropriate impulse filters
    tiltOffsetMagnitude = this.ImpulseFilter(tiltOffsetMagnitude, motionVec3.magnitude, fastTiltOffsetImpulseFilter);
    tiltOffsetBaseLine = this.ImpulseFilter(tiltOffsetBaseLine, motionVec3.magnitude, slowImpulseFilter);

    compassMagnitude = this.ImpulseFilter(compassMagnitude, compass.magnitude, fastCompassImpulseFilter);
    compassBaseLine = this.ImpulseFilter(compassBaseLine, compass.magnitude, slowImpulseFilter);

    bool notJostled = tiltOffsetMagnitude < 0.2;
    bool magnetMovedDown = (compassMagnitude / compassBaseLine) > 1.1;
    bool magnetMovedUp = (compassMagnitude / compassBaseLine) < 0.9;
    bool magnetMoved = magnetMovedUp || magnetMovedDown;

    if (notJostled) {
      if (magnetMovedDown) ReportDown(compass);
      else downReported = false;

      if (magnetMovedUp) ReportUp();
      else {
        upReported = false;
        clickReported = false;
      }

      if (magnetMoved) {
        OnMagnetMoved(this, new CardboardEvent());
      }
    }
  }

  private void ReportDown(Vector3 compass) {
    if (downReported == false && compass.z > 100) { // random int is measuring speed
      OnMagnetDown(this, new CardboardEvent());
      downReported = true;
      magnetHeld = true;
      clickStartTime = Time.time;
    }
  }

  private void ReportUp() {
    if (upReported == false) {
      OnMagnetUp(this, new CardboardEvent());
      CheckForClick();
      upReported = true;
      magnetHeld = false;
    }
  }

  private void CheckForClick() {
    bool withinClickThreshold = SecondsMagnetHeld() <= clickSpeedThreshold;
    clickStartTime = 0f;
    if (withinClickThreshold) {
      if(clickReported == false) {
        click = true;
        OnMagnetClicked(this, new CardboardEvent());
      }
      clickReported = true;
    }
  }

  public float SecondsMagnetHeld() {
    if (clickStartTime == 0f) return 0f;
    return Time.time - clickStartTime;
  }

  public bool IsMagnetHeld() {
    return magnetHeld;
  }

  public bool WasClicked()  {
    if(click == true) {
      click = false;
      return true;
    } else {
      return false;
    }
  }
}