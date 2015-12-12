using UnityEngine;
using System.Collections;

public class CardboardControlPointer : MonoBehaviour {
  public GameObject pointerPrefab;
  public LayerMask raycastIgnoreLayer = 1 << Physics.IgnoreRaycastLayer;
  public float fadeTime = 0.6f;
  public bool startHidden = false;

  private GameObject pointer;
  private ColorFade colorFade = new ColorFade();
  private AlphaFade alphaFade = new AlphaFade();

  private abstract class FadeState {
    public float counter = 1f;
    public float fadeTime;
    public GameObject pointer;
    public virtual Color CurrentColor() { return Color.white; }
    public virtual float CurrentAlpha() { return 1f; }
    public virtual void Interrupt() {}
    public float PercentageFaded(float counter) {
      return (fadeTime - counter) / fadeTime;
    }
    public void UpdateCounter(float fadeTime, GameObject pointer) {
      this.fadeTime = fadeTime;
      this.pointer = pointer;
      counter -= Time.deltaTime;
      if (counter < 0f) counter = 0f;
    }
    public void ResetCounter() {
      counter = fadeTime - counter;
    }
  }

  private class ColorFade : FadeState {
    public Color target = Color.white;
    public Color source = Color.white;
    public override Color CurrentColor() {
      return Color.Lerp(source, target, PercentageFaded(counter));
    }
    public override void Interrupt() {
      target = pointer.GetComponent<Renderer>().material.color;
      source = target;
      ResetCounter();
    }
  }

  private class AlphaFade : FadeState {
    public float target = 1f;
    public float source = 1f;
    public override float CurrentAlpha() {
      return Mathf.Lerp(source, target, PercentageFaded(counter));
    }
    public override void Interrupt() {
      target = pointer.GetComponent<Renderer>().material.color.a;
      source = target;
      ResetCounter();
    }
  }

  void Start() {
    InitializePointerObject();
    if (startHidden) {
      alphaFade.target = 0f;
      alphaFade.source = 0f;
    }
	}

  void Update() {
    colorFade.UpdateCounter(fadeTime, pointer);
    alphaFade.UpdateCounter(fadeTime, pointer);
    if (colorFade.counter > 0f || alphaFade.counter > 0f) {
      Color newColor = colorFade.CurrentColor();
      newColor.a = alphaFade.CurrentAlpha();
      pointer.GetComponent<Renderer>().material.color = newColor;
    }
  }

  private void InitializePointerObject() {
    pointer = Instantiate(pointerPrefab) as GameObject;
    GameObject head = GameObject.Find("CardboardMain/Head");
    SetPositionOn(head);
    SetRotationOn(head);
    pointer.GetComponent<Renderer>().material.renderQueue = int.MaxValue;
    pointer.transform.parent = head.transform;
    pointer.layer = LayerMask.NameToLayer("Ignore Raycast");
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

  public void Highlight(Color color) {
    colorFade.Interrupt();
    colorFade.target = color;
  }

  public void ClearHighlight() {
    colorFade.Interrupt();
    colorFade.target = Color.white;
  }

  public void Hide() {
    alphaFade.Interrupt();
    alphaFade.target = 0f;
  }

  public void Show() {
    alphaFade.Interrupt();
    alphaFade.target = 1f;
  }
}
