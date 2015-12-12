using UnityEngine;
using System.Collections;

public class CardboardControlPointer : MonoBehaviour {
	private GameObject pointer;
  private Color targetColor = Color.white;
  private Color previousColor = Color.white;
  private float fadeColorCounter = 0f;
  private float targetAlpha = 1f;
  private float previousAlpha = 1f;
  private float fadeAlphaCounter = 0f;

  public GameObject pointerPrefab;
  public LayerMask raycastIgnoreLayer = 1 << Physics.IgnoreRaycastLayer;
  public float fadeTime = 0.6f;
  public bool startHidden = false;

  void Start () {
    pointer = Instantiate(pointerPrefab) as GameObject;
    GameObject head = GameObject.Find("CardboardMain/Head");
    SetPositionOn(head);
    SetRotationOn(head);
    pointer.GetComponent<Renderer>().material.renderQueue = int.MaxValue;
    pointer.transform.parent = head.transform;
    pointer.layer = LayerMask.NameToLayer("Ignore Raycast");
    if (startHidden) targetAlpha = 0f;
	}

  void Update() {
    if (fadeColorCounter > 0f || fadeAlphaCounter > 0f) {
      float colorPercentage = (fadeTime - fadeColorCounter) / fadeTime;
      float alphaPercentage = (fadeTime - fadeAlphaCounter) / fadeTime;

      Color newColor = Color.Lerp(previousColor, targetColor, colorPercentage);
      newColor.a = Mathf.Lerp(previousAlpha, targetAlpha, alphaPercentage);
      pointer.GetComponent<Renderer>().material.color = newColor;

      fadeColorCounter -= Time.deltaTime;
      if (fadeColorCounter < 0f) fadeColorCounter = 0f;
      fadeAlphaCounter -= Time.deltaTime;
      if (fadeAlphaCounter < 0f) fadeAlphaCounter = 0f;
    }
    else {
      Color newColor = targetColor;
      newColor.a = targetAlpha;
      pointer.GetComponent<Renderer>().material.color = newColor;
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

  private void FadeColorTo(Color color) {
    previousColor = targetColor;
    targetColor = color;
  }

  private void FadeAlphaTo(float alpha) {
    previousAlpha = targetAlpha;
    targetAlpha = alpha;
  }

  private void InterruptColorFade() {
    targetColor = pointer.GetComponent<Renderer>().material.color;
    fadeColorCounter = fadeTime - fadeColorCounter;
  }

  private void InterruptAlphaFade() {
    targetColor.a = pointer.GetComponent<Renderer>().material.color.a;
    fadeAlphaCounter = fadeTime - fadeAlphaCounter;
  }

  public void Highlight(Color color) {
    InterruptColorFade();
    FadeColorTo(color);
  }

  public void ClearHighlight() {
    InterruptColorFade();
    FadeColorTo(Color.white);
  }

  public void Hide() {
    InterruptAlphaFade();
    FadeAlphaTo(0f);
  }

  public void Show() {
    InterruptAlphaFade();
    FadeAlphaTo(1f);
  }

  // Create github issue:
  //   Move Highlight, ClearHighlight, Hide, and Show to the pointer object itself
  //   in order to allow custom pointers
}
