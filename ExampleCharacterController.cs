using UnityEngine;
using System.Collections;

public class ExampleCharacterController : MonoBehaviour {
  private static CardboardInput cardboard;

  public float speed = 1.0f;
  public float jumpSpeed = 1.0f;
  public float gravity = 1.0f;
  private Transform diveCameraTransform;
  private Vector3 moveDirection = Vector3.zero;

	void Start () {
    // Make sure we never dim the screen
	  Screen.sleepTimeout = SleepTimeout.NeverSleep;
    cardboard = new CardboardInput();
    cardboard.OnMagnetDown += CardboardDown;
    cardboard.OnMagnetUp += CardboardUp;
    cardboard.OnMagnetClicked += CardboardClick;

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

	// Update is called once per frame
	void Update () {
    cardboard.Update(Input.acceleration, Input.compass.rawVector);

    moveDirection = diveCameraTransform.forward * Input.GetAxis("Vertical");
    moveDirection.y = 0;

    moveDirection = transform.TransformDirection(moveDirection);
    moveDirection *= speed;
    if (Input.GetButton("Jump")) {
      moveDirection.y = jumpSpeed;
    }
    moveDirection.y -= gravity * Time.deltaTime;

    CharacterController controller = GetComponent<CharacterController>();
    controller.Move(moveDirection * Time.deltaTime);
	}
}
