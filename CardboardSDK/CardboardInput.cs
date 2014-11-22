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

public class CardboardInput : MonoBehaviour {
  private Vector3 lastTiltVector;
  private float tiltOffsetBaseLine = 0f;
  private float magneticFieldBaseLine = 0f;

  private float tiltOffsetMagnitude = 0f;
  private float magneticFieldMagnitude = 0f;

  private int slowImpulseFilter = 25;
  private int fastMagnetImpulseFilter = 3;
  private int fastTiltImpulseFilter = 5;  // Clicking the magnet tends to tilt the device slightly.

  public float clickSpeedThreshold = 0.4f;
  private float clickStartTime = 0f;

  private bool upReported = false;
  private bool downReported = true; // down is triggered once as it finds baselines
  private bool clickReported = false;
  private bool click = false;

  private bool magnetHeld = false;

  public bool vibrateOnMagnetDown = false;
  public bool vibrateOnMagnetUp = false;
  public bool vibrateOnMagnetClicked = true;

  public delegate void CardboardAction(object sender, CardboardEvent cardboardEvent);
  public CardboardAction OnMagnetDown = delegate {};
  public CardboardAction OnMagnetUp = delegate {};
  public CardboardAction OnMagnetClicked = delegate {};

  public void Start() {
    Input.compass.enabled = true;
    magneticFieldMagnitude = Input.compass.rawVector.magnitude;
    magneticFieldBaseLine = Input.compass.rawVector.magnitude;
    tiltOffsetMagnitude = Input.acceleration.magnitude;
    tiltOffsetBaseLine = Input.acceleration.magnitude;
  }

  public float ImpulseFilter(float from_value, float to_value, int filter) {
    return ((filter-1) * from_value + to_value) / filter;
  }

  public void Update() {
    Vector3 magneticField = Input.compass.rawVector;
    Vector3 tiltNow = Input.acceleration;
    Vector3 tiltOffset = tiltNow - lastTiltVector;
    lastTiltVector = tiltNow;

    // Apply Finite Impulse Response (FIR) filters
    // If the tilt hasn't changed, but the compass has, then the magnetic field moved
    // without device this is the essence of a cardboard magnet click.
    tiltOffsetMagnitude = ImpulseFilter(tiltOffsetMagnitude, tiltOffset.magnitude, fastTiltImpulseFilter);
    tiltOffsetBaseLine = ImpulseFilter(tiltOffsetBaseLine, tiltOffset.magnitude, slowImpulseFilter);
    magneticFieldMagnitude = ImpulseFilter(magneticFieldMagnitude, magneticField.magnitude, fastMagnetImpulseFilter);
    magneticFieldBaseLine = ImpulseFilter(magneticFieldBaseLine, magneticField.magnitude, slowImpulseFilter);

    bool notJostled = tiltOffsetMagnitude < 0.2;
    bool magnetMovedDown = (magneticFieldMagnitude / magneticFieldBaseLine) > 1.11;
    bool magnetMovedUp = (magneticFieldMagnitude / magneticFieldBaseLine) < 0.97;
    if (Debug.isDebugBuild) {
      magnetMovedDown = magnetMovedDown || Input.GetButtonDown("Jump");
      magnetMovedUp = magnetMovedUp || Input.GetButtonUp("Jump");
    }
    
    if (notJostled) {
      if (magnetMovedDown) ReportDown();
      else downReported = false;

      if (magnetMovedUp) ReportUp();
      else upReported = false;
    }
  }

  public void ReportDown() {
    if (downReported == false) {
      if (Debug.isDebugBuild) Debug.Log(" *** Magnet Down *** ");
      OnMagnetDown(this, new CardboardEvent());
      if (vibrateOnMagnetDown) Vibrate();
      downReported = true;
      magnetHeld = true;
      clickStartTime = Time.time;
    }
  }

  public void ReportUp() {
    if (upReported == false) {
      if (Debug.isDebugBuild) Debug.Log(" *** Magnet Up *** ");
      OnMagnetUp(this, new CardboardEvent());
      if (vibrateOnMagnetUp) Vibrate();
      upReported = true;
      magnetHeld = false;
      CheckForClick();
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
      click = true;
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

  public bool WasClicked()  {
    if(click == true) {
      click = false;
      return true;
    }
    else {
      return false;
    }
  }
}