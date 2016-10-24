using UnityEngine;
using System.Collections;

/**
* Dealing with raw touch input from a Cardboard device
*/
public class ParsedTouchData {
  private bool wasTouched = false;

  public ParsedTouchData() {}

  private GameObject CardboardGameObject() {
    GameObject gameObject = Camera.main.gameObject;
    return gameObject.transform.parent.parent.gameObject;
  }

  public void Update() {
    wasTouched |= IsDown();
  }

  public bool IsDown() {
    // touchCount can jump for no reason in a Cardboard
    // but it's too quick to be considered "Moved"
    return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved;
  }

  public bool StillDown() {
    return Input.touchCount > 0;
  }

  public bool IsUp() {
    if (!StillDown() && wasTouched) {
      wasTouched = false;
      return true;
    }
    return false;
  }

  public void PrintDebug() {
    Debug.Log("--- Touch\ncount: " + Input.touchCount +
              "\ntouched: " + wasTouched);
  }
}
