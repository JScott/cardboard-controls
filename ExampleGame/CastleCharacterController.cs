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
    cardboard.OnMagnetClicked += Interact;
    controller = GetComponent<CharacterController>();
    diveCameraTransform = this.transform.GetChild(0);
	}

  public void Interact(object sender, CardboardEvent cardboardEvent) {
    RaycastHit hit;
    Ray ray = new Ray(diveCameraTransform.position, diveCameraTransform.forward);
    if (Physics.Raycast(ray, out hit, 10f)) {
      GameObject obj = hit.collider.gameObject;
      if (obj.layer == LayerMask.NameToLayer("Interactable")) {
        GameObject.Find("star_chimes").audio.Play();
        Object.Destroy(obj);
      }
    }
  }

	void Update () {
    UpdateInput();
    UpdateScene();
	}

  void UpdateInput() {
    cardboard.Update();

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
