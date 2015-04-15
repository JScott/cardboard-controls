using UnityEngine;
using System.Collections;

public class CardboardEvent {
  public CardboardRaycast raycast;

  public CardboardEvent() {
    raycast = null;
  }
  public CardboardEvent(CardboardRaycast _raycast) {
    raycast = _raycast;
  }
}
