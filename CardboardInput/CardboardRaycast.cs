using UnityEngine;
using System.Collections;

/**
* Creating a vision raycast and handling the data from it
* Relies on Google Cardboard SDK API's
*/
public class CardboardRaycast {
  public float maxDistance = Mathf.Infinity;
  public LayerMask layerMask = Physics.DefaultRaycastLayers;
  public RaycastHit focus;
  public bool focused;

  private CardboardHead head;

  public CardboardRaycast(float _maxDistance, LayerMask _layerMask) {
    StereoController stereoController = Camera.main.GetComponent<StereoController>();
    head = stereoController.Head;
    maxDistance = _maxDistance;
    layerMask = _layerMask;
  }
  
  public void Update() {
    focused = Physics.Raycast(head.Gaze, out focus, maxDistance, layerMask);
  }
}
