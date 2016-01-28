using UnityEngine;
using UnityEngine.EventSystems; // Only for the SDK fix
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using CardboardControlDelegates;

/**
* Bring all the control scripts together to provide a convenient API
*/
public class CardboardControl : MonoBehaviour {
  [HideInInspector]
  public CardboardControlTrigger trigger;
  [HideInInspector]
  public CardboardControlGaze gaze;
  [HideInInspector]
  public CardboardControlBox box;
  [HideInInspector]
  public CardboardControlReticle reticle;

  private const int RIGHT_ANGLE = 90; 

  // This variable determinates if the player will move or not 
  private bool isWalking = false;

  CardboardHead head = null;

  //This is the variable for the player speed
  [Tooltip("With this speed the player will move.")]
  public float speed;

  [Tooltip("Activate this checkbox if the player shall move when the Cardboard trigger is pulled.")]
  public bool walkWhenTriggered;

  [Tooltip("Activate this checkbox if the player shall move when he looks below the threshold.")]
  public bool walkWhenLookDown;

  [Tooltip("This has to be an angle from 0° to 90°")]
  public double thresholdAngle;

  [Tooltip("Activate this Checkbox if you want to freeze the y-coordiante for the player. " +
	"For example in the case of you have no collider attached to your CardboardMain-GameObject" +
	"and you want to stay in a fixed level.")]
  public bool freezeYPosition;

  [Tooltip("This is the fixed y-coordinate.")]
  public float yOffset;

  void Start () 
  {
	head = Camera.main.GetComponent<StereoController>().Head;
  }

  private const float TIME_TO_CALIBRATE = 1f;
  private Dictionary<string,float> cooldownCounter = new Dictionary<string,float>() {
    {"OnUp", TIME_TO_CALIBRATE}, // trigger
    {"OnDown", TIME_TO_CALIBRATE},
    {"OnClick", TIME_TO_CALIBRATE},
    {"OnChange", TIME_TO_CALIBRATE}, // gaze
    {"OnStare", TIME_TO_CALIBRATE},
    {"OnTilt", TIME_TO_CALIBRATE} // box
  };
  public float eventCooldown = 0.2f;

  public void Awake() {
    trigger = gameObject.GetComponent<CardboardControlTrigger>();
    gaze = gameObject.GetComponent<CardboardControlGaze>();
    box = gameObject.GetComponent<CardboardControlBox>();
    reticle = gameObject.GetComponent<CardboardControlReticle>();
    InstantiateCardboardSDKReticleObject();
  }

  public void Update() {
    List<string> keys = new List<string>(cooldownCounter.Keys);
    foreach(string key in keys) {
      if (cooldownCounter[key] > 0f) cooldownCounter[key] -= Time.deltaTime;
    }
	// Walk when the Cardboard Trigger is used 
	if (walkWhenTriggered && !walkWhenLookDown && !isWalking && Cardboard.SDK.Triggered) 
	{
		isWalking = true;
	} 
	else if (walkWhenTriggered && !walkWhenLookDown && isWalking && Cardboard.SDK.Triggered) 
	{
		isWalking = false;
	}

	if (isWalking) 
	{
		Vector3 direction = new Vector3(head.transform.forward.x, 0, head.transform.forward.z).normalized * speed * Time.deltaTime;
		Quaternion rotation = Quaternion.Euler(new Vector3(0, -transform.rotation.eulerAngles.y, 0));
		transform.Translate(rotation * direction);
	}

	if(freezeYPosition)
	{
		transform.position = new Vector3(transform.position.x, yOffset, transform.position.z);
	}
  }

  public bool EventReady(string name) {
    if (!CooldownEnabledFor(name) || CooledDown(name)) {
      cooldownCounter[name] = eventCooldown;
      return true;
    }
    return false;
  }

  private bool CooldownEnabledFor(string name) {
    if (name == "OnTilt") return box.useEventCooldowns;
    if (name == "OnChange" || name == "OnStare") return gaze.useEventCooldowns;
    return trigger.useEventCooldowns;
  }

  private bool CooledDown(string name) {
    return cooldownCounter[name] <= 0;
  }

  private void InstantiateCardboardSDKReticleObject() {
    // These allow the reticle to know when it has to focus
    gameObject.AddComponent<GazeInputModule>();
    Camera.main.gameObject.AddComponent<PhysicsRaycaster>();
    // The object itself
    GameObject reticlePrefab = Instantiate(Resources.Load("CardboardReticle")) as GameObject;
    reticlePrefab.transform.parent = Camera.main.transform;
    reticlePrefab.transform.localPosition = Vector3.zero;
    reticlePrefab.transform.localEulerAngles = Vector3.zero;
    reticlePrefab.name = "CardboardReticle";
  }
}
