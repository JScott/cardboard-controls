using UnityEngine;
using System.Collections;

public class CardboardControlPointer : MonoBehaviour {
	private GameObject pointer;
  private Color targetColor = Color.white;
  private Color previousColor = Color.white;
  private float fadeCounter = 0f;

  public GameObject pointerPrefab;
  public LayerMask raycastIgnoreLayer = 1 << Physics.IgnoreRaycastLayer;
  public float fadeTime = 0.6f;

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

  private void InterruptFade() {
    targetColor = pointer.GetComponent<Renderer>().material.color;
    fadeCounter = fadeTime - fadeCounter;
  }

  public void Highlight(Color color) {
    if (fadeCounter <= 0f) {
      fadeCounter = fadeTime;
      FadeTo(color);
    }
  }

  public void ClearHighlight() {
    InterruptFade();
    FadeTo(Color.white);
  }

  public void Hide() {
    Color transparentColor = targetColor;
    transparentColor.a = 0;
    InterruptFade();
    FadeTo(transparentColor);
  }

  public void Show() {
    Color transparentColor = targetColor;
    transparentColor.a = 1;
    InterruptFade();
    FadeTo(transparentColor);
  }

  // TODO: using both show and clearhighlight is abrupt. Change alpha independently

  // Create github issue:
  //   Move Highlight, ClearHighlight, Hide, and Show to the pointer object itself
  //   in order to allow custom pointers

  // TODO: public bool startHidden = false;
}
