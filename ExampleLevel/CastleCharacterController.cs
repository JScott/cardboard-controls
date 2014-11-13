using UnityEngine;
using System.Collections;

public class CastleCharacterController : MonoBehaviour {
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
    cardboard.Update(Input.acceleration, Input.compass.rawVector);

    if (!cardboard.IsMagnetHeld()) {
      moveDirection = Vector3.zero;
    }
    else if (cardboard.SecondsMagnetHeld() > 0.5f) {
      moveDirection = diveCameraTransform.forward;
      moveDirection = transform.TransformDirection(moveDirection);
      moveDirection.y = 0;
      moveDirection *= speed;
    }
  }

  void UpdateScene() {
    moveDirection.y -= gravity * Time.deltaTime;
    controller.Move(moveDirection * Time.deltaTime);
  }
}
