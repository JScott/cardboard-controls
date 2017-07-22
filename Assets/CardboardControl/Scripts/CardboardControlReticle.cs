using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CardboardControlReticle : MonoBehaviour {
  public float fadeTime = 0.6f;
  public bool startHidden = true;
  public LayerMask layerMask = Physics.DefaultRaycastLayers;
  private GvrReticlePointer reticle;
  private ColorFade colorFade = new ColorFade();
  private AlphaFade alphaFade = new AlphaFade();

  void Start() {
    reticle = Camera.main.gameObject.GetComponentInChildren<GvrReticlePointer>();
    if (startHidden) {
      alphaFade.target = 0f;
      alphaFade.source = 0f;
    }
  }

  void Update() {
    colorFade.UpdateCounter(fadeTime, reticle);
    alphaFade.UpdateCounter(fadeTime, reticle);
    Color newColor = colorFade.CurrentColor();
    newColor.a = alphaFade.CurrentAlpha();
    reticle.GetComponent<Renderer>().material.color = newColor;
    Camera.main.gameObject.GetComponent<GvrPointerPhysicsRaycaster>().eventMask = layerMask;
  }

  private void SetPositionOn(GameObject head) {
    Vector3 newPosition = head.transform.position;
    newPosition += head.transform.forward*20f;
    reticle.transform.position = newPosition;
  }

  private void SetRotationOn(GameObject head) {
    Vector3 oldRotation = reticle.transform.localEulerAngles;
    reticle.transform.LookAt(head.transform);
    reticle.transform.localEulerAngles -= oldRotation;
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


  // Helper classes

  private abstract class FadeState {
    public float counter = 1f;
    public float fadeTime;
    public GvrReticlePointer reticle;
    public virtual Color CurrentColor() { return Color.white; }
    public virtual float CurrentAlpha() { return 1f; }
    public virtual void Interrupt() {}
    public float PercentageFaded(float counter) {
      return (fadeTime - counter) / fadeTime;
    }
    public void UpdateCounter(float fadeTime, GvrReticlePointer reticle) {
      this.fadeTime = fadeTime;
      this.reticle = reticle;
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
      target = reticle.GetComponent<Renderer>().material.color;
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
      target = reticle.GetComponent<Renderer>().material.color.a;
      source = target;
      ResetCounter();
    }
  }
}
