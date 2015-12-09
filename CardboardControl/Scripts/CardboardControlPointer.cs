using UnityEngine;
using System.Collections;

public class CardboardControlPointer : MonoBehaviour {
	private GameObject pointer;
  public GameObject pointerPrefab;
  public LayerMask raycastIgnoreLayer = 1 << Physics.IgnoreRaycastLayer;

  void Start () {
    pointer = Instantiate(pointerPrefab) as GameObject;
    GameObject head = GameObject.Find("CardboardMain/Head");
    SetPositionOn(head);
    // SetRotationOn(head);
    pointer.transform.parent = head.transform;
    pointer.layer = LayerMask.NameToLayer("Ignore Raycast");
	}

  void SetPositionOn(GameObject head) {
    Vector3 newPosition = head.transform.position;
    newPosition += head.transform.forward*20f;
    pointer.transform.position = newPosition;
  }

  void SetRotationOn(GameObject head) {
    // Not working yet. Try rotating the example character to see what I mean
    // Needs to respect the rotation of the character AND the prefab
    Vector3 newEulerAngles = pointer.transform.eulerAngles;
    newEulerAngles += head.transform.eulerAngles;
    pointer.transform.eulerAngles = newEulerAngles;
  }

  // TODO: public bool startHidden = false;
  // TODO: void Hide()
  // TODO: void Show()
  // TODO: void ChangeColor(Color color)
  // NOTE: Be sure to fade alpha/colors, never abruptly jump
}
