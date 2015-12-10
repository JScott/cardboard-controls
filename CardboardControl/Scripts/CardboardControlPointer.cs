using UnityEngine;
using System.Collections;

public class CardboardControlPointer : MonoBehaviour {
	private GameObject pointer;
  private Color targetColor = Color.white;
  private Color previousColor = Color.white;
  private float targetScale = 1f;
  private float previousScale = 1f;
  private float fadeCounter = 0f;

  public GameObject pointerPrefab;
  public LayerMask raycastIgnoreLayer = 1 << Physics.IgnoreRaycastLayer;
  public float fadeTime = 0.7f;

  void Start () {
    pointer = Instantiate(pointerPrefab) as GameObject;
    GameObject head = GameObject.Find("CardboardMain/Head");
    SetPositionOn(head);
    SetRotationOn(head);
    pointer.GetComponent<Renderer>().material.renderQueue = int.MaxValue;
    pointer.transform.parent = head.transform;
    pointer.layer = LayerMask.NameToLayer("Ignore Raycast");
	}

  void Update() {
    if (fadeCounter > 0f) {
      float percentage = (fadeTime - fadeCounter) / fadeTime;
      Color newColor = Color.Lerp(previousColor, targetColor, percentage);
      pointer.GetComponent<Renderer>().material.color = newColor;
      fadeCounter -= Time.deltaTime;
    }
    else {
      pointer.GetComponent<Renderer>().material.color = targetColor;
    }
  }

  private void SetPositionOn(GameObject head) {
    Vector3 newPosition = head.transform.position;
    newPosition += head.transform.forward*20f;
    pointer.transform.position = newPosition;
  }

  private void SetRotationOn(GameObject head) {
    Vector3 oldRotation = pointer.transform.localEulerAngles;
    pointer.transform.LookAt(head.transform);
    pointer.transform.localEulerAngles -= oldRotation;
  }

  private void FadeTo(Color color) {
    previousColor = targetColor;
    targetColor = color;
  }

  // private void ScaleTo(float scale) {
  //   previousScale = targetScalet;
  //   targetScale = scale;
  // }

  public void Highlight(Color color) {
    if (fadeCounter <= 0f) {
      fadeCounter = fadeTime;
      FadeTo(color);
      // ScaleTo(2f);
    }
  }

  public void ClearHighlight() {
    if (fadeCounter <= 0f) {
      fadeCounter = fadeTime;
      FadeTo(Color.white);
      // ScaleTo(0.5f);
    }
  }

  // TODO: public bool startHidden = false;
  // TODO: void Hide()
  // TODO: void Show()
  // TODO: void ChangeColor(Color color)
  // NOTE: Be sure to fade alpha/colors, never abruptly jump
}
