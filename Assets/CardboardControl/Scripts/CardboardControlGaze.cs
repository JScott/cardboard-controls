using UnityEngine;
using System.Collections;
using CardboardControlDelegates;

/**
* Creating a vision raycast and handling the data from it
* Relies on Google VR's API
*/
public class CardboardControlGaze : MonoBehaviour {
  public float maxDistance = Mathf.Infinity;
  public LayerMask layerMask = Physics.DefaultRaycastLayers;
  public bool useEventCooldowns = false;
  public bool vibrateOnChange = false;
  public bool vibrateOnStare = false;
  public float stareTimeThreshold = 2.0f;

  private Transform camera;
  private GameObject previousObject = null;
  private GameObject currentObject = null;
  private float gazeStartTime = 0f;
  private RaycastHit hit;
  private bool isHeld;
  private bool stared = false;

  private CardboardControl cardboard;
  public CardboardControlDelegate OnChange = delegate {};
  public CardboardControlDelegate OnStare = delegate {};

  public void Start() {
    cardboard = gameObject.GetComponent<CardboardControl>();
    camera = Camera.main.transform;
  }

  public void Update() {
    isHeld = Physics.Raycast(Ray(), out hit, maxDistance, layerMask);
    CheckGaze();
  }

  private void CheckGaze() {
    if (GazeChanged() && cardboard.EventReady("OnChange")) ReportGazeChange();
    if (!stared && Staring() && cardboard.EventReady("OnStare")) ReportStare();
    currentObject = Object();
  }

  private bool Staring() {
    return SecondsHeld() > stareTimeThreshold;
  }

  private bool GazeChanged() {
    if (currentObject != Object()) {
      previousObject = currentObject;
      stared = false;
      return true;
    }
    return false;
  }

  private void ReportGazeChange() {
    OnChange(this);
    if (vibrateOnChange) Handheld.Vibrate();
    gazeStartTime = Time.time;
  }

  private void ReportStare() {
    OnStare(this);
    if (vibrateOnStare) Handheld.Vibrate();
    stared = true;
  }

  public bool IsHeld() {
    return isHeld;
  }

  public bool WasHeld() {
    return previousObject != null;
  }

  public float SecondsHeld() {
    return Time.time - gazeStartTime;
  }

  public RaycastHit Hit() {
    return hit;
  }

  public GameObject Object() {
    if (IsHeld()) {
      return hit.transform.gameObject;
    }
    return null;
  }

  public GameObject PreviousObject() {
    return previousObject;
  }

  public Vector3 Forward() {
    return Ray().direction.normalized;
  }

  public Vector3 Right() {
    return Vector3.Cross(Forward(), Vector3.up);
  }

  public Quaternion Rotation() {
    return Quaternion.LookRotation(Forward());
  }

  public Ray Ray() {
    return new Ray(camera.position, camera.forward);
  }
}
