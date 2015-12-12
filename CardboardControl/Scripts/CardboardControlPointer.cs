using UnityEngine;
using System.Collections;

public class CardboardControlPointer : MonoBehaviour {
  private static GameObject pointer;
  private const int FOR_COLOR = 0;
  private const int FOR_ALPHA = 1;
  private const int TARGET = 0;
  private const int SOURCE = 1;

  public static float fadeTime = 0.6f;
  public static bool startHidden = false;
  private ColorFade colorFade;
  private AlphaFade alphaFade;

  public GameObject pointerPrefab;
  public LayerMask raycastIgnoreLayer = 1 << Physics.IgnoreRaycastLayer;

  void Start () {
    InitializeGameObject();
    InitializeFadeState();
  }

  private void InitializeGameObject() {
    pointer = Instantiate(pointerPrefab) as GameObject;
    GameObject head = GameObject.Find("CardboardMain/Head");
    SetPositionOn(head);
    SetRotationOn(head);
    pointer.GetComponent<Renderer>().material.renderQueue = int.MaxValue;
    pointer.transform.parent = head.transform;
    pointer.layer = LayerMask.NameToLayer("Ignore Raycast");
  }

  private void InitializeFadeState() {
    colorFade = new ColorFade(Color.white);
    float startingAlpha = startHidden ? 0f : 1f;
    alphaFade = new AlphaFade(startingAlpha);
  }

  void Update() {
    colorFade.Update();
    alphaFade.Update();
    Color newColor = colorFade.CurrentValue();
    newColor.a = alphaFade.CurrentValue();
    pointer.GetComponent<Renderer>().material.color = newColor;
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
    colorFade.To(color);
  }

  public void ClearHighlight() {
    colorFade.To(Color.white);
  }

  public void Hide() {
    alphaFade.To(0f);
  }

  public void Show() {
    alphaFade.To(1f);
  }

  // Create github issue:
  //   Move Highlight, ClearHighlight, Hide, and Show to the pointer object itself
  //   in order to allow custom pointers


  // Unfortunately, C# doesn't have duck typing due to its strong typing.
  // This means that I have to create these huge, non-functional classes filled
  // with ugly casting in order to not repeat code with very small differences.
  private abstract class Fade {
    public float counter = 0f;
    protected object source;
    protected object target;
    protected object current;
    public Fade(object initialValue) {
      source = initialValue;
      target = initialValue;
      current = initialValue;
    }
    public virtual void Update() {
      if (counter > 0f) {
        current = GenerateCurrent();
        counter -= Time.deltaTime;
        if (counter < 0f) counter = 0f;
      }
      else {
        current = target;
      }
    }
    protected virtual void Interrupt() {
      target = CurrentPointerValue();
      counter = fadeTime - counter;
    }
    protected virtual float Percentage() {
      return (fadeTime - counter) / fadeTime;
    }
    protected virtual void FadeTo(object newTarget) {
      Interrupt();
      source = target;
      target = newTarget;
    }
    protected abstract object GenerateCurrent();
    protected abstract object CurrentPointerValue();
  }

  private class ColorFade : Fade {
    private const char DELIMITER = ',';
    public ColorFade(Color color) : base(Serialize(color) as object) {}
    protected override object GenerateCurrent() {
      Color source = Deserialize((string)this.source);
      Color target = Deserialize((string)this.target);
      Color newColor = Color.Lerp(source, target, Percentage());
      return (object)Serialize(newColor);
    }
    protected override object CurrentPointerValue() {
      return (object)Serialize(pointer.GetComponent<Renderer>().material.color);
    }
    public void To(Color color) {
      FadeTo(Serialize(color));
    }
    public Color CurrentValue() {
      return Deserialize((string)current);
    }
    private static string Serialize(Color color) {
      return color.r.ToString() + DELIMITER +
             color.g.ToString() + DELIMITER +
             color.b.ToString();
    }
    private static Color Deserialize(string color) {
      string[] array = color.Split(DELIMITER);
      return new Color(float.Parse(array[0]),
                       float.Parse(array[1]),
                       float.Parse(array[2]));
    }
  }

  private class AlphaFade : Fade {
    public AlphaFade(float alpha) : base(alpha as object) {}
    protected override object GenerateCurrent() {
      return (object)Mathf.Lerp((float)source, (float)target, Percentage());
    }
    protected override object CurrentPointerValue() {
      return (object)pointer.GetComponent<Renderer>().material.color.a;
    }
    public void To(float alpha) {
      FadeTo((object)alpha);
    }
    public float CurrentValue() {
      return (float)current;
    }
  }
}
