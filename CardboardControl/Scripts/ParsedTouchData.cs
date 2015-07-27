using UnityEngine;
using System.Collections;

/**
* Dealing with raw touch input from a Cardboard device
*/
public class ParsedTouchData {
  private bool wasTouched = false;

  public ParsedTouchData() {
    Cardboard cardboard = CardboardGameObject().GetComponent<Cardboard>();
    cardboard.TapIsTrigger = false;
  }

  private GameObject CardboardGameObject() {
    GameObject gameObject = Camera.main.gameObject;
    return gameObject.transform.parent.parent.gameObject;
  }

  public void Update() {
    wasTouched |= IsDown();
  }

  public bool IsDown() {
    return Input.touchCount > 0;
  }

  public bool IsUp() {
    if (!IsDown() && wasTouched) {
      wasTouched = false;
      return true;
    }
    return false;
  }
}
