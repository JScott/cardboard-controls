using UnityEngine;
using System.Collections;
using CardboardInputDelegates;

/**
* Creating a vision raycast and handling the data from it
* Relies on Google Cardboard SDK API's
*/
public class CardboardControlGaze {
  public float maxDistance = Mathf.Infinity;
  public LayerMask layerMask = Physics.DefaultRaycastLayers;
  public bool vibrateOnChange = false;
  
  private GameObject recentlyFocusedObject = null;
  private float focusStartTime = 0f;
  private CardboardHead head;
  
  // Hide in inspector?
  public RaycastHit focus;
  public bool focused;
  public CardboardInputDelegate OnChange = delegate {};

  public CardboardControlGaze() {
    StereoController stereoController = Camera.main.GetComponent<StereoController>();
    head = stereoController.Head;
  }
  
  public void Update() {
    focused = Physics.Raycast(head.Gaze, out focus, maxDistance, layerMask);
    CheckRaycastFocus();
  }

  private void CheckRaycastFocus() {
    if (recentlyFocusedObject != FocusedObject()) ReportFocusChange();
    recentlyFocusedObject = FocusedObject();
  }

  private void ReportFocusChange() {
    OnChange(this, new CardboardInputEvent());
    //if (debugNotificationsEnabled) Debug.Log(" *** Focus Changed *** ");
    if (vibrateOnChange) Handheld.Vibrate();
    focusStartTime = Time.time;
  }

  public bool IsFocused() {
    return focused;
  }

  public float SecondsFocused() {
    if (focusStartTime == 0f) return 0f;
    return Time.time - focusStartTime;
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
