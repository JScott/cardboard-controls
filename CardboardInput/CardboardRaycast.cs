using UnityEngine;
using System.Collections;

/**
* Creating a vision raycast and handling the data from it
* Relies on Google Cardboard SDK API's
*/
public class CardboardRaycast {
  public float maxDistance = Mathf.Infinity;
  public LayerMask layerMask = Physics.DefaultRaycastLayers;

  private CardboardHead head;
  private RaycastHit focus;
  private bool focused;

  public CardboardRaycast() {
    StereoController stereoController = Camera.main.GetComponent<StereoController>();
    head = stereoController.Head;
  }
  
  public void Update() {
    focused = Physics.Raycast(head.Gaze, out focus, maxDistance, layerMask);
  }

  public bool IsFocused() {
    return focused;
  }

  public RaycastHit Focus() {
    return focus;
  }

  public GameObject FocusedObject() {
    if (IsFocused()) {
      return focus.transform.gameObject;
    } else {
      return null;
    }
  }
}
