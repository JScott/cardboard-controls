using UnityEngine;
using System.Collections;

public class CardboardControlPointer : MonoBehaviour {
	private GameObject pointer;
  public GameObject pointerPrefab;
  public LayerMask raycastIgnoreLayer = 1 << Physics.IgnoreRaycastLayer;

  void Start () {
    pointer = Instantiate(pointerPrefab) as GameObject;
    GameObject head = GameObject.Find("CardboardMain/Head");
    pointer.transform.parent = head.transform;
    pointer.layer = LayerMask.NameToLayer("Ignore Raycast");
	}

  // TODO: public bool startHidden = false;
  // TODO: void Hide()
  // TODO: void Show()
  // TODO: void ChangeColor(Color color)
  // NOTE: Be sure to fade alpha/colors, never abruptly jump
}
