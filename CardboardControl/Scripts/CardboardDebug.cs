using UnityEngine;
using System.Collections;

/**
* Handling some more complex methods for dealing with debug data
*/
public class CardboardDebug {
  public KeyCode magnetKey;
  public KeyCode orientationKey;

  public CardboardDebug(KeyCode _magnetKey, KeyCode _orientationKey) {
    magnetKey = _magnetKey;
    orientationKey = _orientationKey;
  }

  public bool KeyFor(string forInput) {
    switch(forInput) {
      case "orientationTilt":
        return Input.GetKeyDown(orientationKey);
      default:
        return false;
    }
  }

  public string Charts(bool isMagnetHeld, bool isTiltReported, string magnetReadingsChart) {
    return magnetReadingsChart + "\n" + MagnetStateChart(isMagnetHeld, isTiltReported);
  }

  private string MagnetStateChart(bool isMagnetHeld, bool isTiltReported) {
    string chart = "";
    chart += "Magnet State\n";
    chart += (isMagnetHeld) ? "U " : "x ";
    chart += (!isMagnetHeld) ? "D " : "x ";
    chart += isTiltReported ? "T " : "x ";
    chart += "\n";
    return chart;
  }
}
