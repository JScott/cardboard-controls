using UnityEngine;
using System.Collections;

public class ExampleCharacterController : MonoBehaviour {
  private static CardboardInput cardboard;

  public float speed = 1.0f;
  public float jumpSpeed = 1.0f;
  public float gravity = 1.0f;
  private Transform diveCameraTransform;
  private Vector3 moveDirection = Vector3.zero;
  private CharacterController controller;

	void Start () {
    cardboard = new CardboardInput();
    cardboard.OnMagnetDown += CardboardDown;
    cardboard.OnMagnetUp += CardboardUp;
    cardboard.OnMagnetClicked += CardboardClick;

    controller = GetComponent<CharacterController>();
    diveCameraTransform = this.transform.GetChild(0);
	}

  public void CardboardDown(object sender, CardboardEvent cardboardEvent) {
    ChangeSphereColor("SphereDown");
  }

  public void CardboardUp(object sender, CardboardEvent cardboardEvent) {
    ChangeSphereColor("SphereUp");
  }

  public void CardboardClick(object sender, CardboardEvent cardboardEvent) {
    ChangeSphereColor("SphereClick");
    TextMesh textMesh = GameObject.Find("SphereClick/Counter").GetComponent<TextMesh>();
    int increment = int.Parse(textMesh.text) + 1;
    textMesh.text = increment.ToString();
  }

  public void ChangeSphereColor(string name) {
    GameObject sphere = GameObject.Find(name);
    sphere.renderer.material.color = new Color(Random.value, Random.value, Random.value);
  }

	void Update () {
    UpdateInput();
    UpdateScene();
	}

  void UpdateInput() {
    cardboard.Update();

    TextMesh textMesh = GameObject.Find("SphereDown/Counter").GetComponent<TextMesh>();
    if (!cardboard.IsMagnetHeld() ) {
      textMesh.renderer.enabled = Time.time % 1 < 0.5;
    }
    else {
      textMesh.renderer.enabled = true;
      textMesh.text = cardboard.SecondsMagnetHeld().ToString("#.##");
    }
  }

  void UpdateScene() {
    moveDirection.y -= gravity * Time.deltaTime;
    controller.Move(moveDirection * Time.deltaTime);
  }
}
