/*     -- MIT/X11 like license --

Copyright (c) 2014 Paramita ltd, (Secret Ingredient Games)

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
//  Author: Andrew Whyte
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
  public float tiltedBaseLine = 0f;
  public float magnetBaseLine = 0f;

  public float tiltedMagn = 0f;
  public float magnetMagn = 0f;

  private int N_SlowFIR = 25;
  private int N_FastFIR_magnet = 3;
  private int N_FastFIR_tilted = 5;  // Clicking the magnet tends to tilt the device slightly.


  public float threshold = 1.0f;

  bool click = false;
  bool clickReported = false;

  public delegate void MagnetAction();
  public MagnetAction OnClick = delegate {};

  public CardboardInput() {
    Input.compass.enabled = true;

    // Note that init is platform specific to unity.
    magnetMagn = Input.compass.rawVector.magnitude;
    magnetBaseLine = Input.compass.rawVector.magnitude;
    tiltedBaseLine = Input.acceleration.magnitude;
    tiltedMagn = Input.acceleration.magnitude;
  }

  public void Update(Vector3 acc,  Vector3 compass) {
    // we are interested in the change of the tilt not the actual tilt.
    Vector3 TiltNow = acc;
    Vector3 motionVec3 = TiltNow - lastTiltVector;
    lastTiltVector = TiltNow;

    // update tilt and compass "fast" values
    tiltedMagn = ((N_FastFIR_tilted-1) * tiltedMagn + motionVec3.magnitude) / N_FastFIR_tilted;
    magnetMagn = ((N_FastFIR_magnet-1) * magnetMagn + compass.magnitude) / N_FastFIR_magnet;

    // update the "slow" values
    tiltedBaseLine = ( (N_SlowFIR-1) * tiltedBaseLine + motionVec3.magnitude) / N_SlowFIR;
    magnetBaseLine = ( (N_SlowFIR-1) * magnetBaseLine + compass.magnitude) / N_SlowFIR;

    if( tiltedMagn < 0.2 && (magnetMagn / magnetBaseLine) > 1.1  ) {
      if( clickReported == false) {
        click = true;
        OnClick();
      }
      clickReported = true;
    } else  {
      clickReported = false;
    }
  }

  public bool WasClicked()  {
    // Basic premise is that the magnitude of magnetic field should change while the
    // device is steady.  This seems to be suiltable for menus etc.

    // Clear the click by reading (so each 'click' returns true only once)
    if(click == true) {
      click = false;
      return true;
    } else {
      return false;
    }
  }
}