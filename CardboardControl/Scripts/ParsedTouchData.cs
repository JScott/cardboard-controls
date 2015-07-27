using UnityEngine;
using System.Collections;

/**
* Dealing with raw touch input from a Cardboard device
*/
public class ParsedTouchData {
  private bool wasTouched = false;

  public ParsedTouchData() {}

  public void Update() {
    wasTouched |= this.IsDown();
  }

  public bool IsDown() {
    return Input.touchCount > 0;
  }

  public bool IsUp() {
    if (!this.IsDown() && wasTouched) {
      wasTouched = false;
      return true;
    }
    return false;
  }
}
