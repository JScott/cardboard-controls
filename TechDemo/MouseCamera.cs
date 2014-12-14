using UnityEngine;
using System.Collections;

/*
Replace this camera with the Durovis Dive camera to work on an actual Cardboard.
*/
public class MouseCamera : MonoBehaviour {
  private float lookSpeed = 100.0f;

  void Update () {
    float deltaSpeed = Time.deltaTime * lookSpeed;
    float aroundYAxis = Input.GetAxis("Mouse X") * deltaSpeed;
    float aroundXAxis = -Input.GetAxis("Mouse Y") * deltaSpeed;
    transform.RotateAround(transform.position, transform.right, aroundXAxis);
    transform.RotateAround(transform.position, Vector3.up, aroundYAxis);
  }
}
